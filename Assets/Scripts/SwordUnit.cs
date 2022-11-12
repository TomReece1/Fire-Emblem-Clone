using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SwordUnit : CharBehaviour
{


    private void Awake()
    {
        Init(5, 1, 50, 9);

    }
    public void Init(int m, int r, int hp, int dmg)
    {
        this.m = m;
        this.r = r;
        this.hp = hp;
        this.dmg = dmg;
        healthBar.SetMaxHealth(hp);
    }

}
