using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource audioSource;
    public AudioClip startMusicClip;
    AudioClip currentClip;

    float baseVolume;
    float currentVolume;
    [SerializeField]
    float m_volumeMultiplier = 0.3f;
    public float volumeMultiplier
    {
        get { return m_volumeMultiplier; }

        set
        {
            m_volumeMultiplier = value;
            audioSource.volume = baseVolume * m_volumeMultiplier;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        PlayMusic(startMusicClip);
    }

    public void PlayMusic(AudioClip music, float volume = 1)
    {
        if (music != currentClip)
        {
            audioSource.clip = music;
            audioSource.volume = volume * volumeMultiplier;
            audioSource.Play();
            audioSource.loop = true;
        }

        baseVolume = volume;
        currentClip = music;
    }

    public void ChangeVolume(float newVolumeMultiplier)
    {
        volumeMultiplier = newVolumeMultiplier;

        currentVolume = volumeMultiplier * baseVolume;

        audioSource.volume = currentVolume;
    }

    public void PlaySoundEffect(AudioClip soundEffect, float volume = 1f)
    {
        if (audioSource != null && soundEffect != null)
        {
            audioSource.PlayOneShot(soundEffect, volume);
        }
    }
}