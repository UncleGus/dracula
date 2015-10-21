using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuryOfDracula.GameLogic
{
    public class EncounterSet
    {
        private EncounterDetail[] encounterDeck;

        public EncounterSet()
        {
            encounterDeck = CreateEncounterDeck();
        }

        private EncounterDetail[] CreateEncounterDeck()
        {
            EncounterDetail[] tempEncounterDeck = new EncounterDetail[46];
            tempEncounterDeck[(int)Encounter.None] = new EncounterDetail(Encounter.None);
            tempEncounterDeck[(int)Encounter.Ambush1] = new EncounterDetail(Encounter.Ambush1);
            tempEncounterDeck[(int)Encounter.Ambush2] = new EncounterDetail(Encounter.Ambush2);
            tempEncounterDeck[(int)Encounter.Ambush3] = new EncounterDetail(Encounter.Ambush3);
            tempEncounterDeck[(int)Encounter.Assassin] = new EncounterDetail(Encounter.Assassin);
            tempEncounterDeck[(int)Encounter.Bats1] = new EncounterDetail(Encounter.Bats1);
            tempEncounterDeck[(int)Encounter.Bats2] = new EncounterDetail(Encounter.Bats2);
            tempEncounterDeck[(int)Encounter.Bats3] = new EncounterDetail(Encounter.Bats3);
            tempEncounterDeck[(int)Encounter.DesecratedSoil1] = new EncounterDetail(Encounter.DesecratedSoil1);
            tempEncounterDeck[(int)Encounter.DesecratedSoil2] = new EncounterDetail(Encounter.DesecratedSoil2);
            tempEncounterDeck[(int)Encounter.DesecratedSoil3] = new EncounterDetail(Encounter.DesecratedSoil3);
            tempEncounterDeck[(int)Encounter.Fog1] = new EncounterDetail(Encounter.Fog1);
            tempEncounterDeck[(int)Encounter.Fog2] = new EncounterDetail(Encounter.Fog2);
            tempEncounterDeck[(int)Encounter.Fog3] = new EncounterDetail(Encounter.Fog3);
            tempEncounterDeck[(int)Encounter.Fog4] = new EncounterDetail(Encounter.Fog4);
            tempEncounterDeck[(int)Encounter.MinionWithKnife1] = new EncounterDetail(Encounter.MinionWithKnife1);
            tempEncounterDeck[(int)Encounter.MinionWithKnife2] = new EncounterDetail(Encounter.MinionWithKnife2);
            tempEncounterDeck[(int)Encounter.MinionWithKnife3] = new EncounterDetail(Encounter.MinionWithKnife3);
            tempEncounterDeck[(int)Encounter.MinionWithKnifeAndPistol1] = new EncounterDetail(Encounter.MinionWithKnifeAndPistol1);
            tempEncounterDeck[(int)Encounter.MinionWithKnifeAndPistol2] = new EncounterDetail(Encounter.MinionWithKnifeAndPistol2);
            tempEncounterDeck[(int)Encounter.MinionWithKnifeAndRifle1] = new EncounterDetail(Encounter.MinionWithKnifeAndRifle1);
            tempEncounterDeck[(int)Encounter.MinionWithKnifeAndRifle2] = new EncounterDetail(Encounter.MinionWithKnifeAndRifle2);
            tempEncounterDeck[(int)Encounter.Hoax1] = new EncounterDetail(Encounter.Hoax1);
            tempEncounterDeck[(int)Encounter.Hoax2] = new EncounterDetail(Encounter.Hoax2);
            tempEncounterDeck[(int)Encounter.Lightning1] = new EncounterDetail(Encounter.Lightning1);
            tempEncounterDeck[(int)Encounter.Lightning2] = new EncounterDetail(Encounter.Lightning2);
            tempEncounterDeck[(int)Encounter.Peasants1] = new EncounterDetail(Encounter.Peasants1);
            tempEncounterDeck[(int)Encounter.Peasants2] = new EncounterDetail(Encounter.Peasants2);
            tempEncounterDeck[(int)Encounter.Plague] = new EncounterDetail(Encounter.Plague);
            tempEncounterDeck[(int)Encounter.Rats1] = new EncounterDetail(Encounter.Rats1);
            tempEncounterDeck[(int)Encounter.Rats2] = new EncounterDetail(Encounter.Rats2);
            tempEncounterDeck[(int)Encounter.Saboteur1] = new EncounterDetail(Encounter.Saboteur1);
            tempEncounterDeck[(int)Encounter.Saboteur2] = new EncounterDetail(Encounter.Saboteur2);
            tempEncounterDeck[(int)Encounter.Spy1] = new EncounterDetail(Encounter.Spy1);
            tempEncounterDeck[(int)Encounter.Spy2] = new EncounterDetail(Encounter.Spy2);
            tempEncounterDeck[(int)Encounter.Thief1] = new EncounterDetail(Encounter.Thief1);
            tempEncounterDeck[(int)Encounter.Thief2] = new EncounterDetail(Encounter.Thief2);
            tempEncounterDeck[(int)Encounter.NewVampire1] = new EncounterDetail(Encounter.NewVampire1);
            tempEncounterDeck[(int)Encounter.NewVampire2] = new EncounterDetail(Encounter.NewVampire2);
            tempEncounterDeck[(int)Encounter.NewVampire3] = new EncounterDetail(Encounter.NewVampire3);
            tempEncounterDeck[(int)Encounter.NewVampire4] = new EncounterDetail(Encounter.NewVampire4);
            tempEncounterDeck[(int)Encounter.NewVampire5] = new EncounterDetail(Encounter.NewVampire5);
            tempEncounterDeck[(int)Encounter.NewVampire6] = new EncounterDetail(Encounter.NewVampire6);
            tempEncounterDeck[(int)Encounter.Wolves1] = new EncounterDetail(Encounter.Wolves1);
            tempEncounterDeck[(int)Encounter.Wolves2] = new EncounterDetail(Encounter.Wolves2);
            tempEncounterDeck[(int)Encounter.Wolves3] = new EncounterDetail(Encounter.Wolves3);
            return tempEncounterDeck;
        }

        public List<Encounter> GetEncountersFromString(string line)
        {
            List<Encounter> tempEncounterList = new List<Encounter>();
            foreach (EncounterDetail e in encounterDeck)
            {
                if (e.Encounter.Name().ToLower().StartsWith(line.ToLower()))
                {
                    tempEncounterList.Add(e.Encounter);
                }
            }
            foreach (Encounter e in tempEncounterList)
            {
                if (e.Name() != tempEncounterList.First().Name())
                {
                    return new List<Encounter>();
                }
            }
            return tempEncounterList;
        }

    }
}
