using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IHitByBullet
{
    public float DoorAngleOfBounce;
    public int DoorBounceForce;
    public HingeJoint2D DoorHinge;
    Quaternion StartRotation;
    [Space]
    Rigidbody2D MyRigidBody;

    [Space]
    [Header("Door Moving Audio")]
    public AudioClip m_MovingClip;
    [Range(0 , 1)]
    public float m_MovingVolume = 1;

    private AudioSource m_audioSource;
    private bool PlayAudio = false;
    public void HitByBullet(Vector3 a_Vecocity , Vector3 HitPoint)
    {
        this.GetComponent<Rigidbody2D>().AddForceAtPosition(a_Vecocity , HitPoint); //hah
    }

    // bool HasBounced = false;
    // float timer;
    // float freezeTimer;
    // Use this for initialization
    void Start()
    {
        StartRotation = transform.rotation;
        m_audioSource = GetComponent<AudioSource>();
        //m_audioSource.outputAudioMixerGroup = (Resources.Load("AudioMixer/SFXAudio") as  AudioSource).outputAudioMixerGroup;
        if (m_audioSource == null)
        {
            m_audioSource = this.gameObject.AddComponent<AudioSource>();
            m_audioSource.outputAudioMixerGroup = (Resources.Load("AudioMixer/SFXAudio") as GameObject).GetComponent<AudioSource>().outputAudioMixerGroup;
            //m_audioSource.outputAudioMixerGroup = (Resources.Load("AudioMixer/SFXAudio") as  AudioSource).outputAudioMixerGroup;
            m_audioSource.playOnAwake = false;
            m_audioSource.spatialBlend =  1;
        }

        m_audioSource.clip = m_MovingClip;
        m_audioSource.volume = m_MovingVolume;
        //freezeTimer = 0;
        //timer = 0f;
        MyRigidBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

        if (Mathf.Abs(MyRigidBody.velocity.magnitude) > 0)
        {
            if (PlayAudio)
            {
                m_audioSource.Play();
                PlayAudio = false;  
            }
            else if (!m_audioSource.isPlaying)
            {
                PlayAudio = true;
            }
        }

        if (DoorHinge.jointAngle > DoorHinge.limits.max - DoorAngleOfBounce)
        {
            MyRigidBody.AddTorque(DoorBounceForce , ForceMode2D.Force);
            //m_audioSource.Play();
            // HasBounced = true;
            // timer = 0;
        }
        else if (DoorHinge.jointAngle < DoorHinge.limits.min + DoorAngleOfBounce)
        {
            MyRigidBody.AddTorque(-DoorBounceForce , ForceMode2D.Force);
            //m_audioSource.Play();
            // HasBounced = true;
            // timer = 0;
        }
        //else
        //{
        //   // HasBounced = false;
        //   // timer += Time.deltaTime;
        //}
    }
    void Reset()
    {
        this.transform.rotation = StartRotation;
    }

}