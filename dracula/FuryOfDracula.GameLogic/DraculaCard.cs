using System;
using System.Runtime.Serialization;

namespace FuryOfDracula.GameLogic
{
    [DataContract]
    public class DraculaCard
    {
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
    }
}