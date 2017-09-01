using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    private static AudioManager mInstance;

    public static AudioManager Instance
    {
        get { return null; }
    }
	// Use this for initialization
	void Awake ()
    {
        SingletonTester.Instance.AddSingleton(this);
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
