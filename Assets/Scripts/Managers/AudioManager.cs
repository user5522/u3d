using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public float slowMotionPitch;
    public float normalPitch;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    public void SetPitch(float pitch) => AudioListener.volume = pitch;
}