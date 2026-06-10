namespace CircleImporter.Core.Models
{
    /// <summary>
    /// Sector type constants for Circle MUD rooms
    /// </summary>
    public enum SectorType : int
    {
        Inside = 0,
        City = 1,
        Field = 2,
        Forest = 3,
        Hills = 4,
        Mountains = 5,
        WaterSwim = 6,
        WaterNoSwim = 7,
        Underwater = 8,
        Flying = 9
    }
}
