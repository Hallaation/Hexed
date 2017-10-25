using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using XboxCtrlrInput;
public class ScreenTransition : MonoBehaviour
{
    public Animator m_Animator;

    UINavigation _UINavi;
    EventSystem _eventSystem;

    private Image BlackOut;
    private Image TeamLogo;

    private Queue<Image> fadingQueue = new Queue<Image>();
    // Use this for initialization

    void Awake()
    {
        m_Animator = this.GetComponent<Animator>();

        TeamLogo = this.transform.GetChild(3).GetComponent<Image>();
        BlackOut = this.transform.GetChild(4).GetComponent<Image>();
        fadingQueue.Enqueue(BlackOut);
        fadingQueue.Enqueue(TeamLogo);
        StartCoroutine(WakeUp());
       // OpenDoor();
        _UINavi = UINavigation.Instance;
        _eventSystem = FindObjectOfType<EventSystem>();
        SceneManager.sceneLoaded += OnSceneLoad;
    }

    IEnumerator WakeUp()
    {
        float t = 0.0f;
        float maxTime = 3;
        float alphaValue = 0;

        while (fadingQueue.Count != 0)
        {
            while (t < maxTime)
            {
                Debug.Log("is this shit even running");
                t += Time.deltaTime;
                alphaValue = Mathf.Lerp(255.0f, 0, t);
                fadingQueue.Peek().color = new Vector4(1, 1, 1, alphaValue / 255.0f);
                for (int i = 0; i < 4; i++)
                {
                    if (XCI.GetButton(XboxButton.A, XboxController.Any + i))
                    {
                        t = maxTime;
                        fadingQueue.Peek().color = new Vector4(1, 1, 1, 0);
                    }
                }
                yield return null;
            }
            t = 0.0f;
            fadingQueue.Dequeue();
            yield return null;
        }
        OpenDoor();
    }
    void Update()
    {
        if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Scene_Transition_Open"))
        {
            if (_eventSystem)
                _eventSystem.enabled = true;
            if (_UINavi)
                _UINavi.enabled = true;
        }
        else
        {
            if (_eventSystem)
                _eventSystem.enabled = false;
            if (_UINavi)
                _UINavi.enabled = false;

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
        _eventSystem = FindObjectOfType<EventSystem>();
        // if (FindObjectOfType<ScreenTransition>() != this)
        // {
        //     Destroy(FindObjectOfType<ScreenTransition>().gameObject);
        // }
        // this.transform.SetParent(FindObjectOfType<Canvas>().transform);
        // this.transform.localPosition = Vector3.zero;
        // m_Animator.SetTrigger("OpenDoor");
    }
}
