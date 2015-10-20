using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DraculaSimulator
{
    public class EncounterDetail
    {
        public string name { get; set; }
        public string abbreviation { get; set; }
        public bool isRevealed { get; set; }

        public EncounterDetail(string newName, string newAbbreviation)
        {
            name = newName;
            abbreviation = newAbbreviation;
        }

    }

    public enum Encounter
    {
        None,
        Ambush1,
        Ambush2,
        Ambush3,
        Assasin,
        Bats1,
        Bats2,
        Bats3,
        DesecratedSoil1,
        DesecratedSoil2,
        DesecratedSoil3,
        Fog1,
        Fog2,
        Fog3,
        Fog4,
        MinionwithKnife1,
        MinionwithKnife2,
        MinionwithKnife3,
        MinionwithKnifeandPistol1,
        MinionwithKnifeandPistol2,
        MinionwithKnifeandRifle1,
        MinionwithKnifeandRifle2,
        Hoax1,
        Hoax2,
        Lightning1,
        Lightning2,
        Peasants1,
        Peasants2,
        Plague,
        Rats1,
        Rats2,
        Saboteur1,
        Saboteur2,
        Spy1,
        Spy2,
        Thief1,
        Thief2,
        NewVampire1,
        NewVampire2,
        NewVampire3,
        NewVampire4,
        NewVampire5,
        NewVampire6,
        Wolves1,
        Wolves2,
        Wolves3
    }
}
