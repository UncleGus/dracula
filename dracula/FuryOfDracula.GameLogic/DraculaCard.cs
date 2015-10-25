using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FuryOfDracula.GameLogic
{
    [DataContract]
    public class DraculaCard
    {
        [DataMember]
        public string Abbreviation { get; private set; }
        [DataMember]
        public Location Location { get; private set; }
        [DataMember]
        public Power Power { get; private set; }
        [DataMember]
        public bool IsRevealed { get; set; }
        [DataMember]
        public ConsoleColor Color { get; private set; }

        public DraculaCard(string abbreviation, Location location, Power power)
        {
            Abbreviation = abbreviation;
            Location = location;
            Power = power;
            IsRevealed = false;
            Color = ConsoleColor.DarkYellow;
        }

        public DraculaCard(string abbreviation, Location location, Power power, ConsoleColor color)
        {
            Abbreviation = abbreviation;
            Location = location;
            Power = power;
            IsRevealed = false;
            Color = color;
        }
    }
}
