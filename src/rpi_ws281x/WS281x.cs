using Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;

namespace rpi_ws281x
{
    /// <summary>
    /// Wrapper class to control WS281x LEDs
    /// </summary>
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    // ReSharper disable once InconsistentNaming
    public class WS281x : IDisposable
    {
        private ws2811_t _ws2811;
        private GCHandle _ws2811Handle;
        private readonly Dictionary<int, Controller> _controllers;

        private bool _isDisposingAllowed;

        /// <summary>
        /// Initialize the wrapper
        /// </summary>
        /// <param name="settings">Settings used for initialization</param>
        public WS281x(Settings settings)
        {
            _ws2811 = new ws2811_t
            {
                dmanum = settings.DMAChannel,
                freq = settings.Frequency,
                channel_0 = InitChannel(0, settings.Controllers),
                channel_1 = InitChannel(1, settings.Controllers)
            };

            //Pin the object in memory. Otherwise GC will probably move the object to another memory location.
            //This would cause errors because the native library has a pointer on the memory location of the object.
            _ws2811Handle = GCHandle.Alloc(_ws2811, GCHandleType.Pinned);
            var initResult = PInvoke.ws2811_init(ref _ws2811);
            if (initResult != ws2811_return_t.WS2811_SUCCESS)
            {
                throw WS281xException.Create(initResult, "initializing");
            }

            // save a copy of the controllers - used to update LEDs
            _controllers = new Dictionary<int, Controller>(settings.Controllers);

            // if specified, apply gamma correction
            if (settings.GammaCorrection != null)
            {
                if (settings.Controllers.ContainsKey(0))
                    Marshal.Copy(settings.GammaCorrection.ToArray(), 0, _ws2811.channel_0.gamma,
                        settings.GammaCorrection.Count);
                if (settings.Controllers.ContainsKey(1))
                    Marshal.Copy(settings.GammaCorrection.ToArray(), 0, _ws2811.channel_1.gamma,
                        settings.GammaCorrection.Count);
            }

            //Disposing is only allowed if the init was successful.
            //Otherwise the native cleanup function throws an error.
            _isDisposingAllowed = true;
        }

        /// <summary>
        /// Renders the content of the channels
        /// </summary>
        /// <param name="force">Force LEDs to updated - default only updates if when a change is done</param>
        public void Render(bool force = false)
        {
            var shouldRender = false;

            if (_controllers.ContainsKey(0) && (force || _controllers[0].IsDirty))
            {
                var ledColor = _controllers[0].GetColors(true);
                Marshal.Copy(ledColor, 0, _ws2811.channel_0.leds, ledColor.Length);
                shouldRender = true;
            }

            if (_controllers.ContainsKey(1) && (force || _controllers[1].IsDirty))
            {
                var ledColor = _controllers[1].GetColors(true);
                Marshal.Copy(ledColor, 0, _ws2811.channel_1.leds, ledColor.Length);
                shouldRender = true;
            }

            if (!shouldRender) return;
            var result = PInvoke.ws2811_render(ref _ws2811);
            if (result != ws2811_return_t.WS2811_SUCCESS)
            {
                throw WS281xException.Create(result, "rendering");
            }
        }

        /// <summary>
        /// Set all LEDs (on all controllers) to the same color.
        /// </summary>
        /// <param name="color">color to display</param>
        public void SetAll(Color color)
        {
            foreach (var controller in _controllers.Values)
            {
                controller.SetAll(color);
                controller.IsDirty = false;
            }

            Render(true);
        }

        /// <summary>
        /// Clear all LEDs
        /// </summary>
        public void Reset()
        {
            foreach (var controller in _controllers.Values)
            {
                controller.Reset();
                controller.IsDirty = false;
            }

            Render(true);
        }

        public Controller GetController(ControllerType controllerType = ControllerType.PWM0)
        {
            int channelNumber = (controllerType == ControllerType.PWM1) ? 1 : 0;
            if (_controllers.ContainsKey(channelNumber) &&
                _controllers[channelNumber].ControllerType == controllerType)
            {
                return _controllers[channelNumber];
            }

            return null;
        }

        /// <summary>
        /// Initialize the channel properties
        /// </summary>
        /// <param name="channelIndex">Index of the channel tu initialize</param>
        /// <param name="controllers">Controller Settings</param>
        private ws2811_channel_t InitChannel(int channelIndex, Dictionary<int, Controller> controllers)
        {
            ws2811_channel_t channel = new ws2811_channel_t();

            if (!controllers.ContainsKey(channelIndex)) return channel;
            channel.count = controllers[channelIndex].LEDCount;
            channel.gpionum = controllers[channelIndex].GPIOPin;
            channel.brightness = controllers[channelIndex].Brightness;
            channel.invert = Convert.ToInt32(controllers[channelIndex].Invert);

            if (controllers[channelIndex].StripType != StripType.Unknown)
            {
                //Strip type is set by the native assembly if not explicitly set.
                //This type defines the ordering of the colors e. g. RGB or GRB, ...
                channel.strip_type = (int) controllers[channelIndex].StripType;
            }

            return channel;
        }

        #region Obsolete

        [Obsolete(
            "GetMessageForStatusCode is depreciated. Returned string is mangled due to ANSI/UNICODE conversion. Use WS281xException.GetErrorMessage(...) instead.")]
        public string GetMessageForStatusCode(ws2811_return_t statusCode)
        {
            var strPointer = PInvoke.ws2811_get_return_t_str((int) statusCode);
            return Marshal.PtrToStringAuto(strPointer);
        }

        [Obsolete("SetLEDColor is depreciated, please use GetController(controllerType).SetLED(ledID,color) instead")]
        // ReSharper disable once InconsistentNaming
        // ReSharper disable once UnusedMember.Global
        public void SetLEDColor(int channelIndex, int ledId, Color color)
        {
            if (_controllers.ContainsKey(channelIndex))
            {
                _controllers[channelIndex].SetLED(ledId, color);
            }
        }

        #endregion

        #region IDisposable Support

        private bool _disposedValue; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                if (_isDisposingAllowed)
                {
                    PInvoke.ws2811_fini(ref _ws2811);
                    _ws2811Handle.Free();

                    _isDisposingAllowed = false;
                }

                _disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~WS281x()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        #endregion
    }
}