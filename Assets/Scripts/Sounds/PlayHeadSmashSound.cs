using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayHeadSmashSound : MonoBehaviour {
    public AudioClip HeadSmasheeSound;
    AudioSource m_AudioSource;
    public bool Play;
    // Use this for initialization
    void Start () {
        
      
      
    }
	
    void Awake()
    {
        
    }

   public void PlaySound()
    {
        m_AudioSource = GetComponent<AudioSource>();
        m_AudioSource.outputAudioMixerGroup = AudioManager.RequestMixerGroup(SourceType.SFX);
        m_AudioSource.Play();
    }
	// Update is called once per frame
	void Update () {
		
	}
}
