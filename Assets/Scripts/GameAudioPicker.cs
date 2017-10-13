using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAudioPicker : MonoBehaviour {

    AudioSource m_AudioSource;

    public AudioClip[] MusicClips;

    private float minVolume = 0;
    private float maxVolume = 1;
    private float currentValue = 0;
    public float VolumeIncrement = 0.1f;
    
	// Use this for initialization
	void Awake ()
    {
        object[] loadedResources = Resources.LoadAll("Audio/Music");
        MusicClips = new AudioClip[loadedResources.Length];
        for (int i = 0; i < loadedResources.Length; i++)
        {
            MusicClips[i] = loadedResources[i] as AudioClip;
        }
        m_AudioSource = this.GetComponent<AudioSource>();
        if (!m_AudioSource)
        {
            m_AudioSource = this.gameObject.AddComponent<AudioSource>();
        }

        m_AudioSource.outputAudioMixerGroup = (Resources.Load("AudioMixer/MusicAudio") as GameObject).GetComponent<AudioSource>().outputAudioMixerGroup;
        m_AudioSource.clip = MusicClips[Random.Range(0 , MusicClips.Length)];
        m_AudioSource.loop = true;
        m_AudioSource.Play();
    }
	
	// Update is called once per frame
	void Update ()
    {
        m_AudioSource.volume = Mathf.Lerp(minVolume, maxVolume, currentValue);
        currentValue += VolumeIncrement * Time.deltaTime;

		if (currentValue > maxVolume)
        {
            currentValue = 1;
        }
	}
}
