using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private BoardTiles BoardTiles;

    public int turn = 1;
    public bool playerTurn = true;

    // Start is called before the first frame update
    void Awake()
    {
        BoardTiles = GameObject.Find("Floor").GetComponent<BoardTiles>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTurn && Input.GetKeyDown("e")) EndTurn();
    }

    public void EndTurn()
    {
        BoardTiles.ClearAllTiles();
        turn++;
        playerTurn = false;
    }
            

    public bool CheckTurnOver()
    {
        foreach (GameObject character in GameObject.FindGameObjectsWithTag("Character"))
        {
            if (character.GetComponent<CharBehaviour>().turnStage < 2) return false;
        }
        return true;
    }

}
