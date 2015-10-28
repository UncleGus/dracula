using System.Runtime.Serialization;

namespace FuryOfDracula.GameLogic
{
    [DataContract]
    public class EncounterTile
    {
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

        [DataMember]
        public Encounter Encounter { get; private set; }

        [DataMember]
        public bool IsRevealed { get; set; }

        [DataMember]
        public string Abbreviation { get; private set; }
    }
}