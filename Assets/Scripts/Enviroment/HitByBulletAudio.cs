using UnityEngine;
using System.Collections;
using System;

public class HitByBulletAudio : MonoBehaviour, IHitByBullet, IHitByMelee
{
    private AudioSource m_audioSource;
    public AudioClip m_audioClip;
    public bool m_bRandomizePitch = false;
    [Range(0 , 1)]
    public float clipVolume = 1;

    public void HitByBullet(Vector3 a_Vecocity , Vector3 HitPoint)
    {

        //m_audioSource.PlayOneShot(m_audioClip , clipVolume);
        m_audioSource.Play();
    }

    void Awake()
    {
        m_audioSource = GetComponent<AudioSource>();
        //m_audioSource.outputAudioMixerGroup = (Resources.Load("AudioMixer/SFXAudio") as  AudioSource).outputAudioMixerGroup;
        if (m_audioSource == null)
        {
            m_audioSource = this.gameObject.AddComponent<AudioSource>();
            m_audioSource.outputAudioMixerGroup = (Resources.Load("AudioMixer/SFXAudio") as GameObject).GetComponent<AudioSource>().outputAudioMixerGroup;
            //m_audioSource.outputAudioMixerGroup = (Resources.Load("AudioMixer/SFXAudio") as  AudioSource).outputAudioMixerGroup;
            m_audioSource.playOnAwake = false;
            m_audioSource.spatialBlend =  1;
            m_audioSource.clip = m_audioClip;
            m_audioSource.pitch = (m_bRandomizePitch) ? UnityEngine.Random.Range(0.9f , 1.1f) : 1;

        }
    }

    public void HitByMelee(Weapon meleeWeapon, AudioClip soundEffect, float Volume = 1, float Pitch = 1)
    {
       
        m_audioSource.Play();
    }
}
