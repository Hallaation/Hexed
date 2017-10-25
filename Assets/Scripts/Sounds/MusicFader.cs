using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicFader : MonoBehaviour
{
    bool MixerFound = false;
    AudioMixer MenuAudioMasterMixer;
    float MusicOriginalVolume;
    float CurrentVolume;
    GameObject SettingsManagerGameObject;
    SettingsManager SettingsManagerScript;
    float FadeSpeed;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

   public void DoStuff()
    {
        StartCoroutine(MusicFadeOut());
    }

   public IEnumerator MusicFadeOut()
    {
        if (MixerFound == false)
        {
            MenuAudioMasterMixer = transform.root.Find("SettingsManager").GetComponent<AudioMixer>();
            MixerFound = true;

            SettingsManagerGameObject = transform.root.Find("SettingsManager").gameObject;
            SettingsManagerScript = SettingsManagerGameObject.GetComponent<SettingsManager>();
            MusicOriginalVolume = SettingsManagerScript.musicVolumeSlider.value;
            CurrentVolume = MusicOriginalVolume;
        }
        while (CurrentVolume != 0)
        {
            CurrentVolume = CurrentVolume - (FadeSpeed * Time.deltaTime);
            if (CurrentVolume < 0)
            {
                CurrentVolume = 0;
            }
            MenuAudioMasterMixer.SetFloat("Music", CurrentVolume);
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }
  public  IEnumerator MusicFadeIn()
    {
        if (MixerFound != false)
        {
            while (CurrentVolume != 0)
            {
                CurrentVolume = CurrentVolume - (FadeSpeed * Time.deltaTime);
                if (CurrentVolume < MusicOriginalVolume)
                {
                    CurrentVolume += (FadeSpeed * Time.deltaTime);
                }
                MenuAudioMasterMixer.SetFloat("Music", CurrentVolume);
                yield return new WaitForEndOfFrame();
            }
        }
        yield return null;
    }
}
