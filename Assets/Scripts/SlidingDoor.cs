using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingDoor : MonoBehaviour {
    public float TravelDistance = 10;
    public float TimeToOpen = 2;
    public float TimeStaysOpen = 2;
    public float TimeBetweenMovements = .1f;
    float timer;
    BoxCollider2D Coll;
    Collider2D Coll2d;
    Transform ChildPosition;
    Vector3 OriginalPosition;
    Vector3 EndPosition;
    bool DoorIsOpening;

    [Space]
    //Rigidbody2D MyRigidBody;

    [Space]
    [Header("Door Open Audio")]
    public AudioClip m_MovingClip;
    [Range(0 , 1)]
    public float m_MovingVolume = 1;

    [Space]
    [Header("Door Close Audio")]
    public AudioClip m_ClosingAudio;
    [Range(0 , 1)]
    public float m_ClosingVolume = 1;

    private AudioSource m_audioSource;
    private bool PlayAudio = false;
    private bool PlayClosing = false;
    // Use this for initialization
    void Start () {
        m_audioSource = this.GetComponent<AudioSource>();
        if (m_audioSource == null)
        {
            m_audioSource = this.gameObject.AddComponent<AudioSource>();
            m_audioSource.outputAudioMixerGroup = AudioManager.RequestMixerGroup(SourceType.SFX);
            //m_audioSource.outputAudioMixerGroup = (Resources.Load("AudioMixer/SFXAudio") as  AudioSource).outputAudioMixerGroup;
            m_audioSource.playOnAwake = false;
            m_audioSource.spatialBlend =  1;
        }
        m_audioSource.clip = m_MovingClip;
        m_audioSource.volume = m_MovingVolume;
        Coll = GetComponent<BoxCollider2D>();
        Coll2d = GetComponent<Collider2D>();
   
        ChildPosition = transform.GetChild(0).GetComponentInChildren<Transform>();
        OriginalPosition = ChildPosition.transform.position;
        EndPosition = OriginalPosition + (transform.up * TravelDistance);
        DoorIsOpening = false;
        timer = 0;
    }
	
	// Update is called once per frame
	void Update () {
		if(DoorIsOpening == false)
        {
            if (!Coll.IsTouchingLayers(1 << LayerMask.NameToLayer("Player")))
            {
                if (timer >= TimeStaysOpen)
                {
                    StartCoroutine(CCloseDoor());
                    timer = 0;
                }
                timer += Time.deltaTime;
            }
        }
        if (ChildPosition.position.magnitude >= OriginalPosition.magnitude - 0.1f)
        {
            PlayClosing = false;
        }
        if (PlayAudio)
        {
            m_audioSource.clip = m_MovingClip;
            m_audioSource.volume = m_MovingVolume;
            m_audioSource.Play();
            PlayAudio = false;
        }

        if (PlayClosing)
        {
            m_audioSource.clip = m_ClosingAudio;
            m_audioSource.volume = m_ClosingVolume;
            m_audioSource.Play();
            PlayClosing = false;
        }

    }

    void OnTriggerEnter2D(Collider2D Collider)
    {
        if (Collider.tag == "Player")
        {
            if (DoorIsOpening == false)
            {
                DoorIsOpening = true;
                StartCoroutine(COpenDoor());
            }
        }
    }

   void OpenDoor()
    {
        float ctimer = 0f;
        while (timer < TimeToOpen)
        {
            ChildPosition.position += new Vector3(0, ((TravelDistance / TimeToOpen) * Time.deltaTime));
            if(ChildPosition.position.y > OriginalPosition.y + TravelDistance)
            {
                ChildPosition.position = new Vector3(OriginalPosition.x, OriginalPosition.y + TravelDistance);
                break;
            }
            ctimer += Time.deltaTime;

        }
        //CloseDoor();
    }
    void CloseDoor()
    {
        float ctimer = 0f;
        while (timer < TimeToOpen)
        {
            ChildPosition.position -= new Vector3(0, ((TravelDistance / TimeToOpen) * Time.deltaTime));
            if (ChildPosition.position.y < OriginalPosition.y)
            {
                ChildPosition.position = new Vector3(OriginalPosition.x, OriginalPosition.y);
                break;
            }
            ctimer += Time.deltaTime;

        }
    }

    IEnumerator COpenDoor()
    {
        Vector3 startingPosition = ChildPosition.position;
        float ctimer = 0f;
        PlayAudio = true;
        while (ctimer < TimeToOpen)
        {
            ChildPosition.position = Vector3.Lerp(startingPosition, EndPosition, (ctimer / TimeToOpen));
            ctimer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        //CloseDoor();
        DoorIsOpening = false;
    }
    IEnumerator CCloseDoor()
    {
        Vector3 startingPosition = ChildPosition.position;
        float ctimer = 0f;

        if (startingPosition != OriginalPosition)
        {
            PlayClosing = true;
        }

        while (ctimer < TimeToOpen && DoorIsOpening == false)  
        {
            ChildPosition.position = Vector3.Lerp(startingPosition, OriginalPosition, (ctimer / TimeToOpen));
            ctimer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
    //    //    IEnumerator COpenDoor()
    //    {

    //        float ctimer = 0f;
    //        while (ctimer <= TimeToOpen)
    //        {
    //            ChildPosition.position += new Vector3(0, ((TravelDistance / TimeToOpen) * TimeBetweenMovements));
    //            if (ChildPosition.position.y > OriginalPosition.y + TravelDistance)
    //            {
    //                ChildPosition.position = new Vector3(OriginalPosition.x, OriginalPosition.y + TravelDistance);
    //                break;
    //                timer = 0;
    //            }
    //ctimer += TimeBetweenMovements;
    //            yield return new WaitForEndOfFrame();
    //timer = 0;
    //        }
    //        //CloseDoor();
    //        DoorIsOpening = false;
    //    }
    //    IEnumerator CCloseDoor()
    //{
    //    float ctimer = 0f;
    //    while (ctimer < TimeToOpen && DoorIsOpening == false)
    //    {
    //        ChildPosition.position -= new Vector3(0, ((TravelDistance / TimeToOpen) * TimeBetweenMovements));
    //        if (ChildPosition.position.y < OriginalPosition.y)
    //        {
    //            ChildPosition.position = new Vector3(OriginalPosition.x, OriginalPosition.y);
    //            break;
    //        }
    //        ctimer += TimeBetweenMovements;
    //        yield return new WaitForEndOfFrame();

    //    }
}

