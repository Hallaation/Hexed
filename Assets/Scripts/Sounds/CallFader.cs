using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallFader : MonoBehaviour {
   public float m_FadeSpeed;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void Fade()
    {
        GameManagerc.Instance.GetComponent<MusicFader>().FadeOut();
        GameManagerc.Instance.GetComponent<MusicFader>().FadeSpeed = m_FadeSpeed;
    }
}
