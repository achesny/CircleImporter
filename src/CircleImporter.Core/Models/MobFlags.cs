namespace CircleImporter.Core.Models;

/// <summary>
/// Flags that define mob behaviors and properties.
/// </summary>
[Flags]
public enum MobFlags
{
    Spec = 1,           // Mob has a special procedure
    Sentinel = 2,       // Mob should not move
    Scavenger = 4,      // Mob collects items
    IhReserved = 8,     // Reserved for internal use
    Aggressive = 16,    // Mob attacks all players
    StayZone = 32,      // Mob stays in zone
    Wimpy = 64,         // Mob flees when wounded
    AggressiveToEvil = 128,    // Mob attacks evil players
    AggressiveToGood = 256,    // Mob attacks good players
    AggressiveToNeutral = 512, // Mob attacks neutral players
    Memory = 1024,      // Mob remembers attackers
    Helper = 2048,      // Mob helps other mobs
    NoCharm = 4096,     // Mob cannot be charmed
    NoSummon = 8192,    // Mob cannot be summoned
    NoStun = 16384,     // Mob cannot be stunned
    NoSleep = 32768,    // Mob cannot be put to sleep
    NoFreeze = 65536,   // Mob cannot be frozen
    NoTracking = 131072, // Mob cannot be tracked
    Questor = 262144,   // Mob is a quest NPC
    Berserk = 524288    // Mob goes berserk
}
