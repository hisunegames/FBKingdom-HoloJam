using UnityEngine;

public class Health : MonoBehaviour
{
    public EnemyData enemyData;
    public float currentHealth;

    public HealthSystem healthBar;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = enemyData.maxHealth;
        healthBar.SetMaxHealth(enemyData.maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     Damage(20);
        // }
    }

    public void Damage(float damage)
    {
        currentHealth -= damage;

        healthBar.SetHealth(currentHealth);
    }
}
