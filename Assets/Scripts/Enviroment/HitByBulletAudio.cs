using UnityEngine;
using System.Collections;

public class HitByBulletAudio : MonoBehaviour, IHitByBullet
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
        if (m_audioSource == null)
        {
            m_audioSource = this.gameObject.AddComponent<AudioSource>();
            m_audioSource.playOnAwake = false;
            m_audioSource.spatialBlend =  1;
            m_audioSource.clip = m_audioClip;
            m_audioSource.pitch = (m_bRandomizePitch) ? Random.Range(0.9f , 1.1f) : 1;

        }
    }
}
