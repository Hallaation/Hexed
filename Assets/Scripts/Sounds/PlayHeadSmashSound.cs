using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayHeadSmashSound : MonoBehaviour {
    public AudioClip HeadSmasheeSound;
    AudioSource audio;
    public bool Play;
    // Use this for initialization
    void Start () {
        
      
      
    }
	
    void Awake()
    {
        
    }

   public void PlaySound()
    {
        audio = GetComponent<AudioSource>();
        audio.outputAudioMixerGroup = (Resources.Load("AudioMixer/SFXAudio") as GameObject).GetComponent<AudioSource>().outputAudioMixerGroup;
        audio.Play();
    }
	// Update is called once per frame
	void Update () {
		
	}
}
