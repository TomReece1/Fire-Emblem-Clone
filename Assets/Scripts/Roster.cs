using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roster : MonoBehaviour
{
    public class UnitInfo
    {
        public string unitName;
        public string weapon;
        public int m;
        public int r;
        public int hp;
        public int dmg;
    }

    public List<UnitInfo> unitList = new List<UnitInfo>();

    public void AddUnit(string name, string weapon, int m, int r, int hp, int dmg)
    {
        UnitInfo unit = new UnitInfo { unitName = name, weapon=weapon, m = m, r=r, hp=hp, dmg=dmg };
        unitList.Add(unit);
    }

    public void RemoveUnit(int index)
    {
        unitList.Remove(unitList[index]);
    }

    public void PrintRoster()
    {
        Debug.Log($"{unitList.Count} units in roster");
        Debug.Log($"{unitList[0].unitName} is first with {unitList[0].dmg} dmg");
    }

}
