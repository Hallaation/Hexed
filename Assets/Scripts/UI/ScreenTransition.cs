using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
public class ScreenTransition : MonoBehaviour {

    public Animator m_Animator;
	// Use this for initialization
	void Awake ()
    {
        m_Animator = this.GetComponent<Animator>();

        OpenDoor();
        SceneManager.sceneLoaded += OnSceneLoad;
	}
	
    void Update()
    {
        if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Scene_Transition_Open"))
        {
            FindObjectOfType<EventSystem>().enabled = true;
            FindObjectOfType<UINavigation>().enabled = true;
        }
        else
        {
            FindObjectOfType<EventSystem>().enabled = false;
            FindObjectOfType<UINavigation>().enabled = false;
        }
    }
    public void CloseDoor()
    {
        Debug.Log("Close Door");
        m_Animator.SetTrigger("CloseDoor");
    }


    public void OpenDoor()
    {
        Debug.Log("Open Door");
        m_Animator.SetTrigger("OpenDoor");
    }

    void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        
       // if (FindObjectOfType<ScreenTransition>() != this)
       // {
       //     Destroy(FindObjectOfType<ScreenTransition>().gameObject);
       // }
       // this.transform.SetParent(FindObjectOfType<Canvas>().transform);
       // this.transform.localPosition = Vector3.zero;
       // m_Animator.SetTrigger("OpenDoor");
    }
}
