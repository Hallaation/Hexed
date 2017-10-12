using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitByMeleeAction : MonoBehaviour, IHitByMelee
{
    public bool CopyBulletAudio = false;

    private AudioSource m_audioSource;
    public AudioClip m_audioClip;
    public bool m_bRandomizePitch = false;
    [Range(0, 1)]
    public float clipVolume = 1;
    public bool m_bShakeCamera = true;
    public bool PlayParticles = false;
    public GameObject ParticlePrefab;


    void Awake()
    {
        //If I want to copy all the bullet action's audio/particle stuff, then do so. otherwise dont bother.
        if (CopyBulletAudio)
        {
            if (!this.GetComponent<HitByBulletAction>())
            {
                //Debug.LogError("Hit By Bullet Action script not found!");
            }
            else
            {
                HitByBulletAction temp = this.GetComponent<HitByBulletAction>();
                m_audioClip = temp.m_audioClip;
                m_bRandomizePitch = temp.m_bRandomizePitch;
                clipVolume = temp.clipVolume;
                m_bShakeCamera = temp.m_bShakeCamera;
                PlayParticles = temp.PlayParticles;
                ParticlePrefab = temp.ParticlePrefab;
            }
        }

        m_audioSource = GetComponent<AudioSource>();
        //m_audioSource.outputAudioMixerGroup = (Resources.Load("AudioMixer/SFXAudio") as  AudioSource).outputAudioMixerGroup;
        if (m_audioSource == null)
        {
            m_audioSource = this.gameObject.AddComponent<AudioSource>();
            m_audioSource.outputAudioMixerGroup = (Resources.Load("AudioMixer/SFXAudio") as GameObject).GetComponent<AudioSource>().outputAudioMixerGroup;
            //m_audioSource.outputAudioMixerGroup = (Resources.Load("AudioMixer/SFXAudio") as  AudioSource).outputAudioMixerGroup;
            m_audioSource.playOnAwake = false;
            m_audioSource.spatialBlend = 1;
            m_audioSource.clip = m_audioClip;
            m_audioSource.pitch = (m_bRandomizePitch) ? UnityEngine.Random.Range(0.9f, 1.1f) : 1;
        }
    }

    public void HitByMelee(Weapon meleeWeapon, AudioClip soundEffect, float Volume = 1, float Pitch = 1)
    {
        Debug.Log("Help");

        m_audioSource.Play();
    }


    IEnumerator MakeAndPlayParticle()
    {
        GameObject particleMade = Instantiate(ParticlePrefab, this.transform.position, ParticlePrefab.transform.rotation);
        ParticleSystem ps = particleMade.GetComponent<ParticleSystem>();
        ps.Play();
        yield return null;
    }
}
