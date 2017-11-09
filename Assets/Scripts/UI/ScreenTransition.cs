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
    public float m_fTimeBetweenFades = 3;
    UINavigation _UINavi;
    EventSystem _eventSystem;

    private Image BlackOut;
    private Image TeamLogo;
    private Image BlackOut2nd;
    private Queue<Image> fadingQueue = new Queue<Image>();
    private Stack<Image> fadingStack = new Stack<Image>();
    private bool m_bDoorOpened;
    public bool DoorOpened { get { return m_bDoorOpened; } }


    public AudioClip m_OpenClip;
    public AudioClip m_closeClip;
    private AudioSource m_AudioSource;
    // Use this for initialization

    void Awake()
    {
        m_Animator = this.GetComponent<Animator>();
        m_AudioSource = this.GetComponent<AudioSource>();
        if (!m_AudioSource)
        {
            m_AudioSource = this.gameObject.AddComponent<AudioSource>();
            m_AudioSource.loop = false;
            m_AudioSource.playOnAwake = false;
            m_AudioSource.outputAudioMixerGroup = AudioManager.RequestMixerGroup(SourceType.SFX);
        }

        if (transform.childCount > 3 && GameManagerc.Instance.m_bDoLogoTransition)
        {
            TeamLogo = this.transform.GetChild(4).GetComponent<Image>();
            BlackOut = this.transform.GetChild(5).GetComponent<Image>();
            BlackOut2nd = this.transform.GetChild(3).GetComponent<Image>();

            fadingQueue.Enqueue(BlackOut);
            fadingQueue.Enqueue(TeamLogo);
            fadingQueue.Enqueue(BlackOut2nd);


            StartCoroutine(WakeUp());
        }
        else
        {
            if (transform.childCount > 3)
            {
                TeamLogo = this.transform.GetChild(3).GetComponent<Image>(); //TODO Need to Re-add to the queue so it Fades in and then fades out.
                BlackOut = this.transform.GetChild(4).GetComponent<Image>();
                TeamLogo.enabled = false;
                BlackOut.enabled = false;
            }
            OpenDoor();
        }

        // OpenDoor();
        _UINavi = UINavigation.Instance;
        _eventSystem = FindObjectOfType<EventSystem>();
        SceneManager.sceneLoaded += OnSceneLoad;
    }

    IEnumerator WakeUp()
    {
        float t = 0.0f;
        float alphaValue = 0;

        m_bDoorOpened = false;
        yield return new WaitForSeconds(1);
        while (fadingQueue.Count != 0)
        {
            while (t < m_fTimeBetweenFades - 0.5f)
            {
                t += Time.deltaTime;
                alphaValue = Mathf.Lerp(255.0f, 0, t);
                fadingQueue.Peek().color = new Vector4(1, 1, 1, alphaValue / 255.0f);
                for (int i = 0; i < 4; i++)
                {
#if UNITY_EDITOR
                    if (XCI.GetButton(XboxButton.A, XboxController.Any + i) || Input.GetKey(KeyCode.Space))
                    {
                        t = m_fTimeBetweenFades;
                        fadingQueue.Peek().color = new Vector4(1, 1, 1, 0);
                    }
#endif
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
            m_bDoorOpened = true;
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
        Debug.Log("Closing door");
        m_AudioSource.clip = m_closeClip;
        m_AudioSource.Play();
        m_Animator.SetTrigger("CloseDoor");
    }


    public void OpenDoor()
    {
        m_AudioSource.clip = m_OpenClip;
        m_AudioSource.Play();
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
