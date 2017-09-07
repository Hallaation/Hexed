using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class SettingsManager : MonoBehaviour
{

    public Toggle fullscreenToggle;
    public Dropdown resolutionDropdwon;
    public Dropdown textureQualityDropdown;
    public Dropdown AAdropdown;
    public Dropdown vSyncDrop;
    public Slider masterVolumeSlider;
    public Button applyButton;
    public Resolution[] resolutions;
    public Settings gameSettings;

    private bool m_bUnsavedChanges = false;
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
        gameSettings = new Settings();
        resolutions = Screen.resolutions;

        //subscribe onfullscreentoggle to value changed event;
        fullscreenToggle.onValueChanged.AddListener(delegate { onFullScreenToggle(); });
        resolutionDropdwon.onValueChanged.AddListener(delegate { onResolutionChange(); });
        //textureQualityDropdown.onValueChanged.AddListener(delegate { onTextureQualityChange(); });
        //AAdropdown.onValueChanged.AddListener(delegate { onAntialiasingChange(); });
        vSyncDrop.onValueChanged.AddListener(delegate { onVsyncChange(); });
        masterVolumeSlider.onValueChanged.AddListener(delegate { OnMasterVolumeChange(); });
        applyButton.onClick.AddListener(delegate { OnApplyButtonClick(); });
        foreach (Resolution reso in resolutions)
        {
            resolutionDropdwon.options.Add(new Dropdown.OptionData(reso.ToString()));
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
        gameSettings.musicVolume = masterVolumeSlider.value;
        m_bUnsavedChanges = true;
    }

    public void OnApplyButtonClick()
    {
        Screen.fullScreen = gameSettings.Fullscreen;
        Screen.SetResolution(resolutions[gameSettings.resolutionIndex].width, resolutions[gameSettings.resolutionIndex].height, gameSettings.Fullscreen);
        QualitySettings.vSyncCount = gameSettings.vSync;
        AudioListener.volume = gameSettings.musicVolume;
        SaveSettings();
    }
    public void SaveSettings()
    {
        Debug.Log("settings saved");
        string jsonData = JsonUtility.ToJson(gameSettings, true);
        File.WriteAllText(Application.persistentDataPath + "/gameSettings.json", jsonData);
        print(Application.persistentDataPath);
    }

    public void LoadSettings()
    {
        if (File.Exists(Application.persistentDataPath + "/gameSettings.json"))
        {
            gameSettings = JsonUtility.FromJson<Settings>(File.ReadAllText(Application.persistentDataPath + "/gameSettings.json"));
            masterVolumeSlider.value = gameSettings.musicVolume;
            //AAdropdown.value = (int)Mathf.Sqrt(gameSettings.antiAliasing);
            //textureQualityDropdown.value = textureQualityDropdown.options.Count + gameSettings.textureQuality;
            resolutionDropdwon.value = gameSettings.resolutionIndex;
            fullscreenToggle.isOn = gameSettings.Fullscreen;
            Debug.Log("Settinsg loaded");

            resolutionDropdwon.RefreshShownValue();


            OnMasterVolumeChange();
            onFullScreenToggle();
            onResolutionChange();
            //onAntialiasingChange();
            //onTextureQualityChange();
        }
    }
}
