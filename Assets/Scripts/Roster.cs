using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roster : MonoBehaviour
{

    public static Roster Instance;

    public class UnitInfo
    {
        public string unitName;
        public string weapon;
        public int m;
        public int r;
        public int max_hp;
        public int dmg;

        public int level;
        public int exp;
        public int str;
        public int def;
        public int spd;
        public int str_g;
        public int def_g;
        public int spd_g;

        public void LevelUp()
        {
            Debug.Log($"{unitName} leveled up! str: {str} to {str + str_g} | def: {def} to {def + def_g} | spd: {spd} to {spd + spd_g}");
            level += 1;
            exp = 0;
            str += str_g;
            def += def_g;
            spd += spd_g;
            
        }

    }

    public List<UnitInfo> fullUnitList = new List<UnitInfo>();
    public List<UnitInfo> unitList = new List<UnitInfo>();

    private void Awake()
    {
        // start of new code
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        // end of new code

        Instance = this;
        DontDestroyOnLoad(gameObject);

        UnitInfo tom = new UnitInfo { unitName = "Tom", weapon = "sword", m = 7, r = 2, max_hp = 30, dmg = 9, level=1, exp=0, str=5, def=4, spd=9, str_g=1, def_g=1, spd_g=2 };
        UnitInfo adam = new UnitInfo { unitName = "Adam", weapon = "lance", m = 8, r = 1, max_hp = 40, dmg = 8, level = 1, exp = 0, str = 4, def = 8, spd = 4, str_g = 1, def_g = 2, spd_g = 1 };
        UnitInfo jarreth = new UnitInfo { unitName = "Jarreth", weapon = "axe", m = 6, r = 1, max_hp = 50, dmg = 10, level = 1, exp = 0, str = 10, def = 5, spd = 6, str_g = 2, def_g = 1, spd_g = 1 };
        fullUnitList.Add(tom);
        fullUnitList.Add(adam);
        fullUnitList.Add(jarreth);

        unitList = fullUnitList;
        PrintRoster();
    }

    public void AddUnit(string name, string weapon, int m, int r, int hp, int dmg)
    {
        UnitInfo unit = new UnitInfo { unitName = name, weapon=weapon, m = m, r=r, max_hp=hp, dmg=dmg };
        unitList.Add(unit);
    }

    //Modify a unit? After a stat increase that's not through level up?

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
