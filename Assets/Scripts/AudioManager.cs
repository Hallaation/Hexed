using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public List<AudioSource> AudioSources = new List<AudioSource>();
    private static AudioManager mInstance;

    public static AudioManager Instance
    {
        get
        {
            if (!mInstance)
            {
                mInstance = (AudioManager)FindObjectOfType(typeof(AudioManager));
                if (!mInstance)
                {
                    mInstance = (new GameObject("AudioManager")).AddComponent<AudioManager>();
                }
            }
            return mInstance;
        }
    }
    // Use this for initialization
    void Awake()
    {
        foreach (AudioSource auSource in this.GetComponents<AudioSource>())
        {
            AudioSources.Add(auSource);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }


}
