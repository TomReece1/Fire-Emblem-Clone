using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class GameController : MonoBehaviour
{

    public int turn = 1;
    public bool playerTurn = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
/*        if (CheckTurnOver())
        {
            turn++;
            playerTurn = false;
        }*/
    }


    //A method to cycle through all blue units and check their moveStage
    //If all have moveStage == 2 then playerTurn = false and enemy turn methods start

    public bool CheckTurnOver()
    {
        foreach (GameObject character in GameObject.FindGameObjectsWithTag("Character"))
        {
            if (character.GetComponent<CharBehaviour>().turnStage < 2) return false;
        }
        return true;
    }

}
