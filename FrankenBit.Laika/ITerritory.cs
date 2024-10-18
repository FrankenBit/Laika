namespace FrankenBit.Laika;

/// <summary>
///    Represents the target project for mutation testing.
/// </summary>
internal interface ITerritory
{
    /// <summary>
    ///     Occurs when the <see cref="ITerritory" /> has changed.
    /// </summary>
    event Action? HasChanged;
}
