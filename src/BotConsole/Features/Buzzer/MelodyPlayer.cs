namespace BotConsole.Features.Buzzer;

/// <summary>
/// Buzzer wrapper which allows to play melody element sequences with desired tempo.
/// </summary>
public class MelodyPlayer : IDisposable
{
    private readonly Iot.Device.Buzzer.Buzzer _buzzer;
    private int _wholeNoteDurationInMilliseconds;

    /// <summary>
    /// Create MelodyPlayer.
    /// </summary>
    /// <param name="buzzer">Buzzer instance to be played on.</param>
    public MelodyPlayer(Iot.Device.Buzzer.Buzzer buzzer) => _buzzer = buzzer;

    /// <summary>
    /// Create MelodyPlayer.
    /// </summary>
    /// <param name="chip">Buzzer chip</param>
    /// <param name="channel">Buzzer channel</param>
    public MelodyPlayer(int chip, int channel)
    {
        _buzzer = new Iot.Device.Buzzer.Buzzer(chip, channel);
    }
    
    /// <summary>
    /// Create MelodyPlayer.
    /// </summary>
    /// <param name="pinNumber">Pin connected to buzzer</param>
    public MelodyPlayer(int pinNumber)
    {
        _buzzer = new Iot.Device.Buzzer.Buzzer(pinNumber);
    }

    /// <summary>
    /// Play melody elements sequence.
    /// </summary>
    /// <param name="sequence">Sequence of pauses and notes elements to be played.</param>
    /// <param name="tempo">Tempo of melody playing.</param>
    /// <param name="tonesToTranspose">Tones to transpose</param>
    public void Play(IList<MelodyElement> sequence, int tempo, int tonesToTranspose = 0)
    {
        _wholeNoteDurationInMilliseconds = GetWholeNoteDurationInMilliseconds(tempo);
        sequence = TransposeSequence(sequence, tonesToTranspose);
        foreach (var element in sequence)
        {
            PlayElement(element);
        }
    }

    private static IList<MelodyElement> TransposeSequence(IList<MelodyElement> sequence, int tonesToTranspose)
    {
        if (tonesToTranspose == 0)
        {
            return sequence;
        }

        return sequence
            .Select(element => TransposeElement(element, tonesToTranspose))
            .ToList();
    }

    private static MelodyElement TransposeElement(MelodyElement element, int tonesToTranspose)
    {
        if (element is not NoteElement noteElement)
        {
            return element;
        }

        // Every octave consists of 12 notes numbered from 1 to 12. Each octave numbered from 1 to 8.
        // To transpose over octaves we will get absolute index of note like (octave index - 1) * 12 + (note index - 1).
        // As far as octave and note numbered starting from 1 we decrease it's values by 1 for farther calculation.
        var absoluteNoteNumber = ((int) noteElement.Octave - 1) * 12 + ((int) noteElement.Note - 1);

        // Then we transpose absolute number. In case gotten value exceeds maximum value of 96 (12 notes * 8 octave)
        // we calculate remainder of dividing by 96. In case gotten value is below 0 we add 96.
        absoluteNoteNumber = (absoluteNoteNumber + tonesToTranspose) % 96;
        absoluteNoteNumber = absoluteNoteNumber >= 0 ? absoluteNoteNumber : absoluteNoteNumber + 96;

        // Then to get transposed octave index we divide transposed absolute index
        // value by 12 and increase it by 1 due to we decreased it by 1 before.
        // To get transposed note index we will get remainder of dividing by 12
        // and increase it by 1 due to we decreased it by 1 before.
        noteElement.Octave = (Octave) (absoluteNoteNumber / 12 + 1);
        noteElement.Note = (Note) (absoluteNoteNumber % 12 + 1);
        return noteElement;
    }

    private void PlayElement(MelodyElement element)
    {
        var durationInMilliseconds = _wholeNoteDurationInMilliseconds / (int) element.Duration;

        if (element is not NoteElement noteElement)
        {
            // In case it's a pause element we have only just wait desired time.
            Thread.Sleep(durationInMilliseconds);
        }
        else
        {
            // In case it's a note element we play it.
            var frequency = GetFrequency(noteElement.Note, noteElement.Octave);
            _buzzer.PlayTone(frequency, (int) (durationInMilliseconds * 0.7));
            Thread.Sleep((int) (durationInMilliseconds * 0.3));
        }
    }

    private static readonly Dictionary<Note, double> NotesOfEightOctaveToFrequenciesMap
        = new Dictionary<Note, double>
        {
            {Note.C, 4186.01},
            {Note.Db, 4434.92},
            {Note.D, 4698.63},
            {Note.Eb, 4978.03},
            {Note.E, 5274.04},
            {Note.F, 5587.65},
            {Note.Gb, 5919.91},
            {Note.G, 6271.93},
            {Note.Ab, 6644.88},
            {Note.A, 7040.00},
            {Note.Bb, 7458.62},
            {Note.B, 7902.13}
        };

    // In music tempo defines amount of quarter notes per minute.
    // Dividing minute (60 * 1000) by tempo we get duration of quarter note.
    // Whole note duration equals to four quarters.
    private static int GetWholeNoteDurationInMilliseconds(int tempo) => 4 * 60 * 1000 / tempo;

    private static double GetFrequency(Note note, Octave octave)
    {
        // We could decrease octave of every note by 1 by dividing it's frequency by 2.
        // We have predefined frequency of every note of eighth octave rounded to 2 decimals.
        // To get note frequency of any other octave we have to divide note frequency of eighth octave by 2 n times,
        // where n is a difference between eight octave and desired octave.
        var eightOctaveNoteFrequency = GetNoteFrequencyOfEightOctave(note);
        var frequencyDivider = Math.Pow(2, 8 - (int) octave);
        return Math.Round(eightOctaveNoteFrequency / frequencyDivider, 2);
    }

    private static double GetNoteFrequencyOfEightOctave(Note note)
    {
        if (NotesOfEightOctaveToFrequenciesMap.TryGetValue(note, out double result))
        {
            return result;
        }

        return 0;
    }

    public void Dispose() => _buzzer.Dispose();
}