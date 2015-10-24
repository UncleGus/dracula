using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace FuryOfDracula.GameLogic
{
    [DataContract]
    public class EncounterTile
    {
        [DataMember]
        public Encounter Encounter { get; private set; }
        [DataMember]
        public bool IsRevealed { get; set; }
        [DataMember]
        public string Abbreviation { get; private set; }

        public EncounterTile(Encounter encounter)
        {
            Encounter = encounter;
            IsRevealed = false;
            Abbreviation = encounter.Name().Substring(0, 3).ToUpper();
        }

        public EncounterTile(Encounter encounter, string abbreviation)
        {
            Encounter = encounter;
            IsRevealed = false;
            Abbreviation = abbreviation;
        }

    }
}
