namespace CircleImporter.Core.Models;

/// <summary>
/// Represents a zone in Circle MUD.
/// </summary>
public class Zone
{
    /// <summary>
    /// Virtual number (unique identifier) of the zone.
    /// </summary>
    public int Vnumber { get; set; }

    /// <summary>
    /// Name of the zone.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Bottom vnumber of the zone range.
    /// </summary>
    public int Bottom { get; set; }

    /// <summary>
    /// Top vnumber of the zone range.
    /// </summary>
    public int Top { get; set; }

    /// <summary>
    /// Lifespan in minutes - how often the zone resets.
    /// </summary>
    public int Lifespan { get; set; }

    /// <summary>
    /// Reset mode: 0 (never), 1 (only if empty), 2 (always).
    /// </summary>
    public int ResetMode { get; set; }

    /// <summary>
    /// Reset commands that execute when the zone resets.
    /// </summary>
    public List<ZoneCommand> Commands { get; set; } = new();
}
