using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

//adams contribution

public class PanelOpener : MonoBehaviour
{

    public GameObject PanelText;
    public void UpdateStats(CharBehaviour stats)
        {
            PanelText.GetComponent<TextMeshProUGUI>().text = 
             $"Name: {stats.unitName} \n" +
             $"Level: {stats.level} \n" +
             $"Health: {stats.hp} \n" +
             $"Damage: {stats.dmg} \n";                                 
        }
}