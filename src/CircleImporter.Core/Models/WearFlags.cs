using System;

namespace CircleImporter.Core.Models
{
    /// <summary>
    /// Wear flags for Circle MUD objects
    /// </summary>
    [Flags]
    public enum WearFlags : int
    {
        None = 0,
        Take = 1,
        Finger = 2,
        Neck = 4,
        Body = 8,
        Head = 16,
        Legs = 32,
        Feet = 64,
        Hands = 128,
        Arms = 256,
        Shield = 512,
        About = 1024,
        Waist = 2048,
        Wrist = 4096,
        Wield = 8192,
        Hold = 16384,
        Thrown = 32768,
        Worn = 65536
    }
}
