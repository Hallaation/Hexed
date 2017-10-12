using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.IO;

public class SettingsManager : MonoBehaviour
{
    public Toggle fullscreenToggle;
    public Dropdown resolutionDropdwon;
    public Dropdown textureQualityDropdown;
    public Dropdown AAdropdown;
    public Dropdown vSyncDrop;
    public Slider masterVolumeSlider;
    public Slider sfxVolumeSlider;
    public Slider musicVolumeSlider;
    public Button applyButton;
    public Resolution[] resolutions;
    public Settings gameSettings;

    private bool m_bUnsavedChanges = false;
    public AudioMixer masterMixer;

    public bool UnsavedChanges { get { return m_bUnsavedChanges; } }

    private static SettingsManager mInstance;

    public static SettingsManager Instance
    {
        get
        {
            if (!mInstance)
            {
                mInstance = (SettingsManager)FindObjectOfType(typeof(SettingsManager));

            }
            return mInstance;
        }
    }


    // Use this for initialization
    void OnEnable()
    {

        masterMixer = (Resources.Load("AudioMixer/MasterAudio") as GameObject).GetComponent<AudioSource>().outputAudioMixerGroup.audioMixer;

        gameSettings = new Settings();
        resolutions = Screen.resolutions;

        //subscribe onfullscreentoggle to value changed event;
        if (fullscreenToggle)
            fullscreenToggle.onValueChanged.AddListener(delegate { onFullScreenToggle(); });
        if (resolutionDropdwon)
            resolutionDropdwon.onValueChanged.AddListener(delegate { onResolutionChange(); });
        //textureQualityDropdown.onValueChanged.AddListener(delegate { onTextureQualityChange(); });
        //AAdropdown.onValueChanged.AddListener(delegate { onAntialiasingChange(); });
        if (vSyncDrop)
            vSyncDrop.onValueChanged.AddListener(delegate { onVsyncChange(); });
        if (masterVolumeSlider)
            masterVolumeSlider.onValueChanged.AddListener(delegate { OnMasterVolumeChange(); });
        if (sfxVolumeSlider)
            sfxVolumeSlider.onValueChanged.AddListener(delegate { OnSFXVolumeChange(); });
        if (musicVolumeSlider)
            musicVolumeSlider.onValueChanged.AddListener(delegate { OnMusicVolumeChange(); });
        if (applyButton)
            applyButton.onClick.AddListener(delegate { OnApplyButtonClick(); });

        if (resolutionDropdwon)
        {
            foreach (Resolution reso in resolutions)
            {
                resolutionDropdwon.options.Add(new Dropdown.OptionData(reso.ToString()));
            }
        }

        LoadSettings();
    }

    public void onFullScreenToggle()
    {
        /*Screen.fullScreen = */
        gameSettings.Fullscreen = fullscreenToggle.isOn;
        m_bUnsavedChanges = true;
    }

    public void onResolutionChange()
    {
        gameSettings.resolutionIndex = resolutionDropdwon.value;
        m_bUnsavedChanges = true;
    }

    public void onTextureQualityChange()
    {
        /*QualitySettings.masterTextureLimit =*/
        gameSettings.textureQuality = textureQualityDropdown.options.Count - textureQualityDropdown.value;
        m_bUnsavedChanges = true;
    }

    public void onAntialiasingChange()
    {
        /*QualitySettings.antiAliasing = */
        gameSettings.antiAliasing = (int)AAdropdown.value * (int)AAdropdown.value;
        m_bUnsavedChanges = true;
    }

    public void onVsyncChange()
    {
        /*QualitySettings.vSyncCount = */
        gameSettings.vSync = vSyncDrop.value;

        m_bUnsavedChanges = true;
    }

    public void OnMasterVolumeChange()
    {
        /*AudioListener.volume = */
        gameSettings.masterVolume = masterVolumeSlider.value;
        masterMixer.SetFloat("Master", masterVolumeSlider.value);
        m_bUnsavedChanges = true;
    }

    public void OnSFXVolumeChange()
    {
        gameSettings.sfxVolume = sfxVolumeSlider.value;
        masterMixer.SetFloat("SFX", sfxVolumeSlider.value);
        m_bUnsavedChanges = true;
    }

    public void OnMusicVolumeChange()
    {
        gameSettings.musicVolume = musicVolumeSlider.value;
        masterMixer.SetFloat("Music", musicVolumeSlider.value);
        m_bUnsavedChanges = true;
    }
    public void OnApplyButtonClick()
    {
        Screen.fullScreen = gameSettings.Fullscreen;
        Screen.SetResolution(resolutions[gameSettings.resolutionIndex].width, resolutions[gameSettings.resolutionIndex].height, gameSettings.Fullscreen);
        QualitySettings.vSyncCount = gameSettings.vSync;
        //AudioListener.volume = gameSettings.masterVolume; Change to the master volume mixer
        SaveSettings();
    }
    public void SaveSettings()
    {
     //   Debug.Log("settings saved");
        string jsonData = JsonUtility.ToJson(gameSettings, true);
        File.WriteAllText(Application.persistentDataPath + "/gameSettings.json", jsonData);
        print(Application.persistentDataPath);
    }

    public void LoadSettings()
    {
        if (File.Exists(Application.persistentDataPath + "/gameSettings.json"))
        {
            gameSettings = JsonUtility.FromJson<Settings>(File.ReadAllText(Application.persistentDataPath + "/gameSettings.json"));
            if (masterVolumeSlider)
                masterVolumeSlider.value = gameSettings.masterVolume;
            if (sfxVolumeSlider)
                sfxVolumeSlider.value = gameSettings.sfxVolume;
            if (musicVolumeSlider)
                musicVolumeSlider.value = gameSettings.musicVolume;
            //AAdropdown.value = (int)Mathf.Sqrt(gameSettings.antiAliasing);
            //textureQualityDropdown.value = textureQualityDropdown.options.Count + gameSettings.textureQuality;
            if (resolutionDropdwon)
                resolutionDropdwon.value = gameSettings.resolutionIndex;

            if (fullscreenToggle)
            {
                fullscreenToggle.isOn = gameSettings.Fullscreen;
                onFullScreenToggle();
            }

            if (resolutionDropdwon)
            {
                resolutionDropdwon.RefreshShownValue();
            }

            //Apply the changes
            OnMasterVolumeChange();
            OnSFXVolumeChange();
            OnMusicVolumeChange();

            //onAntialiasingChange();
            //onTextureQualityChange();
        }
    }
}
