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
    float aerialMultiplier = .8f;
    float comboMultiplier = 1.5f;

    [Header("Player Animation")]
    public Animator m_Animator;
    public bool isSlashing = false;

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

        if (Input.GetButton("Fire1"))
        {
            if (currentWeapon == 1)
            {
                // if (attackOnCooldown)
                //     return;
                m_Animator.SetTrigger("Slash");
                isSlashing = true;
                m_Animator.SetBool("isSlashing", isSlashing);
                // sword.SetActive(true);
                // StartCoroutine(AttackCooldown());
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
        else
        {
            isSlashing = false;
            m_Animator.SetBool("isSlashing", isSlashing);
        }
        // if (isSlashing)
        // {
        //     sword.SetActive(true);
        // }
        // else
        // {
        //     sword.SetActive(false);
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

        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Hit");
            if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Action-Slash-Aerial"))
                other.gameObject.GetComponent<Health>().Damage(swordDamage * aerialMultiplier);
            else if (m_Animator.GetInteger("ComboState") == 3)
                other.gameObject.GetComponent<Health>().Damage(swordDamage * comboMultiplier);
            else
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
    // public IEnumerator AttackCooldown()
    // {
    //     yield return new WaitForSeconds(0.1f);

    //     attackOnCooldown = true;
    //     yield return new WaitForSeconds(attackCooldown);
    //     attackOnCooldown = false;
    // }

    public void PlayerHit(GameObject other, EnemyData enemyData, float facingDirection)
    {
        m_Animator.Play("Action-Hurt");
        // StartCoroutine(InvincibiltyFrames());
        other.GetComponent<PlayerScript>().Damage(enemyData.attackPower);
        StartCoroutine(FakeAddForceMotion(other, enemyData, -facingDirection));
    }
    public IEnumerator FakeAddForceMotion(GameObject target, EnemyData enemyData, float facingDirection)
    {
        float i = 0.01f;
        Rigidbody2D rb = target.GetComponent<Rigidbody2D>();
        Vector2 raycastDirection = new Vector2(-facingDirection, 0); // Raycast direction opposite to the facing direction
        float raycastDistance = .5f; // Adjust the distance as needed
        int worldLayerMask = LayerMask.GetMask("World"); // Create a layer mask for the "world" layer

        int numberOfRaycasts = 10; // Number of raycast lines
        float padding = 0.1f; // Padding to avoid detecting the ground
        Collider2D collider = target.GetComponent<Collider2D>(); // Get the Collider2D component
        float colliderHeight = collider.bounds.size.y; // Get the height of the collider
        float raycastSpacing = (colliderHeight - 2 * padding) / (numberOfRaycasts - 1); // Calculate the spacing between each raycast line

        while (enemyData.knockbackForce >= i)
        {
            bool obstacleDetected = false;

            for (int j = 0; j < numberOfRaycasts; j++)
            {
                // Adjust the raycast origin for each raycast line
                Vector2 raycastOrigin = new Vector2(target.transform.position.x, collider.bounds.min.y + padding + j * raycastSpacing);
                RaycastHit2D hit = Physics2D.Raycast(raycastOrigin, raycastDirection, raycastDistance, worldLayerMask);

                // Draw the raycast line
                Debug.DrawLine(raycastOrigin, raycastOrigin + raycastDirection * raycastDistance, Color.red);

                if (hit.collider != null)
                {
                    // Obstacle detected, stop the player's velocity
                    Debug.Log("Obstacle detected");
                    rb.linearVelocity = Vector2.zero;
                    obstacleDetected = true;
                    break;
                }
            }

            if (obstacleDetected)
            {
                yield break;
            }

            // Apply the knockback force
            rb.linearVelocity = new Vector2(enemyData.knockbackForce / i * -facingDirection, 0.1f); // For X axis positive force
            i += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        rb.linearVelocity = Vector2.zero;
        yield return null;
    }

    public IEnumerator FakeAddForceMotionEnemy(GameObject target, EnemyData enemyData, float facingDirection)
    {
        // float i = 0.01f;
        // while (swordKnockback.x >= i)
        // {
        //     if (target == null)
        //         yield break;

        //     target.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(swordKnockback.x * enemyData.mass / i * facingDirection, swordKnockback.y); // !! For X axis positive force
        //     i += Time.deltaTime;
        //     yield return new WaitForEndOfFrame();

        // }
        if (target == null)
            yield break;
        target.GetComponent<Rigidbody2D>().AddForce(new Vector2(swordKnockback.x * facingDirection, swordKnockback.y), ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.1f);
        if (target == null)
            yield break;
        target.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        yield return null;
    }

    // public IEnumerator InvincibiltyFrames()
    // {
    //     SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
    //     instance.isInvincible = true;
    //     float blinkInterval = 0.1f; // Adjust the blink interval as needed
    //     float elapsedTime = 0f;

    //     while (elapsedTime < instance.invincibilityTime)
    //     {
    //         spriteRenderer.color = Color.white;
    //         yield return new WaitForSeconds(blinkInterval);
    //         spriteRenderer.color = Color.red;
    //         yield return new WaitForSeconds(blinkInterval);
    //         elapsedTime += 2 * blinkInterval;
    //     }

    //     instance.isInvincible = false;
    //     spriteRenderer.color = Color.white;
    // }

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
