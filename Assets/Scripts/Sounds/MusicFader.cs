using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicFader : MonoBehaviour
{
    AudioMixer MasterAudioMixer;
    float MusicOriginalVolume = 0;
    float CurrentVolume;

    public float FadeSpeed = 15;
    bool MusicFadeInB = false;
    // Use this for initialization
    private void Awake()
    {
        MasterAudioMixer = AudioManager.RequestMixerGroup(SourceType.MASTER).audioMixer;
        MasterAudioMixer.GetFloat("Music", out MusicOriginalVolume);
        CurrentVolume = MusicOriginalVolume;
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
        MasterAudioMixer.GetFloat("Music", out MusicOriginalVolume);
        while (CurrentVolume != -80 && MusicFadeInB == false)
        {
            if (MusicFadeInB == true)
            {
                CurrentVolume = MusicOriginalVolume;
                MasterAudioMixer.SetFloat("Music", CurrentVolume);
                SettingsManager.Instance.musicVolumeSlider.value = CurrentVolume;
                yield return null;
            }
            CurrentVolume = CurrentVolume - (FadeSpeed * Time.deltaTime);
            if (CurrentVolume < -80)
            {
                CurrentVolume = -80;
            }
            MasterAudioMixer.SetFloat("Music", CurrentVolume);
            SettingsManager.Instance.musicVolumeSlider.value = CurrentVolume;

            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }
    public IEnumerator MusicFadeIn()
    {
        while (CurrentVolume != MusicOriginalVolume)
        {
            MusicFadeInB = true;

            CurrentVolume = CurrentVolume + (FadeSpeed * Time.deltaTime);
            if (CurrentVolume > MusicOriginalVolume)
            {
                CurrentVolume = MusicOriginalVolume;
            }
            MasterAudioMixer.SetFloat("Music", CurrentVolume);
            SettingsManager.Instance.musicVolumeSlider.value = CurrentVolume;

            yield return new WaitForEndOfFrame();
        }
        MusicFadeInB = false;
        yield return null;
    }
}
