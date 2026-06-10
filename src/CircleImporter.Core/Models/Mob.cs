namespace CircleImporter.Core.Models;

/// <summary>
/// Represents a mobile (NPC) in Circle MUD.
/// </summary>
public class Mob
{
    /// <summary>
    /// Virtual number (unique identifier) of the mob.
    /// </summary>
    public int Vnumber { get; set; }

    /// <summary>
    /// Keywords/names that identify the mob.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Short description of the mob (shown in room).
    /// </summary>
    public string ShortDescription { get; set; } = string.Empty;

    /// <summary>
    /// Long description of the mob (shown when looking).
    /// </summary>
    public string LongDescription { get; set; } = string.Empty;

    /// <summary>
    /// Description shown when mob performs actions like attacking.
    /// </summary>
    public string ActionDescription { get; set; } = string.Empty;

    /// <summary>
    /// Race of the mob.
    /// </summary>
    public int Race { get; set; }

    /// <summary>
    /// Sex of the mob (0=neutral, 1=male, 2=female).
    /// </summary>
    public int Sex { get; set; }

    /// <summary>
    /// Class of the mob (0=magic user, 1=cleric, 2=thief, 3=warrior).
    /// </summary>
    public int Class { get; set; }

    /// <summary>
    /// Level of the mob.
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// Hit roll modifier for the mob's attacks.
    /// </summary>
    public int Hitroll { get; set; }

    /// <summary>
    /// Damage roll modifier for the mob's attacks.
    /// </summary>
    public int Damroll { get; set; }

    /// <summary>
    /// Armor class of the mob (lower is better).
    /// </summary>
    public int Ac { get; set; }

    /// <summary>
    /// Hit points of the mob.
    /// </summary>
    public int Hp { get; set; }

    /// <summary>
    /// Mana points of the mob.
    /// </summary>
    public int Mana { get; set; }

    /// <summary>
    /// Movement points of the mob.
    /// </summary>
    public int Move { get; set; }

    /// <summary>
    /// Gold carried by the mob.
    /// </summary>
    public int Gold { get; set; }

    /// <summary>
    /// Experience points gained for killing this mob.
    /// </summary>
    public int Experience { get; set; }

    /// <summary>
    /// Current position of the mob (0=dead, 1=mortally wounded, 2=incapacitated, 3=stunned, 4=sleeping, 5=resting, 6=sitting, 7=fighting, 8=standing).
    /// </summary>
    public int Position { get; set; }

    /// <summary>
    /// Default position of the mob when not fighting.
    /// </summary>
    public int DefaultPosition { get; set; }

    /// <summary>
    /// Mob flags indicating special behaviors and properties.
    /// </summary>
    public int Flags { get; set; }

    /// <summary>
    /// Alignment of the mob (-1000 to 1000, -1000 evil to 1000 good).
    /// </summary>
    public int Alignment { get; set; }
}
