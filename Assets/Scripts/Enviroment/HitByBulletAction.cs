using UnityEngine;
using System.Collections;
using System;

public class HitByBulletAction : MonoBehaviour, IHitByBullet
{
    private AudioSource m_audioSource;
    public AudioClip m_audioClip;
    public bool m_bRandomizePitch = false;
    [Range(0, 1)]
    public float clipVolume = 1;
    public bool m_bShakeCamera = true;
    public bool PlayParticles = false;
    public GameObject ParticlePrefab;

    public void HitByBullet(Vector3 a_Vecocity, Vector3 HitPoint)
    {

        //m_audioSource.PlayOneShot(m_audioClip , clipVolume);
        m_audioSource.Play();

        if (PlayParticles)
        {
            if (ParticlePrefab)
            {
                StartCoroutine(MakeAndPlayParticle());
            }
            //TODO play particles
        }

        if (m_bShakeCamera)
        {
            CameraShake.Instance.ShakeCamera();
        }
    }

    IEnumerator MakeAndPlayParticle()
    {
        GameObject particleMade = Instantiate(ParticlePrefab, this.transform.position, ParticlePrefab.transform.rotation);
        ParticleSystem ps = particleMade.GetComponent<ParticleSystem>();
        ps.Play();
        yield return null;
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
            m_audioSource.spatialBlend = 1;
            m_audioSource.clip = m_audioClip;
            m_audioSource.pitch = (m_bRandomizePitch) ? UnityEngine.Random.Range(0.9f, 1.1f) : 1;
        }
    }

}
