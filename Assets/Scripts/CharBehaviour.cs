using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharBehaviour : MonoBehaviour
{
    private BoardTiles BoardTiles;
    private GameController GameController;
    private int m = 9;
    private int r = 2;

    public int hp = 100;
    private int dmg = 10;

    public int turnStage = 0;

    public AudioSource MoveAudioSource;
    public AudioSource HitAudioSource;

    private void Awake()
    {
        BoardTiles = GameObject.Find("Floor").GetComponent<BoardTiles>();
        GameController = GameObject.Find("GameController").GetComponent<GameController>();
    }

    private void Update()
    {
        if (GameController.playerTurn)
        {
            //if (Input.GetKeyDown("s") && turnStage == 0) ShowTiles();
            if (Input.GetKeyDown("m") && turnStage == 0) MoveMe();
            if (Input.GetKeyDown("a") && turnStage <= 1) Attack();
            if (Input.GetKeyDown("w") && turnStage <= 1) Wait();
        }

        if (Input.GetKeyDown("r")) ListAllRoutePermsAsStrings();

    }

    private void ShowTilesOld()
    {
        BoardTiles.ClearAllTiles();
        ShowBlueTilesOld();
        ShowRedTilesOld();
    }

    private void Wait()
    {
        BoardTiles.ClearAllTiles();
        turnStage = 2;
        if (GameController.CheckTurnOver())
        {
            GameController.EndTurn();
        }
    }

    private void Attack()
    {
        //We have a seperate check if in range function
        //It doesn't matter if the enemy is on a blue or red tile
        //you click attack on the tile
        //if there's an enemy on it (loop through enemies and if (x,z) match the hit tile) && it's in range abs(x-x) + abs(z-z) <= r
        //let's stick with the requirement of hitting a tile tag for now for easy extraction of (x,z) plus it's visually more obvious for the player

        Camera cameraComponent = GameObject.Find("Main Camera").GetComponent<Camera>();
        Ray ray = cameraComponent.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100)
            && hit.transform.gameObject.CompareTag("Tile")
            && BoardTiles.CheckForEnemyOnCoord(hit.transform.position) != null
            && CheckInRange(hit.transform.position)
            && !EventSystem.current.IsPointerOverGameObject())
        {

            BoardTiles.CheckForEnemyOnCoord(hit.transform.position).GetComponent<EnemyBehaviour>().hp -= dmg;
            HitAudioSource.Play();
            Wait();

        }
    }

    private bool CheckInRange(Vector3 coord)
    {
        if (Mathf.Abs(coord.x - transform.position.x) + Mathf.Abs(coord.z - transform.position.z) <= r) return true;
        else return false;
    }

    private void MoveMe()
    {
        Camera cameraComponent = GameObject.Find("Main Camera").GetComponent<Camera>();
        Ray ray = cameraComponent.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100)
            && hit.transform.gameObject.name == "BlueTile(Clone)"
            && !EventSystem.current.IsPointerOverGameObject()
            )
        {
            transform.position = hit.transform.position + new Vector3(0, 0.49f, 0);
            MoveAudioSource.Play();
            turnStage = 1;
            ShowTilesOld();
        }
    }

    private void ShowBlueTilesOld()
    {
        Vector3 root = transform.position - new Vector3(0, 0.49f, 0);

        if (turnStage == 0)
        {

            //at each point on the x axis from max moving left to max moving right
            for (int i = -m; i <= m; i++)
            {
                //unused movement at this x is m-abs(i)
                //bottom most blue is down by the amount of unused movement
                //top most blue is up by the amount of unused movement
                //from bottom to top place a blue
                for (int j = -(m - Mathf.Abs(i)); j <= (m - Mathf.Abs(i)); j++)
                {
                    BoardTiles.AddTile("blue", root + new Vector3(i, 0, j));
                }
            }
        }
        else
        {
            BoardTiles.AddTile("blue", root);
        }

    }

    private void ShowRedTilesOld()
    {
        Vector3 root = transform.position - new Vector3(0, 0.49f, 0);

        if (turnStage == 0)
        {

            //straight off and diagonally off the left and right
            //start at the extreme left attackable square -(m+r), end at the attackable square immediately left of the leftmost move square (-m-1)
            for (int i = -(m + r); i < -m; i++)
            {
                //unused range at this x is m-abs(i)+r
                //bottom most red is down by the amount of unused range
                //top most red is up by the amount of unused range
                //from bottom to top place a red
                //The second AddTile is for off the right side hence -i
                for (int j = -(m - Mathf.Abs(i) + r); j <= (m - Mathf.Abs(i) + r); j++)
                {
                    BoardTiles.AddTile("red", root + new Vector3(i, 0, j));
                    BoardTiles.AddTile("red", root + new Vector3(-i, 0, j));
                }
            }

            //above and below
            for (int i = -m; i <= m; i++)
            {
                for (int j = 1; j <= r; j++)
                {
                    BoardTiles.AddTile("red", root + new Vector3(i, 0, -(m - Mathf.Abs(i) + j)));
                    BoardTiles.AddTile("red", root + new Vector3(i, 0, m - Mathf.Abs(i) + j));
                }
            }

        }
        else
        {
            for (int i = -r; i <= r; i++)
            {
                //Same logic as place blues but for r instead of m
                for (int j = -(r - Mathf.Abs(i)); j <= (r - Mathf.Abs(i)); j++)
                {
                    if (i != 0 || j != 0) BoardTiles.AddTile("red", root + new Vector3(i, 0, j));
                }

            }
        }
    }

    private void ShowBlueTiles()
    {
        //Make a list of coordinates that are blue tile candidates

        Vector3 root = transform.position - new Vector3(0, 0.49f, 0);

        List<Vector3> blueCandidates = new List<Vector3>();
        blueCandidates.Add(root);

        //at each point on the x axis from max moving left to max moving right
        for (int i = -m; i <= m; i++)
        {
            //unused movement at this x is m-abs(i)
            //bottom most blue is down by the amount of unused movement
            //top most blue is up by the amount of unused movement
            //from bottom to top are blue candidates
            for (int j = -(m - Mathf.Abs(i)); j <= (m - Mathf.Abs(i)); j++)
            {
                blueCandidates.Add(root + new Vector3(i, 0, j));
            }
        }
    }

    private void ListAllRoutePerms(Vector3 start)
    {
        List<List<Vector3>> routes = new List<List<Vector3>>();

        //loop i from 1 to m
        //for i=1, add the following 4 lists of length 1 to routes:
        //(0,-1), (0,1), (-1, 0), (1, 0)
        //for i=2, add the following 16 lists of length 2 to routes:
        //[(0,-1), (0,-2)], [(0,-1), (0,0)], [(0,-1), (-1,-1)], [(0,-1), (1,-1)]
        //[(0,1), (0,0)], [(0,1), (0,2)], [(0,1), (-1,1)], [(0,1), (1,1)]
        //[(-1,0), (-1,-1)], [(-1,0), (-1,1)], [(-1,0), (-2,0)], [(-1,0), (0,0)]
        //[(1,0), (1,-1)], [(1,0), (1,1)], [(1,0), (0,0)], [(1,0), (2,0)]

        for (int i = 1; i <= m; i++)
        {
            //Instantiate a single route

            List<Vector3> route = new List<Vector3>();

            //Add the route down
            route.Add(start + new Vector3(0, 0, -1));



        }
    }

    private List<string> ListAllRoutePermsAsStrings()
    {
        List<string> routes = new List<string>();

        //Add these to routes
        //D, U, L, R
        //DD, DU, DL, DR
        //UD, UU, UL, UR
        //LD, LU, LL, LR
        //RD, RU, RL, RR
        //...up to length m

        for (int j = 1; j <= Math.Pow(4, m); j++)
        {
            routes.Add("D");
            routes.Add("U");
            routes.Add("L");
            routes.Add("R");

        }
        //Now we have D, U, L, R, D, U, L, R, D, U, L, R, D, U, L, R

        int currIndex = 0;


        //if m=3, we need to do the next step 4^1 times, then 4^0 times
        //this is how many times we're going to repeat the cycle through D,U,L,R
        for (int k = 1; k <= m-1; k++)
        {

        //double times = Math.Pow(4, (m-1-k));

        for (int l = 1; l <= Math.Pow(4, (m - 1 - k)); l++)
        {
            //when k=1, append D 4^1 times, then U 4^1 times...
            //then when k=0, append D 4^2 times, then U 4^2 times
            for (int j = 1; j <= Math.Pow(4, k); j++)
            {
                routes[currIndex] += "D";
                Debug.Log($"currIndex is {currIndex}, just made {routes[currIndex]}");
                currIndex++;
                
            }
            for (int j = 1; j <= Math.Pow(4, k); j++)
            {
                routes[currIndex] += "U";
                Debug.Log($"currIndex is {currIndex}, just made {routes[currIndex]}");
                currIndex++;
            }
            for (int j = 1; j <= Math.Pow(4, k); j++)
            {
                routes[currIndex] += "L";
                Debug.Log($"currIndex is {currIndex}, just made {routes[currIndex]}");
                currIndex++;
            }
            for (int j = 1; j <= Math.Pow(4, k); j++)
            {
                routes[currIndex] += "R";
                Debug.Log($"currIndex is {currIndex}, just made {routes[currIndex]}");
                currIndex++;
            }
        }
        currIndex = 0;


        }


        for (int i = 0; i < Math.Pow(4, m); i++)
        {
            Debug.Log($"iternation {i} is {routes[i]}");
        }

        return routes;



        
    }

    private void extendPathPerms(List<string> perms)
    {

    }

    private void CheckCoordIfBlue(Vector3 start, Vector3 candidate)
    {
        //make a shortlist of routes from start to candidate
    }

}
