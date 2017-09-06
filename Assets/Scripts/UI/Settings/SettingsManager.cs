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
    { gameSettings.Fullscreen = Screen.fullScreen = fullscreenToggle.isOn; }

    public void onResolutionChange()
    {
        Screen.SetResolution(resolutions[resolutionDropdwon.value].width, resolutions[resolutionDropdwon.value].height, gameSettings.Fullscreen);
        gameSettings.resolutionIndex = resolutionDropdwon.value;
    }

    public void onTextureQualityChange()
    {
        QualitySettings.masterTextureLimit = gameSettings.textureQuality = textureQualityDropdown.options.Count - textureQualityDropdown.value;
    }

    public void onAntialiasingChange()
    {
        QualitySettings.antiAliasing = gameSettings.antiAliasing = (int)AAdropdown.value * (int)AAdropdown.value;
    }

    public void onVsyncChange()
    {
        QualitySettings.vSyncCount = gameSettings.vSync = vSyncDrop.value;
    }

    public void OnMasterVolumeChange()
    {
        AudioListener.volume = gameSettings.musicVolume = masterVolumeSlider.value;
    }

    public void OnApplyButtonClick()
    {
        SaveSettings();
    }
    public void SaveSettings()
    {
        Debug.Log("settings saved");
        string jsonData = JsonUtility.ToJson(gameSettings, true);
        File.WriteAllText(Application.persistentDataPath + "/gameSettings.json", jsonData);
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
