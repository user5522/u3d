using UnityEngine;
using UnityEngine.UI;

public class PlayerBehaviour : MonoBehaviour
{
    public HealthBar healthBar;
    public Image gameOverScreen;

    public void PlayerTakeDmg(int dmgAmount)
    {
        GameManager.gameManager.playerHealth.DmgUnit(dmgAmount);
        healthBar.SetHealth(GameManager.gameManager.playerHealth.Health);

        if (GameManager.gameManager.playerHealth.Health <= 0)
            gameOverScreen.gameObject.SetActive(true);

    }
    public void PlayerHeal(int healing)
    {
        GameManager.gameManager.playerHealth.HealUnit(healing);
        healthBar.SetHealth(GameManager.gameManager.playerHealth.Health);
    }
}