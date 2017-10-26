using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicFader : MonoBehaviour
{
    bool MixerFound = false;
    AudioMixer MenuAudioMasterMixer;
    float MusicOriginalVolume = 0;
    float CurrentVolume;
    Transform SettingsManagerGameObject;
    SettingsManager SettingsManagerScript;
    public float FadeSpeed = 15;
    bool MusicFadeInB = false;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void FadeOut()
    {
        StartCoroutine(MusicFadeOut());
    }

    public void FadeIn()
    {
        StartCoroutine(MusicFadeIn());
    }

    public IEnumerator MusicFadeOut()
    {
        if (MixerFound == false)
        {
            MenuAudioMasterMixer = (Resources.Load("AudioMixer/MasterAudio") as GameObject).GetComponent<AudioSource>().outputAudioMixerGroup.audioMixer;
            MixerFound = true;
            MenuAudioMasterMixer.GetFloat("Music", out MusicOriginalVolume);
            CurrentVolume = MusicOriginalVolume;
        }
        while (CurrentVolume != -80)
        {
            if (MusicFadeInB == true)
            {
                CurrentVolume = MusicOriginalVolume;
                MenuAudioMasterMixer.SetFloat("Music", CurrentVolume);
                SettingsManager.Instance.musicVolumeSlider.value = CurrentVolume;
                yield return null;
            }
            CurrentVolume = CurrentVolume - (FadeSpeed * Time.deltaTime);
            if (CurrentVolume < -80)
            {
                CurrentVolume = -80;
            }
            Debug.Log(CurrentVolume);
            MenuAudioMasterMixer.SetFloat("Music", CurrentVolume);
            SettingsManager.Instance.musicVolumeSlider.value = CurrentVolume;

            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }
    public IEnumerator MusicFadeIn()
    {
        if (MixerFound == false)
        {
            MenuAudioMasterMixer = (Resources.Load("AudioMixer/MasterAudio") as GameObject).GetComponent<AudioSource>().outputAudioMixerGroup.audioMixer;
            MixerFound = true;
            MenuAudioMasterMixer.GetFloat("Music", out MusicOriginalVolume);
            CurrentVolume = MusicOriginalVolume;
        }
        while (CurrentVolume != MusicOriginalVolume)
        {
            MusicFadeInB = true;
            
            CurrentVolume = CurrentVolume + (FadeSpeed * Time.deltaTime);
            if (CurrentVolume > MusicOriginalVolume)
            {
                CurrentVolume = MusicOriginalVolume;
            }
            Debug.Log(CurrentVolume);
            MenuAudioMasterMixer.SetFloat("Music", CurrentVolume);
            SettingsManager.Instance.musicVolumeSlider.value = CurrentVolume;

            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }
}
