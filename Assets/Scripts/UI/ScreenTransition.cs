using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenTransition : MonoBehaviour {

    Animator m_Animator;
	// Use this for initialization
	void Awake ()
    {
        m_Animator = this.GetComponent<Animator>();

        OpenDoor();
        SceneManager.sceneLoaded += OnSceneLoad;
	}
	
    public void CloseDoor()
    {
        m_Animator.SetTrigger("CloseDoor");
    }


    public void OpenDoor()
    {
        m_Animator.SetTrigger("OpenDoor");
    }

    void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        if (FindObjectOfType<ScreenTransition>() != this)
        {
            Destroy(FindObjectOfType<ScreenTransition>().gameObject);
        }
        this.transform.SetParent(FindObjectOfType<Canvas>().transform);
    }
}
