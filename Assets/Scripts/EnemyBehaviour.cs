using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    private GameController GameController;
    private BoardTiles BoardTiles;

    public int hp = 20;
    private int dmg = 5;
    private int m = 5;
    private int r = 2;

    private AudioSource DeathAudioSource;

    void Awake()
    {
        DeathAudioSource = GetComponent<AudioSource>();
        GameController = GameObject.Find("GameController").GetComponent<GameController>();
        BoardTiles = GameObject.Find("Floor").GetComponent<BoardTiles>();
    }

    void Update()
    {
        if (hp<=0) Die();
    }

    public void MakeMove()
    {

        ShowTiles();

        List<GameObject> list = new List<GameObject>();

        foreach (GameObject tile in GameObject.FindGameObjectsWithTag("Tile"))
        {
            GameObject target = IsThereAChar(tile.transform.position);
            GameObject standTile = IsThereAnEmptyBlueInRange(tile.transform.position);

            if (target != null)
            {
                if (standTile != null)
                {
                    Debug.Log($"Found attackable char!!!! target: {target.transform.position} standTile: {standTile.transform.position}");
                    
                    list.Add(target);
                    list.Add(standTile);
                }
            }
        }

        if (list[0] != null && list[1] != null)
        {
            transform.position = list[1].transform.position + new Vector3(0,0.49f,0);
            AttackChar(list[0]);
        }
    }

    private List<GameObject> FindAttackableChar()
    {
        //loop through all chars and check if they're stood on a tile
        //(if they're stood on a tile there will be a tile to stand on unless another enemy is on it)

        //or loop through all tiles and check if there's a character on them
        //(if there is a character on there, there will be a tile to stand on unless another enemy is on it) 

        //in our check if a char is attackable
        //we should discount if not on a tile
        //then discount if no blue square to stand on

        ShowTiles();



        foreach (GameObject tile in GameObject.FindGameObjectsWithTag("Tile"))
        {
            GameObject target = IsThereAChar(tile.transform.position);
            //Debug.Log($"target is {target}");

            GameObject standTile = IsThereAnEmptyBlueInRange(tile.transform.position);
            //Debug.Log($"standTile is {standTile}");


            if (target != null)
            {
                if (standTile != null)
                {
                    Debug.Log($"Found attackable char!!!! target: {target.transform.position} standTile: {standTile.transform.position}");
                    List<GameObject> list = new List<GameObject>();
                    list.Add(target);
                    list.Add(standTile);
                    return list;
                }
            }
            else
            {
                Debug.Log($"No attackble char here... target: {target} standTile: {standTile}");
            }
        }
        return null;
    }

    private void ShowTiles()
    {
        Debug.Log("ShowTiles started");

        BoardTiles.ClearAllTilesImmediate();

        Vector3 root = transform.position - new Vector3(0, 0.49f, 0);
        BoardTiles.AddTile("blue", root);

        int m_rem = m;

        //if (turnStage > 0) m_rem = 0;

        for (int i = 1; i <= (m_rem + r); i++)
        {


            

            foreach (GameObject tile in GameObject.FindGameObjectsWithTag("Tile"))
            {

                //Check if the tile directly above has no obstacle and no tile
                //If it's empty then place a blue on it if i<=m, else a red
                CheckAndPlace(tile.transform.position + new Vector3(0, 0, 1), i);
                CheckAndPlace(tile.transform.position + new Vector3(0, 0, -1), i);

                CheckAndPlace(tile.transform.position + new Vector3(-1, 0, 0), i);
                CheckAndPlace(tile.transform.position + new Vector3(1, 0, 0), i);

            }
        }

    }

    private void CheckAndPlace(Vector3 coord, int iteration)
    {
        //Debug.Log("CheckAndPlace started");
        //On iterations 1 to m, check !obstacle, !enemy, !tile
        //On iterations m+1 to m+r, just check !tile
        //Only place blues before iteration m
        //Only place reds after iteration m
        if (BoardTiles.CheckForObjectOnCoord(coord, "Tile") == null)
        {
            if (iteration <= m && BoardTiles.CheckForObjectOnCoord(coord, "Obstacle") == null && BoardTiles.CheckForObjectOnCoord(coord, "Character") == null) BoardTiles.AddTile("blue", coord);
            else if (iteration > m) BoardTiles.AddTile("red", coord);
        };
    }

    private GameObject IsThereAChar(Vector3 coord)
    {

            GameObject targetCharacter = BoardTiles.CheckForObjectOnCoord(coord, "Character");

                        if (targetCharacter != null)
                        {
                            return targetCharacter;
                        }
        
        return null;
    }

    private GameObject IsThereAnEmptyBlueInRange(Vector3 coord)
    {

        Debug.Log("start IsThereAnEmptyBlueInRange()");

        //BoardTiles.ClearAllTiles();
        //Make a list of the coordinates in range of the target
        List<Vector3> inRangeCoords = new List<Vector3>();
        for (int i = -r; i <= r; i++)
        {
            for (int j = -(r - Mathf.Abs(i)); j <= (r - Mathf.Abs(i)); j++)
            {
                if (i != 0 || j != 0) inRangeCoords.Add(coord + new Vector3(i, 0, j));
                    //BoardTiles.AddTile("red", coord + new Vector3(i, 0, j));
            }
        }

        //is there an empty blue tile in range of the target
/*        BoardTiles.ClearAllTiles();
        BoardTiles.AddTile("blue", coord);
        int m_rem = 0;
        for (int i = 1; i <= (m_rem + r); i++)
        {
            foreach (GameObject tile in GameObject.FindGameObjectsWithTag("Tile"))
            {
                CheckAndPlace(tile.transform.position + new Vector3(0, 0, 1), i);
                CheckAndPlace(tile.transform.position + new Vector3(0, 0, -1), i);
                CheckAndPlace(tile.transform.position + new Vector3(-1, 0, 0), i);
                CheckAndPlace(tile.transform.position + new Vector3(1, 0, 0), i);

            }
        }*/

        //now the in range standable tiles are showing as red
        //loop through them and as soon as you find one that's empty, return it

        //foreach (GameObject tile in GameObject.FindGameObjectsWithTag("Tile"))
        foreach (Vector3 inRangeCoord in inRangeCoords)
        {
            if (BoardTiles.CheckForObjectOnCoord(inRangeCoord, "Character") == null
                && BoardTiles.CheckForObjectOnCoord(inRangeCoord, "Obstacle") == null
                && BoardTiles.CheckForObjectOnCoord(inRangeCoord, "Enemy") == null)
            {


                return BoardTiles.CheckForObjectOnCoord(inRangeCoord, "Tile");
            }
        }
        return null;

    }

    private void MoveToChar(GameObject character)
    {
        //if directly above then stand below
        if (character.transform.position.z > transform.position.z && character.transform.position.x == transform.position.x)
        {
            transform.position = character.transform.position + new Vector3(0, 0, -1);
        }
        //if directly below then stand above
        if (character.transform.position.z < transform.position.z && character.transform.position.x == transform.position.x)
        {
            transform.position = character.transform.position + new Vector3(0, 0, 1);
        }
        //if at all right then stand left
        if (character.transform.position.x > transform.position.x)
        {
            transform.position = character.transform.position + new Vector3(-1, 0, 0);
        }
        //if at all left then stand right
        if (character.transform.position.x < transform.position.x)
        {
            transform.position = character.transform.position + new Vector3(1, 0, 0);
        }

    }

    private void AttackChar(GameObject character)
    {
        character.GetComponent<CharBehaviour>().hp -= dmg;
    }


    public void Die()
    {
        //DeathAudioSource.Play();
        StartCoroutine(ExecuteAfterTime(0.2f));
        //Destroy(gameObject);
    }

    IEnumerator ExecuteAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }

}
