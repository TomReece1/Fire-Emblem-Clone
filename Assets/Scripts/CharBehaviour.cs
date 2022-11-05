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
    private int m = 5;
    private int r = 3;

    private int hp = 100;
    private int dmg = 10;

    public AudioSource MoveAudioSource;
    public AudioSource HitAudioSource;

    private void Awake()
    {
        BoardTiles = GameObject.Find("Floor").GetComponent<BoardTiles>();
    }

    private void Update()
    {
        if (Input.GetKeyDown("s"))
        {
            BoardTiles.ClearAllTiles();
            ShowBlueTiles();
            ShowRedTiles();
        }

        if (Input.GetKeyDown("m")) MoveMe();
        if (Input.GetKeyDown("a")) Attack();
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

            BoardTiles.CheckForEnemyOnCoord(hit.transform.position).GetComponent<TrainingDummy>().hp -= dmg;
            BoardTiles.ClearAllTiles();
            HitAudioSource.Play();
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
            && !EventSystem.current.IsPointerOverGameObject())
        {
            transform.position = hit.transform.position + new Vector3(0, 0.49f, 0);
            BoardTiles.ClearAllTiles();
            MoveAudioSource.Play();
        }
    }

    private void ShowBlueTiles()
    {
        Vector3 root = transform.position - new Vector3(0, 0.49f, 0);

        //at each point on the x axis from max moving left to max moving right
        for (int i = -m; i <= m; i++)
        {
            //unused movement at this x is m-abs(i)
            //bottom most blue is down by the amount of unused movement
            //top most blue is up by the amount of unused movement
            //from bottom to top place a blue
            for (int j = -(m-Mathf.Abs(i)); j <= (m - Mathf.Abs(i)); j++)
            {
                BoardTiles.AddTile("blue", root + new Vector3(i, 0, j));
            }
        }

    }

    private void ShowRedTiles()
    {
        Vector3 root = transform.position - new Vector3(0, 0.49f, 0);

        //straight off and diagonally off the left and right
        //start at the extreme left attackable square -(m+r), end at the attackable square immediately left of the leftmost move square (-m-1)
        for (int i = -(m+r); i < -m; i++)
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

}