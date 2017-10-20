using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayHeadSmashSound : MonoBehaviour {
    public AudioClip HeadSmasheeSound;
    AudioSource audio;
    // Use this for initialization
    void Start () {
        audio = GetComponent<AudioSource>();
        audio.Play(44100);
       
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
