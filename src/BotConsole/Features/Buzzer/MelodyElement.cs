// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
namespace BotConsole.Features.Buzzer;

/// <summary>
/// A base class for melody sequence elements.
/// </summary>
public abstract class MelodyElement
{
    /// <summary>
    /// Duration which defines how long should element take on melody sequence timeline.
    /// </summary>
    public Duration Duration { get; set; }

    // ReSharper disable once PublicConstructorInAbstractClass
    public MelodyElement(Duration duration) => Duration = duration;
}