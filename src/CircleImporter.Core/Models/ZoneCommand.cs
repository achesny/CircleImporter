namespace CircleImporter.Core.Models;

/// <summary>
/// Represents a single zone reset command in a Circle MUD zone.
/// </summary>
public class ZoneCommand
{
    /// <summary>
    /// Command type: D (delete), M (load mob), O (load object), P (put in container), 
    /// G (give to mob), E (equip to mob), R (remove from room), S (set door state).
    /// </summary>
    public char Command { get; set; }

    /// <summary>
    /// If condition: 0 (always), 1 (if last command failed), 2 (if last command succeeded).
    /// </summary>
    public int IfFlag { get; set; }

    /// <summary>
    /// First argument - typically a vnumber (mob, object, or room).
    /// </summary>
    public int Arg1 { get; set; }

    /// <summary>
    /// Second argument - typically a vnumber or count.
    /// </summary>
    public int Arg2 { get; set; }

    /// <summary>
    /// Third argument - typically a vnumber or direction/state.
    /// </summary>
    public int Arg3 { get; set; }

    /// <summary>
    /// Optional comment/description of the command.
    /// </summary>
    public string Comment { get; set; } = string.Empty;
}
