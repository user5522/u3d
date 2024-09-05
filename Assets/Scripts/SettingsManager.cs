using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    private static bool IsPaused = false;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start()
    {
        LoadSettings();
        UpdateCursorVisibility();
    }

    private void Update() => UpdateCursorVisibility();

    private void LoadSettings()
    {
        // Quality
        int qualityIndex = PlayerPrefs.GetInt("QualityIndex", 3);
        QualitySettings.SetQualityLevel(qualityIndex);

        // VSync
        bool vSyncOn = PlayerPrefs.GetInt("VSync", 1) == 1;
        QualitySettings.vSyncCount = vSyncOn ? 1 : 0;

        // Max FPS
        float maxFPS = PlayerPrefs.GetFloat("MaxFPS", 60f);
        Application.targetFrameRate = (int)maxFPS;

        // Resolution
        int resolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", 0);
        Resolution[] resolutions = Screen.resolutions;
        if (resolutionIndex < resolutions.Length)
        {
            Resolution resolution = resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode);
        }
    }

    private void UpdateCursorVisibility()
    {
        Cursor.visible = IsPaused;
        Cursor.lockState = IsPaused ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public void SetPauseState(bool paused) => IsPaused = paused;
    public bool GetPauseState() => IsPaused;
}