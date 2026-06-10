using System;

namespace CircleImporter.Core.Models
{
    /// <summary>
    /// Room flag constants for Circle MUD rooms
    /// </summary>
    [Flags]
    public enum RoomFlags : int
    {
        None = 0,
        Dark = 1,
        DeathTrap = 2,
        NoMob = 4,
        IndoorOnly = 8,
        Peaceful = 16,
        SoundProof = 32,
        NoTrack = 64,
        NoMagic = 128,
        TunnelType = 256,
        Privat = 512,
        Godroom = 1024,
        House = 2048,
        Housecrash = 4096,
        Atrium = 8192,
        OlcZone = 16384,
        BfsMarked = 32768
    }
}
