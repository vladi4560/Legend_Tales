using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioClip gameSound, sfxSoundAttack, sfxSoundDeath;
    public AudioSource musicSource, sfxSource;

    public GameObject toggleOffImg;
    public GameObject toggleOnImg;
    public Button SoundButton;

    private void Awake()
    {
        // Check if an instance already exists
        if (instance != null && instance != this)
        {
            // Destroy this duplicate instance if found
            Destroy(gameObject);
            return;
        }

        // Set the singleton instance
        instance = this;

        // Keep the singleton object alive between scene loads
        DontDestroyOnLoad(gameObject);
        SoundButton.onClick.AddListener(ToggleMusic);
    }
    public void StopGameSound()
    {
        musicSource.Stop();
    }
    // Play a sound effect
    public void PlaySoundEffectAttack()
    {
        if (sfxSoundAttack == null)
        {
            Debug.Log("Sound Not Working");
            return;
        }
        sfxSource.PlayOneShot(sfxSoundAttack);
    }
    public void PlaySoundEffectDeath()
    {
        if (sfxSoundDeath == null)
        {
            Debug.Log("Sound Not Working");
            return;
        }
        sfxSource.PlayOneShot(sfxSoundDeath);
    }

    public void PlayGameSound()
    {
        if (gameSound == null)
        {
            Debug.Log("Sound Not Working");
            return;
        }
        musicSource.clip = gameSound;
        musicSource.Play();
    }

    public void ToggleMusic()
    {
        musicSource.mute = !musicSource.mute;
        if (musicSource.mute)
        {
            toggleOnImg.SetActive(false);
            toggleOffImg.SetActive(true);
        }
        else
        {
            toggleOnImg.SetActive(true);
            toggleOffImg.SetActive(false);
        }
    }
}
