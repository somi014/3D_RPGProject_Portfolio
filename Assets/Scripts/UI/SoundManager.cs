using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Sound
{
    Bgm,
    Effect,
    MaxCount,
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    private AudioSource[] audioSources = new AudioSource[(int)Sound.MaxCount];
    [SerializeField] 
    private AudioClip[] audioClips;

    private AudioSource audioSource;

    private void Awake()
    {
        instance = this;

        audioSource = GetComponent<AudioSource>();
    }

    public void Play(AudioClip audioClip, Sound type = Sound.Effect, float pitch = 1.0f)
    {
        if (audioClip == null)
            return;

        if (type == Sound.Bgm)          // BGM 배경음악 재생
        {
            AudioSource bgmSource = audioSources[(int)Sound.Bgm];
            if (bgmSource.isPlaying)
            {
                bgmSource.Stop();
            }

            bgmSource.pitch = pitch;
            bgmSource.clip = audioClip;
            bgmSource.Play();
        }
        else                            // Effect 효과음 재생
        {
            audioSource.clip = audioClip;
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(audioClip);
        }
    }

    public void Play(int index, float pitch = 1.0f)
    {
        audioSource.clip = audioClips[index];
        audioSource.pitch = pitch;
        audioSource.PlayOneShot(audioClips[index]);
    }
}