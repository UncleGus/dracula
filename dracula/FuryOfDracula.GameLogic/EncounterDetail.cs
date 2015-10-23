using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace FuryOfDracula.GameLogic
{
    [DataContract]
    public class EncounterDetail
    {
        [DataMember]
        public Encounter Encounter { get; private set; }
        [DataMember]
        public bool IsRevealed { get; set; }

        public EncounterDetail(Encounter encounter)
        {
            
            Encounter = encounter;
            IsRevealed = false;
        }
    }
}
