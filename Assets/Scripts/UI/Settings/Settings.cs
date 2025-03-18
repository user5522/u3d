using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class Settings : MonoBehaviour
{
    [Header("General UI")]
    public Toggle vSyncToggle;
    public Toggle fullScreenToggle;
    public Slider maxFPSSlider;
    public TMP_Text maxFPSText;
    public TMP_Dropdown qualityDropdown;
    public TMP_Dropdown resolutionDropdown;

    private FullScreenMode fullScreenMode = FullScreenMode.FullScreenWindow;

    private void Awake()
    {
        LoadSettings();

        vSyncToggle.onValueChanged.AddListener(ToggleVSync);
        fullScreenToggle.onValueChanged.AddListener(ToggleFullScreen);
        maxFPSSlider.onValueChanged.AddListener(SetMaxFPS);
        qualityDropdown.onValueChanged.AddListener(SetQuality);
        resolutionDropdown.onValueChanged.AddListener(SetResolution);

        SetupQualityDropdown();
        SetupResolutionDropdown();
    }

    private void SetupResolutionDropdown()
    {
        resolutionDropdown.ClearOptions();
        Resolution[] resolutions = Screen.resolutions;
        int maxResolutions = Mathf.Min(resolutions.Length, 20);
        for (int i = 0; i < maxResolutions; i++)
        {
            Resolution resolution = resolutions[i];
            TMP_Dropdown.OptionData option = new(
                string.Format("{0}x{1}", resolution.width, resolution.height)
            );
            resolutionDropdown.options.Add(option);
        }

        int savedResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", 0);
        resolutionDropdown.value = savedResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    private void SetupQualityDropdown()
    {
        int savedQualityIndex = PlayerPrefs.GetInt("QualityIndex", 3);
        qualityDropdown.value = savedQualityIndex;
        qualityDropdown.RefreshShownValue();
    }

    private void LoadSettings()
    {
        bool vSyncOn = PlayerPrefs.GetInt("VSync", 1) == 1;
        vSyncToggle.isOn = vSyncOn;
        QualitySettings.vSyncCount = vSyncOn ? 1 : 0;

        bool fullscreen = PlayerPrefs.GetString("Fullscreen", "FullScreenWindow") == "FullScreenWindow";
        fullScreenToggle.isOn = fullscreen;
        Screen.SetResolution(Screen.width, Screen.height, fullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed);

        float maxFPS = PlayerPrefs.GetFloat("MaxFPS", 60f);
        maxFPSSlider.value = maxFPS;
        Application.targetFrameRate = (int)maxFPS;
        maxFPSText.text = "Max Framerate - " + maxFPS.ToString();

        int resolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", 0);
        SetResolution(resolutionIndex);

        int qualityIndex = PlayerPrefs.GetInt("QualityIndex", 3);
        SetQuality(qualityIndex);
    }

    private void ToggleFullScreen(bool isOn)
    {
        fullScreenMode = isOn ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        Screen.SetResolution(Screen.width, Screen.height, fullScreenMode);
        PlayerPrefs.SetString("Fullscreen", isOn ? "FullScreenWindow" : "Windowed");
        PlayerPrefs.Save();
    }

    private void ToggleVSync(bool isOn)
    {
        QualitySettings.vSyncCount = isOn ? 1 : 0;
        PlayerPrefs.SetInt("VSync", isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void SetMaxFPS(float value)
    {
        Application.targetFrameRate = (int)value;
        maxFPSText.text = "FPS Limiter - " + value.ToString();
        PlayerPrefs.SetFloat("MaxFPS", value);
        PlayerPrefs.Save();
    }

    private void SetResolution(int index)
    {
        Resolution[] resolutions = Screen.resolutions;
        Screen.SetResolution(resolutions[index - 1].width, resolutions[index - 1].height, fullScreenMode);
        PlayerPrefs.SetInt("ResolutionIndex", index);
        PlayerPrefs.Save();
    }

    private void SetQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);
        PlayerPrefs.SetInt("QualityIndex", index);
        PlayerPrefs.Save();
    }
}