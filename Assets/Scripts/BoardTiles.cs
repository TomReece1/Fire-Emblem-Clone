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
    //Squeeeeeepz waz ere 2022
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

    public GameObject CheckForObjectOnCoord(Vector3 coord, string objTag)
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag(objTag))
        {
            if (obj.transform.position.x == coord.x && obj.transform.position.z == coord.z) return obj;
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

    public void ClearAllTilesImmediate()
    {
        foreach (GameObject tile in GameObject.FindGameObjectsWithTag("Tile"))
        {
            DestroyImmediate(tile);
        }
    }

    public void AddTile(string colour, Vector3 prevCoord, Vector3 coord)
    {
        if (coord.x >= 0 && coord.z >= 0 && coord.x <= 9 && coord.z <= 9)
        {
            GameObject tile;

            if (colour == "blue") tile = Instantiate(BlueTilePrefab);
            else if (colour == "red") tile = Instantiate(RedTilePrefab);
            else tile = Instantiate(WhiteTilePrefab);

            tile.transform.position = coord;
            tile.GetComponent<TileDirections>().prevCoord = prevCoord;
        }
    }

}
