using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingDummy : MonoBehaviour
{
    public int hp = 20;

    private AudioSource DeathAudioSource;

    void Awake()
    {
        DeathAudioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (hp<=0) Die();
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
