using UnityEngine;
using System.Collections;

public class HitByBulletAudio : MonoBehaviour, IHitByBullet
{
    private AudioSource m_audioSource;
    public AudioClip m_audioClip;
    [Range(0 , 1)]
    public float clipVolume = 1;

    public void HitByBullet(Vector3 a_Vecocity , Vector3 HitPoint)
    {
        Debug.Log("help");
        m_audioSource.PlayOneShot(m_audioClip , clipVolume);
    }

    void Awake()
    {
        m_audioSource = GetComponent<AudioSource>();
        if (m_audioSource == null)
        {
            m_audioSource = this.gameObject.AddComponent<AudioSource>();
            m_audioSource.playOnAwake = false;
        }
    }
}
