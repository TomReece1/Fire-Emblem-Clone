using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    private BoardTiles BoardTiles;
    //private CharBehaviour CharBehaviour;
    private Camera cameraComponent;
    //private EnemyBehaviour EnemyBehaviour;
    public GameObject TurnCounter;
    public GameObject ActiveTeam;

    private GameObject selectedUnit;

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
            if (playerTurn)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    cameraComponent = GameObject.Find("Main Camera").GetComponent<Camera>();
                    Ray ray = cameraComponent.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    //Vector3Int gridCoord;

                    //if you clicked the floor
                    if (Physics.Raycast(ray, out hit, 100)
                        && !EventSystem.current.IsPointerOverGameObject()
                        )
                    {
                        Debug.Log("You clicked something");
                        //check if the rounded vector you clicked has a blue unit on it
                        //gridCoord = Vector3Int.RoundToInt(hit.point);
                        Vector3 RoundedHitCoord = new Vector3(Mathf.RoundToInt(hit.point.x), Mathf.RoundToInt(hit.point.y), Mathf.RoundToInt(hit.point.z));
                        Debug.Log($"Tile clicked has coords: {RoundedHitCoord}");
                        selectedUnit = BoardTiles.CheckForObjectOnCoord(RoundedHitCoord, "Character");
                        if (selectedUnit != null && selectedUnit.GetComponent<CharBehaviour>().turnStage == 0) selectedUnit.GetComponent<CharBehaviour>().ShowTiles();
                    }
                }


/*                    if (Input.GetKeyDown("s") && turnStage == 0) ShowTiles();
                    
                if (Input.GetKeyDown("m") && turnStage == 0) MoveMe();
                if (Input.GetKeyDown("a") && turnStage <= 1) Attack();
                if (Input.GetKeyDown("w") && turnStage <= 1) Wait();*/

                if (Input.GetKeyDown("e")) EndTurn();
            }

            

            if (!playerTurn)
            {
                foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
                {
                    enemy.GetComponent<EnemyBehaviour>().MakeMove();
                }
                EndTurn();
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
