using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharBehaviour : MonoBehaviour
{
    private BoardTiles BoardTiles;
    private GameController GameController;
    private int m = 5;
    //private int m_rem = 0;
    private int r = 3;

    public int hp = 100;
    private int dmg = 10;

    public float speed = 1f;
    private Vector3 target;
    //public GameObject MoveTargetTile;
    public bool isMoving = false;


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
        if (isMoving)
        {
            // Move our position a step closer to the target.
            var step = speed * Time.deltaTime;
            //transform.position = Vector3.MoveTowards(transform.position, target, step);
            transform.position = target;

            // Check if the position of the cube and sphere are approximately equal.
            if (Vector3.Distance(transform.position, target) < 0.001f)
            {
                // Put it on the right spot and stop moving
                transform.position = target;
                isMoving = false;
            }
        }
    }

    IEnumerator ExecuteAfterTime(float time, List<Vector3> route, int iteration)
    {
        yield return new WaitForSeconds(time);
        MoveOneTile(route[iteration] + new Vector3(0, 0.49f, 0));
    }

    IEnumerator RedsAfterTime(float time)
    {
        //Place attackable reds after using movement
        yield return new WaitForSeconds(time);
                    for (int i = -r; i <= r; i++)
            {
                for (int j = -(r - Mathf.Abs(i)); j <= (r - Mathf.Abs(i)); j++)
                {
                    if (i != 0 || j != 0) BoardTiles.AddTile("red", transform.position, transform.position - new Vector3(0, 0.49f, 0) + new Vector3(i, 0, j));
                }
            }
    }

    public void MoveMe()
    {
        Camera cameraComponent = GameObject.Find("Main Camera").GetComponent<Camera>();
        Ray ray = cameraComponent.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100)
            && hit.transform.gameObject.name == "BlueTile(Clone)"
            && !EventSystem.current.IsPointerOverGameObject()
            )
        {
            List<Vector3> route = makeRoute(hit.transform.position);
            MoveOneTile(route[route.Count - 1] + new Vector3(0, 0.49f, 0));

            for (int i = route.Count - 2; i >= 0; i--)
            {
                StartCoroutine(ExecuteAfterTime((route.Count - 1 - i) / 4f, route, i));
            }
            StartCoroutine(RedsAfterTime(route.Count / 4f));
            MoveAudioSource.Play();
            turnStage = 1;

            BoardTiles.ClearAllTiles();
        }
    }

    private void MoveOneTile(Vector3 coord)
    {
        target = coord;
        isMoving = true;
    }

    private List<Vector3> makeRoute(Vector3 endCoord)
    {
        List<Vector3> route = new List<Vector3>();
        route.Add(endCoord);

        Vector3 prevCoord = BoardTiles.CheckForObjectOnCoord(endCoord, "Tile").GetComponent<TileDirections>().prevCoord;
        route.Add(prevCoord);

        do
        {
            prevCoord = BoardTiles.CheckForObjectOnCoord(prevCoord, "Tile").GetComponent<TileDirections>().prevCoord;
            route.Add(prevCoord);
        }
        while (prevCoord.x != transform.position.x || prevCoord.z != transform.position.z);

        return route;
    }

    public void Wait()
    {
        BoardTiles.ClearAllTiles();
        turnStage = 2;
        if (GameController.CheckTurnOver())
        {
            GameController.EndTurn();
        }
    }

    public void Attack()
    {
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

    public void ShowTiles()
    {
        BoardTiles.ClearAllTilesImmediate();
        Vector3 root = transform.position - new Vector3(0, 0.49f, 0);
        BoardTiles.AddTile("blue", root, root);

        int m_rem = m;

        if (turnStage > 0) m_rem = 0;

        for (int i = 1; i <= (m_rem + r); i++)
        {
            foreach (GameObject tile in GameObject.FindGameObjectsWithTag("Tile"))
            {
                //Check if the tile directly above has no obstacle and no tile
                //If it's empty then place a blue on it if i<=m, else a red
                CheckAndPlace(tile.transform.position, new Vector3(0, 0, 1), i);
                CheckAndPlace(tile.transform.position, new Vector3(0, 0, -1), i);
                CheckAndPlace(tile.transform.position, new Vector3(-1, 0, 0), i);
                CheckAndPlace(tile.transform.position, new Vector3(1, 0, 0), i);

            }
        }
    }

    private void CheckAndPlace(Vector3 prevCoord, Vector3 translation, int iteration)
    {
        //On iterations 1 to m, check !obstacle, !enemy, !tile
        //On iterations m+1 to m+r, just check !tile
        //Only place blues before iteration m
        //Only place reds after iteration m
        if (BoardTiles.CheckForObjectOnCoord(prevCoord + translation, "Tile") == null && turnStage < 2)
        {
            if (iteration <= m && BoardTiles.CheckForObjectOnCoord(prevCoord + translation, "Obstacle") == null && BoardTiles.CheckForObjectOnCoord(prevCoord + translation, "Enemy") == null) BoardTiles.AddTile("blue", prevCoord, prevCoord + translation);
            else if (iteration > m) BoardTiles.AddTile("red", prevCoord, prevCoord + translation);
        };
    }


    //Keep these 3 methods around for a while, they work just not needed right now.
    /*    private void ShowBlueTilesOld()
        {
            Vector3 root = transform.position - new Vector3(0, 0.49f, 0);
            if (turnStage == 0)
            {
                for (int i = -m; i <= m; i++)
                {
                    for (int j = -(m - Mathf.Abs(i)); j <= (m - Mathf.Abs(i)); j++)
                    {
                        BoardTiles.AddTile("blue", root + new Vector3(i, 0, j));
                    }
                }
            }
            else BoardTiles.AddTile("blue", root);
        }*/
    /*    private void ShowRedTilesOld()
        {
            Vector3 root = transform.position - new Vector3(0, 0.49f, 0);

            if (turnStage == 0)
            {
                for (int i = -(m + r); i < -m; i++)
                {
                    for (int j = -(m - Mathf.Abs(i) + r); j <= (m - Mathf.Abs(i) + r); j++)
                    {
                        BoardTiles.AddTile("red", root + new Vector3(i, 0, j));
                        BoardTiles.AddTile("red", root + new Vector3(-i, 0, j));
                    }
                }
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
                    for (int j = -(r - Mathf.Abs(i)); j <= (r - Mathf.Abs(i)); j++)
                    {
                        if (i != 0 || j != 0) BoardTiles.AddTile("red", root + new Vector3(i, 0, j));
                    }
                }
            }
        }*/
    /*    private List<string> ListAllRoutePermsAsStrings()
        {
            List<string> routes = new List<string>();
            for (int j = 1; j <= Math.Pow(4, m); j++)
            {
                routes.Add("D");
                routes.Add("U");
                routes.Add("L");
                routes.Add("R");
            }
            int currIndex = 0;
            for (int k = 1; k <= m-1; k++)
            {
            for (int l = 1; l <= Math.Pow(4, (m - 1 - k)); l++)
            {
                for (int j = 1; j <= Math.Pow(4, k); j++)
                {
                    routes[currIndex] += "D";
                    currIndex++;  
                }
                for (int j = 1; j <= Math.Pow(4, k); j++)
                {
                    routes[currIndex] += "U";
                    currIndex++;
                }
                for (int j = 1; j <= Math.Pow(4, k); j++)
                {
                    routes[currIndex] += "L";
                    currIndex++;
                }
                for (int j = 1; j <= Math.Pow(4, k); j++)
                {
                    routes[currIndex] += "R";
                    currIndex++;
                }
            }
            currIndex = 0;
            }
            return routes;
        }*/

}
