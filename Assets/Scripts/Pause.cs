using UnityEngine;

public class Pause : MonoBehaviour
{
    public GameObject pauseMenu;

    private float previousTimeScale;

    public static bool isPaused = false;

    void Start() => previousTimeScale = Time.timeScale;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P)) TogglePause();
    }

    public void TogglePause()
    {
        if (Time.timeScale > 0)
        {
            previousTimeScale = Time.timeScale;
            Time.timeScale = 0;
            AudioListener.pause = true; // for audio whenever that gets added
            pauseMenu.SetActive(true);

            isPaused = true;
        }
        else if (Time.timeScale == 0)
        {
            Time.timeScale = previousTimeScale;
            AudioListener.pause = false; // for audio whenever that gets added
            pauseMenu.SetActive(false);

            isPaused = false;
        }
    }
}