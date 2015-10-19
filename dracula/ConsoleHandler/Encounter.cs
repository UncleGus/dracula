using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EncounterHandler
{
    [DataContract]
    public class Encounter
    {
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string abbreviation { get; set; }
        [DataMember]
        public bool isRevealed { get; set; }

        public Encounter(string newName, string newAbbreviation)
        {
            name = newName;
            abbreviation = newAbbreviation;
            isRevealed = false;
        }

    }
}
