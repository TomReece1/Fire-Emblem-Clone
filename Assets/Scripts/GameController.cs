using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    private BoardTiles BoardTiles;
    private CharBehaviour CharBehaviour;
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

                    //if you clicked the floor
                    if (Physics.Raycast(ray, out hit, 100)
                        && !EventSystem.current.IsPointerOverGameObject()
                        )
                    {
                        Vector3 RoundedHitCoord = new Vector3(Mathf.RoundToInt(hit.point.x), Mathf.RoundToInt(hit.point.y), Mathf.RoundToInt(hit.point.z));
                        //If you clicked a unit, set selectedUnit and CharBehaviour
                        if (BoardTiles.CheckForObjectOnCoord(RoundedHitCoord, "Character"))
                        {
                            selectedUnit = BoardTiles.CheckForObjectOnCoord(RoundedHitCoord, "Character");
                            CharBehaviour = selectedUnit.GetComponent<CharBehaviour>();
                        }
                        //This cannot be refactored to selectedUnity because we need to check if this specific click was on a character's square.
                        if (BoardTiles.CheckForObjectOnCoord(RoundedHitCoord, "Character") != null && CharBehaviour.turnStage == 0) CharBehaviour.ShowTiles();
                        else if (selectedUnit != null && CharBehaviour.turnStage == 0) CharBehaviour.MoveMe();
                        else if (selectedUnit != null && CharBehaviour.turnStage <= 1) CharBehaviour.Attack();
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
