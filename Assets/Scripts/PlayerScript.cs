using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerScript : MonoBehaviour
{
    public event Action<float, float> EventHealthUpdate;
    public event Action<float, float> EventManaUpdate;
    public event Action EventGameOver;

    public static PlayerScript instance;
    public CustomMovementController2D m_Controller;

    [Header("PlayerData")]
    public PlayerStatsSO Stats;
    public float maxHealth = 100;
    public float playerHealth;
    public float maxMana = 100;
    public float playerMana;
    public float invincibilityPadding = .2f;
    public bool isInvincible = false;
    public float playerSpeedMultipler = 1; //TODO: Implement is needed.
    int originalLayer;
    float aerialMultiplier = .8f;
    float comboMultiplier = 1.5f;

    private PlayerBuff currentBuff;
    private bool isFlashing;

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
    public float swordDamage = 20;

    private float attackCooldown = 0.25f;
    private bool attackOnCooldown = false;

    [Header("Gun")]
    public Bullet bulletPrefab;
    public Transform firePoint;
    public float gunDamage = 10; //TODO: Implement is needed.
    public float fireCooldown = 1f;
    private float nextFireTime = 0f;
    private ParticleSystem.EmissionModule emission;

    [Header("SFX")]
    [SerializeField] private AudioSource slashSound;
    [SerializeField] private AudioSource shootSound;
    [SerializeField] private AudioSource jumpSound;
    [SerializeField] private AudioSource hurtSound;
    [SerializeField] private AudioSource dashSound;

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
        InitialStats();

        m_Animator = GetComponent<Animator>();
        emission = gameObject.GetComponentInChildren<ParticleSystem>().emission;
        originalLayer = gameObject.layer;
        m_Controller.OnDash += OnDash;
        m_Controller.OnDashEnd += OnDashEnd;
        m_Controller.OnJump += OnJump;
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
                m_Animator.SetTrigger("Slash");
                isSlashing = true;
                m_Animator.SetBool("isSlashing", isSlashing);

                PlaySound(slashSound);
            }
            else if (currentWeapon == 2 && Time.time >= nextFireTime)
            {
                Shoot();
                m_Animator.SetTrigger("Fire");
                nextFireTime = Time.time + fireCooldown;
            }
        }
        else
        {
            isSlashing = false;
            m_Animator.SetBool("isSlashing", isSlashing);
        }

        if (!gameObject.GetComponent<Animator>().GetBool("isGrounded"))
        {
            emission.enabled = false;
        }
        else
        {
            emission.enabled = true;

        }

        CheckMana();
    }

    private void OnDestroy()
    {
        EventHealthUpdate = null;
        EventManaUpdate = null;
    }

    private void InitialStats()
    {
        maxHealth = Stats.MaxHealth;
        playerHealth = maxHealth;

        maxMana = Stats.MaxMana;
        playerMana = maxMana;

        playerSpeedMultipler = Stats.SpeedMultiplier;

        swordDamage = Stats.SwordDamage;

        gunDamage = Stats.GunDamage;

        currentBuff = PlayerStatsHandler.GetPlayerBuff();
    }

    private void UpdateStats()
    {
        currentBuff = PlayerStatsHandler.GetPlayerBuff();

        maxHealth = Stats.MaxHealth + currentBuff.MaxHealth * Stats.BuffMaxHealth;
        playerSpeedMultipler = Stats.SpeedMultiplier + currentBuff.Speed * Stats.BuffSpeedMultiplier;
        swordDamage = Stats.SwordDamage + currentBuff.SwordDamage * Stats.BuffSwordDamage;
        gunDamage = Stats.GunDamage + currentBuff.GunDamage * Stats.BuffGunDamage;

        playerHealth = maxHealth;

        EventHealthUpdate?.Invoke(playerHealth, maxMana);
    }

    public void Damage(float damage)
    {
        playerHealth -= damage;
        EventHealthUpdate?.Invoke(playerHealth, maxHealth);

        PlaySound(hurtSound);

        if (playerHealth > 0) return;

        OnGameOver();

        m_Controller.enabled = false;
        enabled = false;
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
        StartCoroutine(FlashOnDamage());
        // StartCoroutine(InvincibiltyFrames());
        other.GetComponent<PlayerScript>().Damage(enemyData.attackPower);
        StartCoroutine(FakeAddForceMotion(other, enemyData, -facingDirection));
    }

    public void PlayerObstacleHit(GameObject other, EnemyData enemyData)
    {
        m_Animator.Play("Action-Hurt");
        // StartCoroutine(InvincibiltyFrames());
        StartCoroutine(FlashOnDamage());
        other.GetComponent<PlayerScript>().Damage(enemyData.attackPower);
        StartCoroutine(FakeAddForceMotion(other, enemyData, -m_Controller.FacingDirection));
    }

    private IEnumerator FlashOnDamage()
    {
        isFlashing = true;
        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        for (int i = 0; i < 5; i++)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.1f);
        }
        isFlashing = false;
    }
    public IEnumerator FakeAddForceMotion(GameObject target, EnemyData enemyData, float facingDirection)
    {
        float i = 0.01f;
        Rigidbody2D rb = target.GetComponent<Rigidbody2D>();
        Vector2 raycastDirection = new Vector2(-facingDirection, 0); // Raycast direction opposite to the facing direction
        float raycastDistance = .7f; // Adjust the distance as needed
        int worldLayerMask = LayerMask.GetMask("World"); // Create a layer mask for the "world" layer

        int numberOfRaycasts = 20; // Number of raycast lines
        float padding = 0.1f; // Padding to avoid detecting the ground
        Collider2D collider = target.GetComponent<Collider2D>(); // Get the Collider2D component
        float colliderHeight = collider.bounds.size.y; // Get the height of the collider
        float raycastSpacing = (colliderHeight /*- 2 * padding*/) / (numberOfRaycasts - 1); // Calculate the spacing between each raycast line

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
        Debug.Log("Knockback" + new Vector2(swordKnockback.x * facingDirection, swordKnockback.y));
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

        playerMana -= Stats.DashManaCost;
        EventManaUpdate(playerMana, maxMana);

        PlaySound(dashSound);
        // Revert the player's layer back to the original layer
    }
    private void OnDashEnd()
    {
        StopCoroutine(DashIFramesPadding());
        StartCoroutine(DashIFramesPadding());
    }

    private void OnJump()
    {
        PlaySound(jumpSound);
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
        var bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.Initialize(gunDamage);

        PlaySound(shootSound);
    }

    public int GetRandomBuff()
    {
        int rand = Random.Range(0, 5);

        switch (rand)
        {
            case 0:
                currentBuff.MaxHealth++;
                break;
            case 1:
                currentBuff.RecoverMana++;
                break;
            case 2:
                currentBuff.Speed++;
                break;
            case 3:
                currentBuff.SwordDamage++;
                break;
            case 4:
                currentBuff.GunDamage++;
                break;
            default:
                currentBuff.MaxHealth++;
                break;
        }

        UpdateStats();

        return rand;
    }

    public bool CanDash()
    {
        return playerMana >= Stats.DashManaCost;
    }

    private void CheckMana()
    {
        playerMana += (Stats.ManaRecoverPerSec + currentBuff.RecoverMana * Stats.BuffManaRecover) * Time.deltaTime;

        if (playerMana > maxMana)
        {
            playerMana = maxMana;
        }

        EventManaUpdate(playerMana, maxMana);
    }

    #region subscribe events   
    private void OnGameOver()
    {
        EventGameOver?.Invoke();
    }

    //TODO: Implement method to call this function to save player progress.
    public void OnFinishStage()
    {
        PlayerStatsHandler.SavePlayerBuff(currentBuff);
    }
    #endregion

    private void PlaySound(AudioSource audio)
    {
        if (audio == null) return;

        audio.pitch = SFXHelper.GetRandomPitch();
        audio.Play();
    }
}
