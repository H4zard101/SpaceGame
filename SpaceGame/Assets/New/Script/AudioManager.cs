using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance; // singleton for easy access
    [SerializeField] private AudioSource audioSource; // the source for sound
    [SerializeField] private AudioClip hoverSoundClip; // sound clip for hovering over button

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    public void PlayerHoverSound()
    {
        if (audioSource != null) 
        {
            audioSource.PlayOneShot(hoverSoundClip);
        }
    }
}
