using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraculaSimulator
{
    public class EncounterSet
    {
        public EncounterDetail[] EncounterPool;

        public EncounterSet()
        {
            EncounterPool = CreateEncounterPool();
        }

        private EncounterDetail[] CreateEncounterPool()
        {
            EncounterDetail[] tempEncounterPool = new EncounterDetail[46];
            tempEncounterPool[(int)Encounter.None] = new EncounterDetail("None", "NON");
            tempEncounterPool[(int)Encounter.Ambush1] = new EncounterDetail("Ambush", "AMB");
            tempEncounterPool[(int)Encounter.Ambush2] = new EncounterDetail("Ambush", "AMB");
            tempEncounterPool[(int)Encounter.Ambush3] = new EncounterDetail("Ambush", "AMB");
            tempEncounterPool[(int)Encounter.Assasin] = new EncounterDetail("Assasin", "ASS");
            tempEncounterPool[(int)Encounter.Bats1] = new EncounterDetail("Bats", "BAT");
            tempEncounterPool[(int)Encounter.Bats2] = new EncounterDetail("Bats", "BAT");
            tempEncounterPool[(int)Encounter.Bats3] = new EncounterDetail("Bats", "BAT");
            tempEncounterPool[(int)Encounter.DesecratedSoil1] = new EncounterDetail("Desecrated Soil", "DES");
            tempEncounterPool[(int)Encounter.DesecratedSoil2] = new EncounterDetail("Desecrated Soil", "DES");
            tempEncounterPool[(int)Encounter.DesecratedSoil3] = new EncounterDetail("Desecrated Soil", "DES");
            tempEncounterPool[(int)Encounter.Fog1] = new EncounterDetail("Fog", "FOG");
            tempEncounterPool[(int)Encounter.Fog2] = new EncounterDetail("Fog", "FOG");
            tempEncounterPool[(int)Encounter.Fog3] = new EncounterDetail("Fog", "FOG");
            tempEncounterPool[(int)Encounter.Fog4] = new EncounterDetail("Fog", "FOG");
            tempEncounterPool[(int)Encounter.MinionwithKnife1] = new EncounterDetail("Minion with Knife", "MIK");
            tempEncounterPool[(int)Encounter.MinionwithKnife2] = new EncounterDetail("Minion with Knife", "MIK");
            tempEncounterPool[(int)Encounter.MinionwithKnife3] = new EncounterDetail("Minion with Knife", "MIK");
            tempEncounterPool[(int)Encounter.MinionwithKnifeandPistol1] = new EncounterDetail("Minion with Knife and Pistol", "MIP");
            tempEncounterPool[(int)Encounter.MinionwithKnifeandPistol2] = new EncounterDetail("Minion with Knife and Pistol", "MIP");
            tempEncounterPool[(int)Encounter.MinionwithKnifeandRifle1] = new EncounterDetail("Minion with Knife and Rifle", "MIR");
            tempEncounterPool[(int)Encounter.MinionwithKnifeandRifle2] = new EncounterDetail("Minion with Knife and Rifle", "MIR");
            tempEncounterPool[(int)Encounter.Hoax1] = new EncounterDetail("Hoax", "HOA");
            tempEncounterPool[(int)Encounter.Hoax2] = new EncounterDetail("Hoax", "HOA");
            tempEncounterPool[(int)Encounter.Lightning1] = new EncounterDetail("Lightning", "LIG");
            tempEncounterPool[(int)Encounter.Lightning2] = new EncounterDetail("Lightning", "LIG");
            tempEncounterPool[(int)Encounter.Peasants1] = new EncounterDetail("Peasants", "PEA");
            tempEncounterPool[(int)Encounter.Peasants2] = new EncounterDetail("Peasants", "PEA");
            tempEncounterPool[(int)Encounter.Plague] = new EncounterDetail("Plague", "PLA");
            tempEncounterPool[(int)Encounter.Rats1] = new EncounterDetail("Rats", "RAT");
            tempEncounterPool[(int)Encounter.Rats2] = new EncounterDetail("Rats", "RAT");
            tempEncounterPool[(int)Encounter.Saboteur1] = new EncounterDetail("Saboteur", "SAB");
            tempEncounterPool[(int)Encounter.Saboteur2] = new EncounterDetail("Saboteur", "SAB");
            tempEncounterPool[(int)Encounter.Spy1] = new EncounterDetail("Spy", "SPY");
            tempEncounterPool[(int)Encounter.Spy2] = new EncounterDetail("Spy", "SPY");
            tempEncounterPool[(int)Encounter.Thief1] = new EncounterDetail("Thief", "THI");
            tempEncounterPool[(int)Encounter.Thief2] = new EncounterDetail("Thief", "THI");
            tempEncounterPool[(int)Encounter.NewVampire1] = new EncounterDetail("New Vampire", "VAM");
            tempEncounterPool[(int)Encounter.NewVampire2] = new EncounterDetail("New Vampire", "VAM");
            tempEncounterPool[(int)Encounter.NewVampire3] = new EncounterDetail("New Vampire", "VAM");
            tempEncounterPool[(int)Encounter.NewVampire4] = new EncounterDetail("New Vampire", "VAM");
            tempEncounterPool[(int)Encounter.NewVampire5] = new EncounterDetail("New Vampire", "VAM");
            tempEncounterPool[(int)Encounter.NewVampire6] = new EncounterDetail("New Vampire", "VAM");
            tempEncounterPool[(int)Encounter.Wolves1] = new EncounterDetail("Wolves", "WOL");
            tempEncounterPool[(int)Encounter.Wolves2] = new EncounterDetail("Wolves", "WOL");
            tempEncounterPool[(int)Encounter.Wolves3] = new EncounterDetail("Wolves", "WOL");
            return tempEncounterPool;
        }

        public string EncounterName(Encounter encounter)
        {
            return EncounterPool[(int)encounter].name;
        }
    }
}
