using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip audioJump;
    public AudioClip audioAttack;
    public AudioClip audioDamaged;
    public AudioClip audioItem;
    public AudioClip audioDie;
    public AudioClip audioFinish;

    AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(string action)
    {
        switch (action)
        {
            case "JUMP":
                audioSource.clip = audioJump;
                audioSource.Play();
                break;

            case "ATTACK":
                audioSource.clip = audioAttack;
                audioSource.Play();
                break;

            case "DAMAGED":
                audioSource.clip = audioDamaged;
                audioSource.Play();
                break;

            case "ITEM":
                audioSource.clip = audioItem;
                audioSource.Play();
                break;

            case "DIE":
                audioSource.clip = audioDie;
                audioSource.Play();
                break;

            case "FINISH":
                audioSource.clip = audioFinish;
                audioSource.Play();
                break;
        }
    }
}
