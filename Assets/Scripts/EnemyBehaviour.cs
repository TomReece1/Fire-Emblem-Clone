using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    private GameController GameController;

    public int hp = 20;
    private int dmg = 5;
    private int m = 4;
    private int r = 2;

    private AudioSource DeathAudioSource;

    void Awake()
    {
        DeathAudioSource = GetComponent<AudioSource>();
        GameController = GameObject.Find("GameController").GetComponent<GameController>();
    }

    void Update()
    {
        if (hp<=0) Die();



    }

    public void MakeMove()
    {
        GameObject target = FindClosestChar();
        if (target != null)
        {
            MoveToChar(target);
            AttackChar(target);
        }
    }

    private GameObject FindClosestChar()
    {
        //loop through all chars
        //check if the distance is <= m+r
        //if it is then store that as temporary closest
        //if another is < temporary closest then that becomes temporary closest
        //after loop return temporary closest

        //or loop through all seeable squares and check if there's a character on them
        //no need for a temp closest because you'll just return the first one you find

        //or doesn't even have to be the closest for simplicity

        foreach (GameObject character in GameObject.FindGameObjectsWithTag("Character"))
        {
            if (Mathf.Abs(character.transform.position.x - transform.position.x) + Mathf.Abs(character.transform.position.z - transform.position.z) <= m+r) return character;
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
