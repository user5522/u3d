using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public UnitHealth playerHealth = new UnitHealth(100, 100);

    bool waiting;
    float previousTimescale;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    public void HitStop(float duration)
    {
        if (waiting) return;
        previousTimescale = Time.timeScale;
        Time.timeScale = 0;
        StartCoroutine(Wait(duration));
    }

    IEnumerator Wait(float duration)
    {
        waiting = true;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = previousTimescale;
        waiting = false;
    }
}