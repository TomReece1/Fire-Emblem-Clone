using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class GameController : MonoBehaviour
{
    private Roster Roster;
    private BoardTiles BoardTiles;
    private CharBehaviour CharBehaviour;
    private Camera cameraComponent;
    public GameObject TurnCounter;
    public GameObject ActiveTeam;
    private GameObject ActionButtonsPanel;
    private GameObject MoveButton;
    private GameObject AttackButton;
    private GameObject SpecialButton;
    private GameObject SpecialButtonText;
    private GameObject WaitButton;
    private GameObject BackButton;
    private PanelOpener PanelOpener;
    private GameObject selectedUnit;

    public int turn = 1;
    public bool playerTurn = true;
    public bool gameFrozen = false;

    public GameObject BlueUnitPrefab;
    public GameObject BlueSwordUnitPrefab;
    public GameObject BlueLanceUnitPrefab;
    public GameObject BlueAxeUnitPrefab;

    private List<Vector3> SpawnVectors = new List<Vector3>();

    private Dictionary<string, GameObject> Classes = new Dictionary<string, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        BoardTiles = GameObject.Find("Floor").GetComponent<BoardTiles>();
        Roster = GameObject.Find("MainManager").GetComponent<Roster>();
        PanelOpener = GameObject.Find("StatsPanel").GetComponent<PanelOpener>();
        ActionButtonsPanel = GameObject.Find("ActionButtonsPanel");
        MoveButton = GameObject.Find("MoveButton");
        AttackButton = GameObject.Find("AttackButton");
        SpecialButton = GameObject.Find("SpecialButton");
        SpecialButtonText = GameObject.Find("SpecialButtonText");
        WaitButton = GameObject.Find("WaitButton");
        BackButton = GameObject.Find("BackButton");
        ActionButtonsPanel.SetActive(false);

        SpawnVectors.Add(new Vector3(1, 0.5f, 1));
        SpawnVectors.Add(new Vector3(2, 0.5f, 3));
        SpawnVectors.Add(new Vector3(1, 0.5f, 7));

        Classes.Add("sword", BlueSwordUnitPrefab);
        Classes.Add("lance", BlueLanceUnitPrefab);
        Classes.Add("axe", BlueAxeUnitPrefab);

        for (int i = 0; i < SpawnVectors.Count; i++)
        {
            GameObject unitGO;
            var unit = Roster.unitList[i];
            unitGO = Instantiate(Classes[unit.weapon]);
            unitGO.GetComponent<CharBehaviour>().Init(unit.unitName);
            unitGO.transform.position = SpawnVectors[i];
        }
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
                        //then update stats panel
                        if (
                            (selectedUnit == null)
                            ||
                            (BoardTiles.CheckForObjectOnCoord(RoundedHitCoord, "Character")
                            && (RoundedHitCoord.x != selectedUnit.transform.position.x || RoundedHitCoord.z != selectedUnit.transform.position.z)
                            && (CharBehaviour.turnStage != 3 && CharBehaviour.turnStage != 4)
                            ))
                        {
                            
                            selectedUnit = BoardTiles.CheckForObjectOnCoord(RoundedHitCoord, "Character");
                            CharBehaviour = selectedUnit.GetComponent<CharBehaviour>();
                            if (CharBehaviour.turnStage != 5)
                            {
                                CharBehaviour.turnStage = 1;
                                CharBehaviour.ShowTiles();
                                PanelOpener.UpdateStats(CharBehaviour);
                                DisplayActions();
                            }
                        }
                        else if (
                            selectedUnit != null && CharBehaviour.turnStage == 1
                            && BoardTiles.CheckForObjectOnCoord(RoundedHitCoord, "Tile").name == "BlueTile(Clone)"
                            )
                        {
                            CharBehaviour.MoveMe();
                            MoveButton.SetActive(false);
                        }
                        else if (selectedUnit != null && CharBehaviour.turnStage <= 3
                            && RoundedHitCoord.x == selectedUnit.transform.position.x && RoundedHitCoord.z == selectedUnit.transform.position.z
                            ) CharBehaviour.Wait();
                        else if (selectedUnit != null && CharBehaviour.turnStage <= 3) CharBehaviour.Attack();
                        else if (selectedUnit != null && CharBehaviour.turnStage == 4 && CharBehaviour.specialSelected) CharBehaviour.Special();
                        else if (selectedUnit != null && CharBehaviour.turnStage == 4 && !CharBehaviour.specialSelected) CharBehaviour.Attack();
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

    public void SelectMove()
    {
        MoveButton.SetActive(false);
    }

    public void SelectAttack()
    {
        CharBehaviour.specialSelected = false;
        CharBehaviour.turnStage = 4;
        MoveButton.SetActive(false);
        AttackButton.SetActive(false);
        SpecialButton.SetActive(false);
    }

    public void SelectSpecial()
    {
        CharBehaviour.specialSelected = true;
        CharBehaviour.turnStage = 4;
        MoveButton.SetActive(false);
        AttackButton.SetActive(false);
        SpecialButton.SetActive(false);
    }

    public void SelectWait()
    {
        CharBehaviour.Wait();
    }

    public void SelectBack()
    {
        CharBehaviour.turnStage = 1;
        DisplayActions();
        selectedUnit.transform.position = CharBehaviour.originalCoord;
        BoardTiles.ClearAllTiles();
        CharBehaviour.ShowTiles();
    }

    private void DisplayActions()
    {
        SpecialButtonText.GetComponent<TextMeshProUGUI>().text = $"{CharBehaviour.GetSpecialLabel()}";
        MoveButton.SetActive(true);
        AttackButton.SetActive(true);
        SpecialButton.SetActive(true);
        ActionButtonsPanel.SetActive(true);
    }

    public void HideAllActions()
    {
        MoveButton.SetActive(true);
        AttackButton.SetActive(true);
        SpecialButton.SetActive(true);
        ActionButtonsPanel.SetActive(false);
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
            HideAllActions();
        }
    }
            
    public bool CheckTurnOver()
    {
        foreach (GameObject character in GameObject.FindGameObjectsWithTag("Character"))
        {
            if (character.GetComponent<CharBehaviour>().turnStage < 5) return false;
        }
        return true;
    }

}
