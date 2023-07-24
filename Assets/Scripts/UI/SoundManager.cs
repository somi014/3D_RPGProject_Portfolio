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

    AudioSource[] _audioSources = new AudioSource[(int)Sound.MaxCount];
    [SerializeField] private AudioClip[] audioClips;

    AudioSource audioSource;

    private void Awake()
    {
        instance = this;

        audioSource = GetComponent<AudioSource>();
    }

    public void Init()
    {
        GameObject root = GameObject.Find("@Sound");
        if (root == null)
        {
            root = new GameObject { name = "@Sound" };
            Object.DontDestroyOnLoad(root);

            string[] soundNames = System.Enum.GetNames(typeof(Sound)); // "Bgm", "Effect"
            for (int i = 0; i < soundNames.Length - 1; i++)
            {
                GameObject go = new GameObject { name = soundNames[i] };
                _audioSources[i] = go.AddComponent<AudioSource>();
                go.transform.parent = root.transform;
            }

            _audioSources[(int)Sound.Bgm].loop = true; // bgm 재생기는 무한 반복 재생
        }
    }


    public void Play(AudioClip audioClip, Sound type = Sound.Effect, float pitch = 1.0f)
    {
        if (audioClip == null)
            return;

        if (type == Sound.Bgm) // BGM 배경음악 재생
        {
            AudioSource audioSource = _audioSources[(int)Sound.Bgm];
            if (audioSource.isPlaying)
                audioSource.Stop();

            audioSource.pitch = pitch;
            audioSource.clip = audioClip;
            audioSource.Play();
        }
        else // Effect 효과음 재생
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

    //public void Play(string path, Sound type = Sound.Effect, float pitch = 1.0f)
    //{
    //    AudioClip audioClip = GetOrAddAudioClip(path, type);
    //    Play(audioClip, type, pitch);
    //}
}