using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    public GameObject player;
    public TextMeshProUGUI text;
    public float appearingTime;
    public float disappearingTime;

    void Start()
    {
        GameManager.Instance.isGameOver = true;
        player.SetActive(false);
        StartCoroutine(FlashText());
        Time.timeScale = 1;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            GameManager.Instance.isGameOver = false;
            gameObject.SetActive(false);
            player.SetActive(true);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private IEnumerator FlashText()
    {
        while (true)
        {
            text.gameObject.SetActive(true);
            yield return new WaitForSeconds(appearingTime);

            text.gameObject.SetActive(false);
            yield return new WaitForSeconds(disappearingTime);
        }
    }
}