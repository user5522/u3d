using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SlowDownTime : MonoBehaviour
{
    public float slowdownFactor;
    public float maxSlowdownTime;
    public float rechargeRate;
    public Image bar;
    public Material slowMotionMaterial;
    public Volume postProcessVolume;
    public float chromaticAberrationIntensity = 1f;
    public float dimmingIntensity = -0.5f;

    [HideInInspector] public bool isSlowMotion = false;

    private float availableSlowdownTime;
    private float lastRechargeTime;
    private ChromaticAberration chromaticAberration;
    private ColorAdjustments colorAdjustments;

    void Start()
    {
        availableSlowdownTime = maxSlowdownTime;
        if (postProcessVolume.profile.TryGet(out ChromaticAberration chromaticAberrationEffect))
            chromaticAberration = chromaticAberrationEffect;
        if (postProcessVolume.profile.TryGet(out ColorAdjustments colorAdjustmentsEffect))
            colorAdjustments = colorAdjustmentsEffect;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && !GameManager.Instance.isPaused) ToggleSlowMotion();
        if (isSlowMotion && !GameManager.Instance.isPaused)
        {
            availableSlowdownTime -= Time.unscaledDeltaTime;
            if (availableSlowdownTime <= 0) ToggleSlowMotion();
        }
        else if (!GameManager.Instance.isPaused) RechargeSlowdownTime();
        UpdateUI();
        HandleGamePaused();
    }

    void RechargeSlowdownTime()
    {
        float timeSinceLastRecharge = Time.time - lastRechargeTime;
        float rechargeAmount = timeSinceLastRecharge * rechargeRate;
        availableSlowdownTime = Mathf.Min(availableSlowdownTime + rechargeAmount, maxSlowdownTime);
        lastRechargeTime = Time.time;
    }

    void ToggleSlowMotion()
    {
        if (!isSlowMotion && availableSlowdownTime > 0)
        {
            isSlowMotion = true;
            Time.timeScale = slowdownFactor;
            AudioManager.Instance.SetPitch(AudioManager.Instance.slowMotionPitch);
            slowMotionMaterial.SetFloat("_SlowMotionIntensity", 1);
            SetChromaticAberration(true);
            SetDimming(true);
        }
        else if (isSlowMotion)
        {
            isSlowMotion = false;
            Time.timeScale = 1f;
            AudioManager.Instance.SetPitch(AudioManager.Instance.normalPitch);
            lastRechargeTime = Time.time;
            slowMotionMaterial.SetFloat("_SlowMotionIntensity", 0);
            SetChromaticAberration(false);
            SetDimming(false);
        }
    }

    void SetChromaticAberration(bool enabled)
    {
        if (enabled) chromaticAberration.intensity.Override(chromaticAberrationIntensity);
        chromaticAberration.active = enabled;
    }

    void SetDimming(bool enabled)
    {
        colorAdjustments.active = enabled;

        if (enabled)
        {
            colorAdjustments.postExposure.Override(dimmingIntensity);
            Shader.SetGlobalFloat("_GlobalDimmingIntensity", -dimmingIntensity);
        }
        else
        {
            colorAdjustments.postExposure.Override(0f);
            Shader.SetGlobalFloat("_GlobalDimmingIntensity", 0f);
        }
    }

    void HandleGamePaused()
    {
        if (GameManager.Instance.isPaused && isSlowMotion) Time.timeScale = 0f;
    }

    void UpdateUI() => bar.fillAmount = availableSlowdownTime / maxSlowdownTime;
}