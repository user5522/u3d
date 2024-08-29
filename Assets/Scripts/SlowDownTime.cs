using UnityEngine;
using UnityEngine.UI;

public class SlowDownTime : MonoBehaviour
{
    public float slowdownFactor;
    public float maxSlowdownTime;
    public float rechargeRate;
    public Image bar;
    public Material slowMotionMaterial;

    private bool isSlowMotion = false;
    private float availableSlowdownTime;
    private float lastRechargeTime;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) ToggleSlowMotion();
        if (isSlowMotion)
        {
            availableSlowdownTime -= Time.unscaledDeltaTime;
            if (availableSlowdownTime <= 0) ToggleSlowMotion();
        }
        else RechargeSlowdownTime();
        UpdateUI();
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

    void UpdateUI() => bar.fillAmount = availableSlowdownTime / maxSlowdownTime;
}