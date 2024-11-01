using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public event Action<float, float> EventHealthUpdate;

    public EnemyData enemyData;
    public float currentHealth;

    public HealthSystem healthBar;
    public Animator animator;
    public GameObject damageDisplay;
    public UnityEvent OnDeath;
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
            if (animator != null)
            {
                // Debug.Log("Enemy died");
                gameObject.GetComponent<Collider2D>().enabled = false;
                gameObject.GetComponent<Rigidbody2D>().linearVelocity = new(0, 0);
                gameObject.GetComponent<Rigidbody2D>().simulated = false;
                OnDeath?.Invoke();
                if (enemyData.name != "fbkBoss")
                    animator.Play("Death");
            }
            // else
            //     Destroy(gameObject);
        }
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     Damage(20);
        // }
    }

    public void Damage(float damage)
    {
        if (gameObject == null)
            return;
        currentHealth -= damage;

        // Instantiate the damage display
        Canvas canvas = GetComponentInChildren<Canvas>();
        GameObject _display = Instantiate(damageDisplay, new(0, 0), Quaternion.identity);

        // Find the canvas component in the current game object

        if (canvas != null)
        {
            // Set the canvas as the parent of the instantiated display
            _display.transform.SetParent(canvas.transform, false);
            _display.transform.GetComponent<RectTransform>().localPosition = new(15, 30);
        }

        // Set the text of the damage display
        _display.GetComponent<TMP_Text>().text = $"{{size a=1}}<shake a=0.5>{damage}</shake>{{/size}}";
        _display.GetComponent<Rigidbody2D>().AddForce(new(0.1f * Mathf.Sign(GetComponent<EnemyAI>().facingDirection), 0.5f));

        // Update the health bar
        healthBar.SetHealth(currentHealth);

        EventHealthUpdate?.Invoke(currentHealth, enemyData.maxHealth);
    }
}
