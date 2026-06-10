public class Room
{
    public int Vnumber { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";

    public int RoomFlags { get; set; }
    public int SectorType { get; set; }

    public List<Exit> Exits { get; set; } = new List<Exit>();
}