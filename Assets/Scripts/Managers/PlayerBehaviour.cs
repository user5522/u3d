using UnityEngine;
using UnityEngine.UI;

public class PlayerBehaviour : MonoBehaviour
{
    public HealthBar healthBar;
    public Image gameOverScreen;

    public void PlayerTakeDmg(int dmgAmount)
    {
        GameManager.Instance.playerHealth.DmgUnit(dmgAmount);
        healthBar.SetHealth(GameManager.Instance.playerHealth.Health);

        if (GameManager.Instance.playerHealth.Health <= 0)
            gameOverScreen.gameObject.SetActive(true);

    }
    public void PlayerHeal(int healing)
    {
        GameManager.Instance.playerHealth.HealUnit(healing);
        healthBar.SetHealth(GameManager.Instance.playerHealth.Health);
    }
}