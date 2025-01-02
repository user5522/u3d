using UnityEngine;

public class Pause : MonoBehaviour
{
    public GameObject pauseMenu;

    private float previousTimeScale;

    void Start() => previousTimeScale = Time.timeScale;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P)) TogglePause();
    }

    public void TogglePause()
    {
        if (Time.timeScale > 0 && !GameManager.Instance.isGameOver)
        {
            previousTimeScale = Time.timeScale;
            Time.timeScale = 0;
            AudioListener.pause = true; // for audio whenever that gets added
            pauseMenu.SetActive(true);

            GameManager.Instance.isPaused = true;
        }
        else if (Time.timeScale == 0 && !GameManager.Instance.isGameOver)
        {
            Time.timeScale = previousTimeScale;
            AudioListener.pause = false; // for audio whenever that gets added
            pauseMenu.SetActive(false);

            GameManager.Instance.isPaused = false;
        }
    }

    public void QuitGame() => Application.Quit();
}