using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static Define;

public class SoundManager
{
    AudioSource[]                   _audioSources = new AudioSource[(int)Define.Sound.MaxCount];
    Dictionary<string, AudioClip>   _audioClips = new Dictionary<string, AudioClip>();
    float[]                         _previousVolume = new float[(int)Define.Sound.MaxCount] { 1, 1 };
    float[]                         _lastVolume = new float[(int)Define.Sound.MaxCount] { 1, 1 };
    public void Init()
    {
        GameObject root = GameObject.Find("@Sound");
        if(root == null)
        {
            root = new GameObject { name = "@Sound" };
            Object.DontDestroyOnLoad(root);

            string[] soundNames = System.Enum.GetNames(typeof(Define.Sound));
            for(int i = 0; i<soundNames.Length-1; i++)
            {
                GameObject go = new GameObject { name = soundNames[i] };
                _audioSources[i] = go.AddComponent<AudioSource>();

                //3d SoundEffect Fix
                /*
                _audioSources[i].spatialBlend = 1.0f;
                _audioSources[i].minDistance = 1.0f;
                _audioSources[i].maxDistance = 20.0f;
                _audioSources[i].rolloffMode = AudioRolloffMode.Linear;
                */
                go.transform.parent = root.transform;
            }

            _audioSources[(int)Define.Sound.Bgm].loop = true;
        }
    }

    public void Clear()
    {
        foreach(AudioSource audioSource in _audioSources)
        {
            audioSource.clip = null;
            audioSource.Stop();
        }
        _audioClips.Clear();
    }

    public void Play(string path, Define.Sound type = Define.Sound.Effect,float pitch = 1.0f)
    {
        AudioClip audioClip = GetOrAddAudioClip(path, type);
        
        Play(audioClip, type, pitch);
    }

    public void Play(AudioClip audioClip, Define.Sound type = Define.Sound.Effect, float pitch = 1.0f)
    {

        if (type == Define.Sound.Bgm)
        {

            AudioSource audioSource = _audioSources[(int)Define.Sound.Bgm];

            if (audioSource.isPlaying == true)
                audioSource.Stop();

            audioSource.pitch = pitch;
            audioSource.clip = audioClip;
            audioSource.Play();
        }
        else
        {
            AudioSource audioSource = _audioSources[(int)Define.Sound.Effect];
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(audioClip);

        }
    }

    public void SetVolume(float volumeValue, Define.Sound volume)
    {
        _audioSources[(int)volume].volume = volumeValue;
    }

    public void OnOffVolume(UnityEngine.UI.Slider slider, Define.Sound volume, bool alreadyOn)
    {
        if (alreadyOn)
        {
            _previousVolume[(int)volume] = slider.value;
            slider.value = 0;
        }
        else if (!alreadyOn)
        {
            slider.value = _previousVolume[(int)volume];
        }

    }

    public void SaveCurrentVolume()
    {
        _lastVolume[(int)Define.Sound.Effect] = _audioSources[(int)Define.Sound.Effect].volume;
        _lastVolume[(int)Define.Sound.Bgm] = _audioSources[(int)Define.Sound.Bgm].volume;
    }

    public float LoadCurrentVolume(Define.Sound volume) { return _lastVolume[(int)volume]; }


    AudioClip GetOrAddAudioClip(string path, Define.Sound type = Define.Sound.Effect)
    {
        if (path.Contains("Sounds/") == false)
            path = $"Sounds/{path}";

        AudioClip audioClip;

        if (type == Define.Sound.Bgm)
        {
            audioClip = Managers.Resource.Load<AudioClip>(path);

        }
        else
        {
            //save initial audio clip
            if (_audioClips.TryGetValue(path, out audioClip) == false)
            {
                audioClip = Managers.Resource.Load<AudioClip>(path);
                _audioClips.Add(path, audioClip);
            }
        }

        if (audioClip == null)
            Debug.Log($"Audio Clip Missing! {path}");

        return audioClip;

    }
}
