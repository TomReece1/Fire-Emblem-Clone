using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    private BoardTiles BoardTiles;
    private CharBehaviour CharBehaviour;
    private Camera cameraComponent;
    public GameObject TurnCounter;
    public GameObject ActiveTeam;

    private GameObject selectedUnit;

    public int turn = 1;
    public bool playerTurn = true;

    public bool gameFrozen = false;

    public GameObject BlueUnitPrefab;
    public GameObject BlueSwordUnitPrefab;
    public GameObject BlueLanceUnitPrefab;
    public GameObject BlueAxeUnitPrefab;

    // Start is called before the first frame update
    void Awake()
    {
        BoardTiles = GameObject.Find("Floor").GetComponent<BoardTiles>(); 
        GameObject unitGO;

        unitGO = Instantiate(BlueSwordUnitPrefab);
        unitGO.GetComponent<SwordBehaviour>().Init(7, 2, 30, 9);
        unitGO.transform.position = new Vector3(1, 0.5f, 1);

        unitGO = Instantiate(BlueLanceUnitPrefab);
        unitGO.GetComponent<LanceBehaviour>().Init(8, 1, 40, 8);
        unitGO.transform.position = new Vector3(0, 0.5f, 4);

        unitGO = Instantiate(BlueAxeUnitPrefab);
        unitGO.GetComponent<AxeBehaviour>().Init(6, 1, 50, 10);
        unitGO.transform.position = new Vector3(2, 0.5f, 6);

    }

    // Update is called once per frame
    void Update()
    {
        if (!gameFrozen)
        {
            if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0) SceneManager.LoadScene(1);

            if (playerTurn)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    cameraComponent = GameObject.Find("Main Camera").GetComponent<Camera>();
                    Ray ray = cameraComponent.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit, 100)
                        && !EventSystem.current.IsPointerOverGameObject()
                        )
                    {
                        Vector3 RoundedHitCoord = new Vector3(Mathf.RoundToInt(hit.point.x), Mathf.RoundToInt(hit.point.y), Mathf.RoundToInt(hit.point.z));
                        //If you clicked a unit
                        //that was either the first unit click of the turn
                        //or this click was different than the current selectedUnit
                        //then showtiles for it and set selectedUnit and CharBehaviour
                        if (
                            (selectedUnit == null)
                            ||
                            (BoardTiles.CheckForObjectOnCoord(RoundedHitCoord, "Character")

                            && (RoundedHitCoord.x != selectedUnit.transform.position.x || RoundedHitCoord.z != selectedUnit.transform.position.z)
                            ))
                        {
                            Debug.Log("You tried to reselect a different unit");
                            selectedUnit = BoardTiles.CheckForObjectOnCoord(RoundedHitCoord, "Character");
                            Debug.Log($"You selected a unit {selectedUnit}");

                            if (selectedUnit.name == "CharacterSword(Clone)")
                            {
                                Debug.Log("Unit is a sword");
                                CharBehaviour = selectedUnit.GetComponent<SwordBehaviour>();
                            }
                            else if (selectedUnit.name == "CharacterLance(Clone)")
                            {
                                Debug.Log("Unit is a lance");
                                CharBehaviour = selectedUnit.GetComponent<LanceBehaviour>();
                            }
                            else if (selectedUnit.name == "CharacterAxe(Clone)")
                            {
                                Debug.Log("Unit is a axe");
                                CharBehaviour = selectedUnit.GetComponent<AxeBehaviour>();
                            }

                            Debug.Log($"You've got it's script {CharBehaviour}");
                            CharBehaviour.ShowTiles();
                            Debug.Log("ShowTiles finished");
                        }
                        else if (
                            selectedUnit != null && CharBehaviour.turnStage == 0
                            && BoardTiles.CheckForObjectOnCoord(RoundedHitCoord, "Tile").name == "BlueTile(Clone)"
                            )
                        {
                            Debug.Log("You tried to move the unit");
                            CharBehaviour.MoveMe();
                        }
                        else if (selectedUnit != null && CharBehaviour.turnStage <= 1 && RoundedHitCoord.x == selectedUnit.transform.position.x && RoundedHitCoord.z == selectedUnit.transform.position.z) CharBehaviour.Wait();
                        else if (selectedUnit != null && CharBehaviour.turnStage <= 1) CharBehaviour.Attack();
                    }
                }

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

    public void SpawnUnit()
    {

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
            selectedUnit = null;
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
