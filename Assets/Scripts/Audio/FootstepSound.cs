using UnityEngine;

public class FootstepSound : MonoBehaviour
{
    [Header("References")]
    public AudioSource audioSource;
    public AudioClip[] footstepSounds;
    public float walkStepInterval = 0.5f;
    public float runStepInterval = 0.3f;
    private float stepTimer;

    public float minPitch;
    public float maxPitch;

    private PlayerController playerController;
    private SlowDownTime slowDownTime;
    private bool isSlowMotion;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        playerController = GetComponent<PlayerController>();
        slowDownTime = GetComponent<SlowDownTime>();
        stepTimer = walkStepInterval;
    }

    void Update()
    {
        isSlowMotion = slowDownTime.isSlowMotion;
        if (isSlowMotion) audioSource.pitch = AudioManager.Instance.slowMotionPitch;
        else audioSource.pitch = AudioManager.Instance.normalPitch;

        if ((Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0) &&
            (playerController.grounded || playerController.wallrunning) &&
            playerController.state != PlayerController.MovementState.sliding &&
            playerController.state != PlayerController.MovementState.air)
        {
            stepTimer -= Time.deltaTime;

            bool isRunning = (playerController.state == PlayerController.MovementState.sprinting || playerController.state == PlayerController.MovementState.wallRunningUp);
            float currentStepInterval = isRunning ? runStepInterval : walkStepInterval;

            if (stepTimer <= 0f)
            {
                PlayFootstepSound();
                stepTimer = currentStepInterval;
            }
        }
    }

    void PlayFootstepSound()
    {
        int randomIndex = Random.Range(0, footstepSounds.Length);
        audioSource.clip = footstepSounds[randomIndex];

        audioSource.pitch = Random.Range(minPitch, maxPitch);

        audioSource.PlayOneShot(audioSource.clip);
    }
}