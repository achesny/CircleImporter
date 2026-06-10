using System;

namespace CircleImporter.Core.Models
{
    /// <summary>
    /// Extra flags for Circle MUD objects
    /// </summary>
    [Flags]
    public enum ObjectFlags : int
    {
        None = 0,
        Glow = 1,
        Hum = 2,
        NoRent = 4,
        Nodonate = 8,
        Noinvis = 16,
        Invisible = 32,
        Magic = 64,
        Nodrop = 128,
        Bless = 256,
        Antigood = 512,
        Antievil = 1024,
        Antineutral = 2048,
        Antimage = 4096,
        Anticharm = 8192,
        Sold = 16384,
        Poisoned = 32768,
        Nosacrifice = 65536,
        Nosummon = 131072,
        Noharmful = 262144
    }
}
