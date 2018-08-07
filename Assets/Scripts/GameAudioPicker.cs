using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameAudioPicker : MonoBehaviour
{

    AudioSource m_AudioSource;

    public AudioClip[] MusicClips;

    private float minVolume = 0;
    private float maxVolume = 1;
    private float currentValue = 0;
    public float VolumeIncrement = 0.1f;

    private static GameAudioPicker mInstance;

    public static GameAudioPicker Instance
    {
        get
        {
            if (mInstance == null)
            {
                //If I already exist, make the instance that
                mInstance = (GameAudioPicker)FindObjectOfType(typeof(GameAudioPicker));

                if (mInstance == null)
                {
                    //if not found, make an object and attach me to it
                    mInstance = (new GameObject("GameAudio")).AddComponent<GameAudioPicker>();
                }
                //mInstance.gameObject.name = "Singleton container";
                DontDestroyOnLoad(mInstance.gameObject);
            }
            return mInstance;
        }
    }

    // Use this for initialization
    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoad;
        mInstance = Instance;
        object[] loadedResources = Resources.LoadAll("Audio/Music");
        MusicClips = new AudioClip[loadedResources.Length];
        for (int i = 0; i < loadedResources.Length; i++)
        {
            MusicClips[i] = loadedResources[i] as AudioClip;
        }
        m_AudioSource = this.GetComponent<AudioSource>();
        if (!m_AudioSource)
        {
            m_AudioSource = this.gameObject.AddComponent<AudioSource>();
        }

        m_AudioSource.outputAudioMixerGroup = AudioManager.RequestMixerGroup(SourceType.MUSIC);
        m_AudioSource.clip = MusicClips[Random.Range(0, MusicClips.Length)];
        m_AudioSource.loop = true;
        m_AudioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        m_AudioSource.volume = Mathf.Lerp(minVolume, maxVolume, currentValue);
        currentValue += VolumeIncrement * Time.deltaTime;

        if (currentValue > maxVolume)
        {
            currentValue = 1;
        }
    }

    public void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        GameAudioPicker[] gameAudios = FindObjectsOfType<GameAudioPicker>();
        for (int i = 0; i < gameAudios.Length; i++)
        {
            if (gameAudios[i].gameObject.GetHashCode() != GameAudioPicker.Instance.gameObject.GetHashCode())
            {
                Destroy(gameAudios[i].gameObject);
            }
        }
    }

}
