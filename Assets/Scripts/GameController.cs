using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    private BoardTiles BoardTiles;
    //private EnemyBehaviour EnemyBehaviour;
    public GameObject TurnCounter;
    public GameObject ActiveTeam;

    public int turn = 1;
    public bool playerTurn = true;

    public bool gameFrozen = false;



    // Start is called before the first frame update
    void Awake()
    {
        BoardTiles = GameObject.Find("Floor").GetComponent<BoardTiles>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameFrozen)
        {
            if (playerTurn && Input.GetKeyDown("e")) EndTurn();

            if (!playerTurn)
            {
                Debug.Log("Start enemy turn");

                //loop through all enemies and call MakeMove for them
                foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
                {
                    Debug.Log("Before invoking MakeMove");
                    enemy.GetComponent<EnemyBehaviour>().MakeMove();
                    Debug.Log("MakeMove ended #####################################");
                }
                Debug.Log("Finished MakeMove foreach loop in controller");
                FreezeGame();
                Debug.Log("FreezeGame invoked");
                //EndTurn();
            }
        }

    }

    public void FreezeGame()
    {
        gameFrozen = true;
    }

    private void ResetTurnStages()
    {
        foreach (GameObject character in GameObject.FindGameObjectsWithTag("Character"))
        {
            character.GetComponent<CharBehaviour>().turnStage=0;
        }
    }

    public void EndTurn()
    {
        BoardTiles.ClearAllTilesImmediate();

/*        for (int j = 0; j < GameObject.FindGameObjectsWithTag("Tile").Length; j++)
        {
            Debug.Log(GameObject.FindGameObjectsWithTag("Tile")[j].transform.position);
        }*/

            if (!playerTurn)
        {
            ResetTurnStages();
            turn++;
            playerTurn = true;
            TurnCounter.GetComponent<Text>().text = $"Turn: {turn}";
            ActiveTeam.GetComponent<Text>().text = "Blue's turn";
        }
        else
        {
            playerTurn = false;
            ActiveTeam.GetComponent<Text>().text = "Red's turn";
        }
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
