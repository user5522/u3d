using UnityEngine;
using UnityEngine.UI;

public class SlowDownTime : MonoBehaviour
{
    public float slowdownFactor;
    public float maxSlowdownTime;
    public float rechargeRate;
    public Image bar;
    public Material slowMotionMaterial;

    [HideInInspector] public bool isSlowMotion = false;

    private float availableSlowdownTime;
    private float lastRechargeTime;
    private float previousTimeScale;


    void Start() => availableSlowdownTime = maxSlowdownTime;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && !Pause.isPaused) ToggleSlowMotion();
        if (isSlowMotion && !Pause.isPaused)
        {
            availableSlowdownTime -= Time.deltaTime;
            if (availableSlowdownTime <= 0) ToggleSlowMotion();
        }
        else if (!Pause.isPaused) RechargeSlowdownTime();
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
            slowMotionMaterial.SetFloat("_SlowMotionIntensity", 1);
        }
        else if (isSlowMotion)
        {
            isSlowMotion = false;
            Time.timeScale = 1f;
            lastRechargeTime = Time.time;
            slowMotionMaterial.SetFloat("_SlowMotionIntensity", 0);
        }
    }

    void HandleGamePaused()
    {
        if (Pause.isPaused && isSlowMotion) Time.timeScale = 0f;
    }

    void UpdateUI() => bar.fillAmount = availableSlowdownTime / maxSlowdownTime;
}