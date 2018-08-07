using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
public enum SourceType
{
    MASTER,
    SFX,
    MUSIC,
}

public class AudioManager : MonoBehaviour
{
    public List<AudioSource> AudioSources = new List<AudioSource>();
    private static AudioManager mInstance;

    AudioMixerGroup MasterVolumeMixer;
    AudioMixerGroup SFXVolumeMixer;
    AudioMixerGroup MusicVolumeMixer;
    
    public static AudioManager Instance
    {
        get
        {
            if (!mInstance)
            {
                mInstance = (AudioManager)FindObjectOfType(typeof(AudioManager));
                if (!mInstance)
                {
                    mInstance = (new GameObject("AudioManager")).AddComponent<AudioManager>();
                }
            }
            return mInstance;
        }
    }
    // Use this for initialization
    void Awake()
    {
        foreach (AudioSource auSource in this.GetComponents<AudioSource>())
        {
            AudioSources.Add(auSource);
        }
        MasterVolumeMixer = (Resources.Load("AudioMixer/MasterAudio") as GameObject).GetComponent<AudioSource>().outputAudioMixerGroup;
        SFXVolumeMixer = (Resources.Load("AudioMixer/SFXAudio") as GameObject).GetComponent<AudioSource>().outputAudioMixerGroup;
        MusicVolumeMixer = (Resources.Load("AudioMixer/MusicAudio") as GameObject).GetComponent<AudioSource>().outputAudioMixerGroup;
    }

    public static AudioMixerGroup RequestMixerGroup(SourceType audioType)
    {
        switch (audioType)
        {
            case SourceType.MASTER:
                return (Resources.Load("AudioMixer/MasterAudio") as GameObject).GetComponent<AudioSource>().outputAudioMixerGroup;
            case SourceType.SFX:
                return (Resources.Load("AudioMixer/SFXAudio") as GameObject).GetComponent<AudioSource>().outputAudioMixerGroup;
            case SourceType.MUSIC:
                return (Resources.Load("AudioMixer/MusicAudio") as GameObject).GetComponent<AudioSource>().outputAudioMixerGroup;
            default:
                break;
        }
        return null;
    }

}
