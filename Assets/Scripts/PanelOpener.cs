using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//adams contribution

public class PanelOpener : MonoBehaviour
{

    public GameObject PanelText;
    private void Awake() 
    {
       
    }

    public void UpdateStats(CharBehaviour stats)
    {
   
        PanelText.GetComponent<TextMeshProUGUI>().text = 
            $"Name: {stats.unitName} \n" +
            $"Level: {stats.level} \n" +
            $"Health: {stats.hp} \n" +
            $"Damage: {stats.dmg} \n";
                                             
    }
}
