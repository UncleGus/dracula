using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncounterHandler
{
    public class Encounter
    {
        public string name;
        public string abbreviation;
        public bool isRevealed;

        public Encounter(string newName, string newAbbreviation)
        {
            name = newName;
            abbreviation = newAbbreviation;
            isRevealed = false;
        }
    }
}
