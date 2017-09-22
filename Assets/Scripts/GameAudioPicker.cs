using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAudioPicker : MonoBehaviour {

    AudioSource m_AudioSource;

    public AudioClip[] MusicClips;
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
		
	}
}
