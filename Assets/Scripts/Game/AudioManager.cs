using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoSingleton<AudioManager>
{
    public AudioSource audioSource;
    
    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }
    
    public void PlayAudionClip(AudioClip clip)
    {
        if (audioSource != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }
}
