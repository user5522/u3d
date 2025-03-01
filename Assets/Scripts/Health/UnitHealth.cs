public class UnitHealth
{
    int currentHealth;
    int currentMaxHealth;

    public int Health
    {
        get => currentHealth;
        set => currentHealth = value;
    }
    public int MaxHealth
    {
        get => currentMaxHealth;
        set => currentMaxHealth = value;
    }

    public UnitHealth(int health, int maxHealth)
    {
        currentHealth = health;
        currentMaxHealth = maxHealth;
    }

    public void DmgUnit(int dmgAmount)
    {
        if (currentHealth > 0) currentHealth -= dmgAmount;
    }

    public void HealUnit(int healAmount)
    {
        if (currentHealth < MaxHealth) currentHealth += healAmount;
        if (currentHealth > MaxHealth) currentHealth = MaxHealth;
    }
}