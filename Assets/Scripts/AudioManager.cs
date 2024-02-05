using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource[] audioSources;
    public int currentSource = 1;
    public AudioClip startMusicClip, combatMusicClip, combatVictorySound;
    AudioClip currentClip;

    public float baseVolume = 0.8f, effectVolume = 1;
    float currentVolume;
    [SerializeField]
    float m_volumeMultiplier = 0.3f;
    public float volumeMultiplier
    {
        get { return m_volumeMultiplier; }

        set
        {
            m_volumeMultiplier = value;
            foreach (var item in audioSources)
                item.volume = baseVolume * m_volumeMultiplier;
            audioSources[0].volume = effectVolume * m_volumeMultiplier;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null)
        {
            AudioManager.instance.startMusicClip = startMusicClip;
            AudioManager.instance.combatMusicClip = combatMusicClip;
            AudioManager.instance.PlayMusic(startMusicClip);
            Destroy(this.gameObject);
        }

        volumeMultiplier = volumeMultiplier;
        instance = this;
        DontDestroyOnLoad(this.gameObject);
        PlayMusic(startMusicClip, fadeTime: 0);
    }

    public void PlayMusic(AudioClip music, float volume = 1, float fadeTime = 1)
    {
        if (music != currentClip)
        {
            if (fadeTime > 0)
            {
                FadeChannelOut(currentSource, fadeTime);
                currentSource++;

                if (currentSource >= audioSources.Length)
                    currentSource = 1;
            }

            audioSources[currentSource].clip = music;
            audioSources[currentSource].volume = volume * volumeMultiplier;
            audioSources[currentSource].Play();
            audioSources[currentSource].loop = true;
        }

        baseVolume = volume;
        currentClip = music;
    }

    void FadeChannelOut(int channel, float fadeTime)
    {
        StopAllCoroutines();
        StartCoroutine(IFadeChannel(0.1f, channel, audioSources[currentSource].volume / fadeTime));
    }

    IEnumerator IFadeChannel(float delay, int channel, float volumeStep)
    {
        yield return new WaitForSecondsRealtime(delay);
        audioSources[channel].volume -= volumeStep * delay;
        if (audioSources[channel].volume > 0)
            StartCoroutine(IFadeChannel(delay, channel, volumeStep));
    }

    public void ChangeVolume(float newVolumeMultiplier)
    {
        volumeMultiplier = newVolumeMultiplier;

        currentVolume = volumeMultiplier * baseVolume;

        audioSources[currentSource].volume = currentVolume;
    }

    public void PlaySoundEffect(AudioClip soundEffect, float volume = 1f)
    {
        if (audioSources[0] != null && soundEffect != null)
        {
            audioSources[0].PlayOneShot(soundEffect, volume);
        }
    }

    [ContextMenu("Exploration Music - Fade")]
    public void ExploreMusicFade(bool victory = true)
    {
        if (victory)
            PlaySoundEffect(combatVictorySound, 1.8f);
        PlayMusic(startMusicClip);
    }

    [ContextMenu("Combat Music - Fade")]
    public void CombatMusicFade()
    {
        PlayMusic(combatMusicClip);
    }
}