using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class game_controler : MonoBehaviour
{
    public AudioSource audioSource;

    public void play_audio(string nome_audio)
    {
        AudioClip clip = Resources.Load<AudioClip>(nome_audio);
        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
        else
        {
            Debug.LogError("Áudio não encontrado: " + nome_audio);
        }
    }
}
