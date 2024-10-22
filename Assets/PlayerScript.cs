using System.Collections;
using DYP;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public static PlayerScript instance;
    public BasicMovementController2D m_Controller;

    [Header("PlayerData")]
    public float maxHealth = 100;
    public float playerHealth;

    [Header("Sword")]
    public GameObject swordLeft;
    public GameObject swordRight;

    public Vector2 swordKnockback;
    public float swordDamage;

    public float attackCooldown = 0.25f;
    private bool attackOnCooldown = false;
    void Awake()
    {
        if (instance == null)
        {

            instance = this;
        }
        swordLeft = transform.GetChild(0).gameObject;
        swordRight = transform.GetChild(1).gameObject;
        swordLeft.SetActive(false);
        swordRight.SetActive(false);
    }
    void Start()
    {
        playerHealth = maxHealth;
    }
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (attackOnCooldown)
                return;
            if (m_Controller.FacingDirection == 1)
                swordRight.SetActive(true);
            else
                swordLeft.SetActive(true);
            StartCoroutine(AttackCooldown());
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            swordRight.SetActive(false);
            swordLeft.SetActive(false);
        }

    }

    public void Damage(float damage)
    {
        playerHealth -= damage;
        if (playerHealth <= 0)
        {
            Debug.Log("Player Dead");
        }
        else
        {
            Debug.Log("Player Health: " + playerHealth);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.gameObject.CompareTag("Enemy") && swordRight.activeSelf)
        {
            Debug.Log("Hit");
            other.gameObject.GetComponent<Health>().Damage(swordDamage);
            other.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(swordKnockback.x * m_Controller.FacingDirection, swordKnockback.y), ForceMode2D.Impulse);
        }
        else if (other.gameObject.CompareTag("Enemy") && swordLeft.activeSelf)
        {
            Debug.Log("Hit");
            other.gameObject.GetComponent<Health>().Damage(swordDamage);
            other.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(swordKnockback.x * m_Controller.FacingDirection, swordKnockback.y), ForceMode2D.Impulse);
        }
    }
    IEnumerator AttackCooldown()
    {
        attackOnCooldown = true;
        yield return new WaitForSeconds(attackCooldown);
        attackOnCooldown = false;
    }

}
