using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardTiles : MonoBehaviour
{
    public GameObject BlueTilePrefab;
    public GameObject RedTilePrefab;
    public GameObject WhiteTilePrefab;
    public GameObject ClearTilePrefab;

    //SloppyChief waz ere 2022
    //Methods:
    //Highlight hovered tile

    public GameObject CheckForEnemyOnCoord(Vector3 coord)
    {
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            if (enemy.transform.position.x == coord.x && enemy.transform.position.z == coord.z) return enemy;
        }
        return null;
    }

    public void ClearAllTiles()
    {
        foreach (GameObject tile in GameObject.FindGameObjectsWithTag("Tile"))
        {
            Destroy(tile);
        }
    }

    public void AddTile(string colour, Vector3 coord)
    {
        if (coord.x >= 0 && coord.z >= 0 && coord.x <= 9 && coord.z <= 9)
        {
            if (colour == "blue") Instantiate(BlueTilePrefab).transform.position = coord;
            else if (colour == "red") Instantiate(RedTilePrefab).transform.position = coord;
            else if (colour == "white") Instantiate(WhiteTilePrefab).transform.position = coord;
        }
    }

}
