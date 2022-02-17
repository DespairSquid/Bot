namespace BotConsole.Features.Buzzer.Songs;

public static class AlphabetSong
{
    public static List<MelodyElement> Song { get; } = new()
    {
        new NoteElement(Note.C, Octave.Fourth, Duration.Quarter), // A
        new NoteElement(Note.C, Octave.Fourth, Duration.Quarter), // B
        new NoteElement(Note.G, Octave.Fourth, Duration.Quarter), // C
        new NoteElement(Note.G, Octave.Fourth, Duration.Quarter), // D
        new NoteElement(Note.A, Octave.Fourth, Duration.Quarter), // E
        new NoteElement(Note.A, Octave.Fourth, Duration.Quarter), // F
        new NoteElement(Note.G, Octave.Fourth, Duration.Half),    // G
        new NoteElement(Note.F, Octave.Fourth, Duration.Quarter), // H
        new NoteElement(Note.F, Octave.Fourth, Duration.Quarter), // I
        new NoteElement(Note.E, Octave.Fourth, Duration.Quarter), // J
        new NoteElement(Note.E, Octave.Fourth, Duration.Quarter), // K
        new NoteElement(Note.D, Octave.Fourth, Duration.Eighth),  // L
        new NoteElement(Note.D, Octave.Fourth, Duration.Eighth),  // M
        new NoteElement(Note.D, Octave.Fourth, Duration.Eighth),  // N
        new NoteElement(Note.D, Octave.Fourth, Duration.Eighth),  // O
        new NoteElement(Note.C, Octave.Fourth, Duration.Half),    // P
        new NoteElement(Note.G, Octave.Fourth, Duration.Quarter), // Q
        new NoteElement(Note.G, Octave.Fourth, Duration.Quarter), // R
        new NoteElement(Note.F, Octave.Fourth, Duration.Half),    // S
        new NoteElement(Note.E, Octave.Fourth, Duration.Quarter), // T
        new NoteElement(Note.E, Octave.Fourth, Duration.Quarter), // U
        new NoteElement(Note.D, Octave.Fourth, Duration.Half),    // V
        new NoteElement(Note.G, Octave.Fourth, Duration.Eighth),  // Dou-
        new NoteElement(Note.G, Octave.Fourth, Duration.Eighth),  // ble
        new NoteElement(Note.G, Octave.Fourth, Duration.Quarter), // U
        new NoteElement(Note.F, Octave.Fourth, Duration.Half),    // X
        new NoteElement(Note.E, Octave.Fourth, Duration.Quarter), // Y
        new NoteElement(Note.E, Octave.Fourth, Duration.Quarter), // and
        new NoteElement(Note.D, Octave.Fourth, Duration.Half),    // Z
    };
}