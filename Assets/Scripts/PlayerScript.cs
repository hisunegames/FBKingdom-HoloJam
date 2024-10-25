using System;
using System.Collections;
using DYP;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public static PlayerScript instance;
    public CustomMovementController2D m_Controller;

    [Header("PlayerData")]
    public float maxHealth = 100;
    public float playerHealth;
    public float invincibilityTime = .2f;
    public float invincibilityPadding = .2f;
    public bool isInvincible = false;
    public float playerSpeedMultipler = 1;
    public float multipler = 1.5f;
    int originalLayer;

    [Header("Player Animation")]
    public Animator m_Animator;

    [Header("Equipment")]
    public int currentWeapon = 1;
    //sword main (1) gun sub (2)

    [Header("Sword")]
    // public GameObject swordLeft;
    public GameObject sword;

    public Vector2 swordKnockback;
    public float swordDamage;

    public float attackCooldown = 0.25f;
    private bool attackOnCooldown = false;

    [Header("Gun")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireCooldown = 1f;
    private float nextFireTime = 0f;
    private ParticleSystem.EmissionModule emission;


    void Awake()
    {
        if (instance == null)
        {

            instance = this;
        }
        // swordLeft = transform.GetChild(0).gameObject;
        // swordLeft.SetActive(false);
        sword.SetActive(false);
    }
    void Start()
    {
        playerHealth = maxHealth;
        m_Animator = GetComponent<Animator>();
        emission = gameObject.GetComponentInChildren<ParticleSystem>().emission;
        originalLayer = gameObject.layer;
        m_Controller.OnDash += OnDash;
        m_Controller.OnDashEnd += OnDashEnd;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentWeapon = 1;
            m_Animator.SetBool("ShootReady", false);
            m_Animator.SetInteger("ActionState", 0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentWeapon = 2;
            m_Animator.SetBool("ShootReady", true);
            m_Animator.SetInteger("ActionState", 1);
        }

        if (Input.GetButtonDown("Fire1"))
        {
            if (currentWeapon == 1)
            {
                if (attackOnCooldown)
                    return;
                sword.SetActive(true);
                StartCoroutine(AttackCooldown());
            }
            else if (currentWeapon == 2 && Time.time >= nextFireTime)
            {
                Shoot();
                m_Animator.SetTrigger("Fire");
                nextFireTime = Time.time + fireCooldown;
            }
            // if (m_Controller.FacingDirection == 1)
            // else
            //     swordLeft.SetActive(true);
        }
        // else if (Input.GetButtonUp("Fire1"))
        // {
        //     if (sword.activeSelf)
        //         sword.SetActive(false);
        //     // swordLeft.SetActive(false);
        // }

        if (!gameObject.GetComponent<Animator>().GetBool("isGrounded"))
        {
            emission.enabled = false;
        }
        else
        {
            emission.enabled = true;

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

        if (other.gameObject.CompareTag("Enemy") && sword.activeSelf)
        {
            Debug.Log("Hit");
            other.gameObject.GetComponent<Health>().Damage(swordDamage);
            StartCoroutine(FakeAddForceMotionEnemy(other.gameObject, other.GetComponent<EnemyAI>().enemyData, m_Controller.FacingDirection));
            // other.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(swordKnockback.x * m_Controller.FacingDirection * other.gameObject.GetComponent<Rigidbody2D>().mass, swordKnockback.y), ForceMode2D.Impulse);
        }
        // else if (other.gameObject.CompareTag("Enemy") && swordLeft.activeSelf)
        // {
        //     Debug.Log("Hit");
        //     other.gameObject.GetComponent<Health>().Damage(swordDamage);
        //     other.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(swordKnockback.x * m_Controller.FacingDirection, swordKnockback.y), ForceMode2D.Impulse);
        // }
    }
    IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(0.1f);
        sword.SetActive(false);
        attackOnCooldown = true;
        yield return new WaitForSeconds(attackCooldown);
        attackOnCooldown = false;
    }

    public void PlayerHit(GameObject other, EnemyData enemyData, float facingDirection)
    {
        StartCoroutine(InvincibiltyFrames());
        other.GetComponent<PlayerScript>().Damage(enemyData.attackPower);
        StartCoroutine(FakeAddForceMotion(other, enemyData, -facingDirection));
    }
    public IEnumerator FakeAddForceMotion(GameObject target, EnemyData enemyData, float facingDirection)
    {
        float i = 0.01f;
        while (enemyData.knockbackForce >= i)
        {
            target.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(enemyData.knockbackForce / i * -facingDirection, 0.1f); // !! For X axis positive force
            i += Time.deltaTime;
            yield return new WaitForEndOfFrame();

        }
        target.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        yield return null;
    }

    public IEnumerator FakeAddForceMotionEnemy(GameObject target, EnemyData enemyData, float facingDirection)
    {
        float i = 0.01f;
        while (swordKnockback.x >= i)
        {
            if (target == null)
                yield break;
            target.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(swordKnockback.x * enemyData.mass / i * facingDirection, target.GetComponent<Rigidbody2D>().linearVelocity.y); // !! For X axis positive force
            i += Time.deltaTime;
            yield return new WaitForEndOfFrame();

        }
        if (target == null)
            yield break;
        target.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        yield return null;
    }

    public IEnumerator InvincibiltyFrames()
    {
        GetComponent<SpriteRenderer>().color = Color.red;
        instance.isInvincible = true;
        yield return new WaitForSeconds(instance.invincibilityTime);
        isInvincible = false;
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    public void OnDash(int direction)
    {
        StopCoroutine(DashIFramesPadding());
        // Change the player's layer to the dash layer
        gameObject.layer = LayerMask.NameToLayer("DashLayer");

        instance.isInvincible = true;

        // Revert the player's layer back to the original layer
    }
    private void OnDashEnd()
    {
        StopCoroutine(DashIFramesPadding());
        StartCoroutine(DashIFramesPadding());
    }

    public IEnumerator DashIFramesPadding()
    {
        // Store the original layer
        // playerSpeedMultipler = multipler;
        yield return new WaitForSeconds(invincibilityPadding);
        // playerSpeedMultipler = 1;
        gameObject.layer = originalLayer;

        isInvincible = false;

    }

    void Shoot()
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }
}
