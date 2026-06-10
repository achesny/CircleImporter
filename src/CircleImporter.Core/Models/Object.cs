using System.Collections.Generic;

namespace CircleImporter.Core.Models
{
    public class Object
    {
        public int Vnumber { get; set; }
        public string Name { get; set; } = "";
        public string ShortDescription { get; set; } = "";
        public string LongDescription { get; set; } = "";
        public string ActionDescription { get; set; } = "";
        
        public int Type { get; set; }
        public int ExtraFlags { get; set; }
        public int WearFlags { get; set; }
        
        public int Weight { get; set; }
        public int Value { get; set; }
        public int Rent { get; set; }
        
        public List<int> Values { get; set; } = new List<int>();
        public List<ObjectAffect> Affects { get; set; } = new List<ObjectAffect>();
    }

    public class ObjectAffect
    {
        public int Location { get; set; }
        public int Modifier { get; set; }
    }
}
