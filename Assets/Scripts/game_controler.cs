using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class game_controler : MonoBehaviour
{
    public AudioSource audioSource;

    public void play_audio(AudioClip audio)
    {
        audioSource.clip = audio;
        audioSource.Play();
    }
}
