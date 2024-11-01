using System;
using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyAIType
    {
        Grounded,
        Flying,
        Ranged,
        Jumper,
        Boss
    }
    public GameObject player;

    public EnemyData enemyData;
    public EnemyAIType enemyAIType;
    public float distanceBetween;
    public float facingDirection;
    [SerializeField]
    private float curSpeed;

    public CheckTriggerCollider hitbox;
    public DetectPlayer detection;

    [SerializeField]
    private Vector2[] patrolPoints;
    private int currentPatrolIndex = 0;
    private bool isChasing = false;

    private bool isTurningAround = false;
    public float flightLowerBound;

    [SerializeField]
    private Animator animator;
    public bool isAttacking = false;
    private bool isRetreating = false;
    private bool playerInRange = false;

    private Vector3 startingScale;
    private bool isFlashing = false;

    [SerializeField]
    private GameObject fireballPrefab;
    public float fireballSpeed = 5f;
    public float fireballArcAngle = 45f;
    public float sightRange = 10f;
    private float fireballTimer = 0f;
    [SerializeField] private GameObject fireWavePrefab;
    [SerializeField] private float fireWaveSpeed;

    bool isBossDead = false;

    public enum BossAttackPattern
    {
        Fireball = 1,
        FireWall = 2,
        Gun = 3,
        Dash = 10,
        Teleport = 11,
    }

    [Serializable]
    public struct BossAttackPhase
    {
        public BossAttackPattern[] pattern;
    }

    [Header("BOSS ONLY")]
    [SerializeField]
    private BossAttackPhase[] bossAttackPhases;
    [SerializeField]
    private int phases;
    private int patternNum;
    private float bossFireballTimer;
    private float bossSubFireBallTimer;
    private int bossFireBallCount;
    private float dashTimer;
    private float dashChance = 1f;
    private float teleportTimer;
    private float teleportChance = 1f;
    [SerializeField]
    private DashPointHandler dashPointHandler;
    Vector2 positionToTeleportTo;
    Vector2 positionToDashTo;
    bool isDashing;
    [SerializeField]
    float dashSpeed;
    [HideInInspector] public bool canBoss;
    public Bullet bulletPrefab;
    public Transform firePoint;
    public Transform firePointLeft;
    public Transform firePointRight;
    public GameObject telegrapher;

    public float gunDamage = 10;
    private float gunPostTeleportWaitTime;
    int shotPhase;

    int teleportPhase;

    [Header("SFX")]
    [SerializeField] private AudioSource shootSound;
    [SerializeField] private AudioSource fireballSound;
    [SerializeField] private AudioSource fireWallSound;
    [SerializeField] private AudioSource jumpSound;
    [SerializeField] private AudioSource hurtSound;
    [SerializeField] private AudioSource dashSound;

    void Awake()
    {
        hitbox.StayingInTrigger.AddListener(OnEnemyHitBoxTriggerStay);
        detection.FoundPlayer.AddListener(OnEnemyDetectionTriggerStay);

        if (enemyAIType != EnemyAIType.Boss)
            return;

        GetComponent<Health>().OnDeath.AddListener(InitiateBossDeath);

        if (TryGetComponent<Health>(out var heath))
        {
            heath.EventHealthUpdate += CheckPhase;
        }
    }

    void Start()
    {
        curSpeed = enemyData.speed;
        startingScale = transform.localScale;
        // animator = GetComponentInChildren<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
    }

    void FixedUpdate()
    {
        switch (enemyAIType)
        {
            case EnemyAIType.Grounded:
                HandleGroundedAI();
                break;
            case EnemyAIType.Flying:
                HandleFlyingAI();
                break;
            case EnemyAIType.Ranged:
                HandleRangedAI();
                break;
            case EnemyAIType.Boss:
                HandleBossAI();
                break;
        }
    }

    private void HandleBossAI()
    {
        if (isFlashing || !canBoss || isBossDead) return;

        switch (bossAttackPhases[phases].pattern[patternNum])
        {
            case BossAttackPattern.Fireball:

                bossFireballTimer += Time.deltaTime;

                if (bossFireballTimer >= 2)
                {
                    bossSubFireBallTimer += Time.deltaTime;

                    animator.SetBool("Fireball", true);

                    if (bossSubFireBallTimer >= 0.3f)
                    {
                        ShootFireball();

                        bossSubFireBallTimer = 0f;
                        bossFireBallCount++;

                        if (bossFireBallCount >= 5)
                        {
                            bossFireballTimer = 0;
                            bossFireBallCount = 0;

                            UpdateBossAttackPattern();

                            animator.SetBool("Fireball", false);
                        }
                    }
                }

                break;

            case BossAttackPattern.FireWall:

                bossFireballTimer += Time.deltaTime;

                if (bossFireballTimer >= 5)
                {
                    bossSubFireBallTimer += Time.deltaTime;

                    if (bossSubFireBallTimer >= 0.5f)
                    {
                        animator.SetTrigger("Special");

                        CastFireWave();

                        bossSubFireBallTimer = 0f;
                        bossFireBallCount++;

                        if (bossFireBallCount >= 3)
                        {
                            bossFireballTimer = 0;
                            bossFireBallCount = 0;

                            UpdateBossAttackPattern();
                        }

                    }
                }

                break;

            case BossAttackPattern.Dash:

                switch (teleportPhase)
                {
                    case 0:
                        TelegraphTeleport(true);
                        teleportPhase = 1;
                        return;

                    case 1:
                        teleportTimer += Time.deltaTime;
                        if (teleportTimer >= 1.5f)
                        {
                            teleportPhase = 2;
                            teleportTimer = 0f;
                            ExecuteTeleport();
                        }
                        return;
                }

                if (isDashing)
                {
                    Dash();

                    return;
                }

                dashTimer += Time.deltaTime;

                if (dashTimer >= .5f)
                {

                    if (UnityEngine.Random.Range(0.0f, 1.0f) <= dashChance)
                    {
                        isDashing = true;
                        dashTimer = 0f;
                        dashChance /= 2f;

                        Dash();

                        animator.SetBool("IsDash", true);
                    }
                    else
                    {
                        teleportPhase = 0;
                        dashChance = 1f;

                        UpdateBossAttackPattern();

                        animator.SetBool("IsDash", false);
                    }
                }

                break;

            case BossAttackPattern.Teleport:

                switch (teleportPhase)
                {
                    case 0:
                        TelegraphTeleport(false);
                        teleportPhase = 1;
                        return;

                    case 1:
                        teleportTimer += Time.deltaTime;
                        if (teleportTimer >= 1.5f)
                        {
                            teleportPhase = 0;
                            teleportTimer = 0f;


                            ExecuteTeleport();

                            if (UnityEngine.Random.Range(0.0f, 1.0f) <= teleportChance)
                            {
                                teleportChance /= 2;
                            }
                            else
                            {
                                teleportChance = 1f;

                                UpdateBossAttackPattern();
                            }
                        }
                        return;
                }

                break;

            case BossAttackPattern.Gun:

                switch (teleportPhase)
                {
                    case 0:
                        TelegraphTeleport(true);
                        teleportPhase = 1;
                        return;

                    case 1:
                        teleportTimer += Time.deltaTime;
                        if (teleportTimer >= 1.5f)
                        {
                            teleportPhase = 2;
                            teleportTimer = 0f;
                            ExecuteTeleport();
                        }
                        return;
                }

                if (shotPhase == 0)
                {
                    shotPhase = 1;
                    return;
                }

                if (shotPhase == 1)
                {
                    gunPostTeleportWaitTime += Time.deltaTime;

                    if (gunPostTeleportWaitTime >= 0.5f)
                    {
                        gunPostTeleportWaitTime = 0;

                        Shoot();

                        animator.SetTrigger("Fire");

                        shotPhase = 2;
                    }

                    return;
                }


                gunPostTeleportWaitTime += Time.deltaTime;

                if (gunPostTeleportWaitTime >= 2f)
                {

                    teleportPhase = 0;

                    gunPostTeleportWaitTime = 0;

                    shotPhase = 0;

                    UpdateBossAttackPattern();
                }

                break;
        }

    }

    private void CheckPhase(float currentHealth, float maxHealth)
    {
        PlaySound(hurtSound);

        if (phases == 0 && currentHealth < maxHealth * 0.5f)
        {
            phases = 1;
        }
        if (phases == 1 && currentHealth < maxHealth * 0.2f)
        {
            phases = 2;
        }
    }

    void TelegraphTeleport(bool toPlayer)
    {
        var teleportToPos = (UnityEngine.Random.Range(0, 2));

        int dashToPos = 0;

        if (teleportToPos == 0)
        {
            dashToPos = 1;
        }

        if (toPlayer)
        {

            if (player.transform.position.y <= 0.61f)//bottom
            {
                if (player.transform.position.x <= -4.3f) //left
                {
                    positionToTeleportTo = dashPointHandler.bottomLeft[teleportToPos].transform.position;
                    positionToDashTo = dashPointHandler.bottomLeft[dashToPos].transform.position;

                }
                else if (player.transform.position.x > -4.3f && player.transform.position.x <= 3.96f)//center
                {
                    positionToTeleportTo = dashPointHandler.bottomCenter[teleportToPos].transform.position;
                    positionToDashTo = dashPointHandler.bottomCenter[dashToPos].transform.position;

                }
                else //right
                {
                    positionToTeleportTo = dashPointHandler.bottomRight[teleportToPos].transform.position;
                    positionToDashTo = dashPointHandler.bottomRight[dashToPos].transform.position;
                }

            }
            else//top
            {
                if (player.transform.position.x <= -4.3f) //left
                {
                    positionToTeleportTo = dashPointHandler.topLeft[teleportToPos].transform.position;
                    positionToDashTo = dashPointHandler.topLeft[dashToPos].transform.position;
                }
                else if (player.transform.position.x > -4.3f && player.transform.position.x <= 3.96f)//center
                {
                    positionToTeleportTo = dashPointHandler.topCenter[teleportToPos].transform.position;
                    positionToDashTo = dashPointHandler.topCenter[dashToPos].transform.position;
                }
                else //right
                {
                    positionToTeleportTo = dashPointHandler.topRight[teleportToPos].transform.position;
                    positionToDashTo = dashPointHandler.topRight[dashToPos].transform.position;
                }
            }



        }
        else
        {
            switch (UnityEngine.Random.Range(0, 6))
            {
                case 0:
                    positionToTeleportTo = dashPointHandler.bottomLeft[teleportToPos].transform.position;
                    break;
                case 1:
                    positionToTeleportTo = dashPointHandler.bottomCenter[teleportToPos].transform.position;
                    break;
                case 2:
                    positionToTeleportTo = dashPointHandler.bottomRight[teleportToPos].transform.position;
                    break;
                case 3:
                    positionToTeleportTo = dashPointHandler.topLeft[teleportToPos].transform.position;
                    break;
                case 4:
                    positionToTeleportTo = dashPointHandler.topCenter[teleportToPos].transform.position;
                    break;
                case 5:
                    positionToTeleportTo = dashPointHandler.topRight[teleportToPos].transform.position;
                    break;


            }

        }

        telegrapher.SetActive(true);

        telegrapher.transform.position = positionToTeleportTo;
    }

    void Shoot()
    {
        var bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.Initialize(gunDamage);

        PlaySound(shootSound);
    }

    private void UpdateBossAttackPattern()
    {
        patternNum++;

        if (patternNum >= bossAttackPhases[phases].pattern.Length)
        {
            patternNum = 0;
        }
    }

    private void ExecuteTeleport()
    {
        transform.position = positionToTeleportTo;

        Vector2 direction = player.transform.position - transform.position;
        if (direction.x < 0)
        {
            facingDirection = -1;
        }
        else
        {
            facingDirection = 1;
        }

        FlipSprite();

        PlaySound(dashSound);
    }

    private void Dash()
    {
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(positionToDashTo.x, positionToDashTo.y), curSpeed * enemyData.speedMultiplier * Time.deltaTime * dashSpeed);

        animator.Play("Dash");
        StartCoroutine(KurokamiAfterImageGenerator.instance.GenerateAfterImage());
        if (Mathf.Abs(transform.position.x - positionToDashTo.x) < 0.1f)
        {
            isDashing = false;
        }

        PlaySound(dashSound);
    }

    private void HandleGroundedAI()
    {
        if (isAttacking || isFlashing) return;

        Vector2 direction = player.transform.position - transform.position;
        float distance = direction.magnitude;

        if (distance < enemyData.attackRange)
        {
            isChasing = true;
        }
        else if (distance > enemyData.attackRange * 1.5f)
        {
            isChasing = false;
        }

        if (isChasing)
        {
            if (animator == null || GetComponent<Health>().currentHealth <= 0) return;
            StopCoroutine(TurnAround());
            animator.Play("Move");
            animator.SetBool("IsPatrolling", false);
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb.linearVelocityY == 0)
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(player.transform.position.x, transform.position.y), curSpeed * enemyData.speedMultiplier * Time.deltaTime);

            if (direction.x < 0)
            {
                facingDirection = -1;
            }
            else
            {
                facingDirection = 1;
            }
            FlipSprite();
        }
        else
        {
            Patrol();
        }
    }

    private void HandleFlyingAI()
    {
        if (isAttacking || isFlashing) return;

        Vector2 direction = player.transform.position - transform.position;
        float distance = direction.magnitude;

        if (isRetreating)
        {
            Vector2 retreatDirection = (transform.position - player.transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, transform.position + (Vector3)retreatDirection * enemyData.retreatDistance, curSpeed * Time.deltaTime);
            if (Vector2.Distance(transform.position, player.transform.position) >= enemyData.retreatDistance)
            {
                isRetreating = false;
            }
            return;
        }

        if (distance < distanceBetween)
        {
            isChasing = true;
            // Debug.Log("IsChasing");
        }
        else if (distance > distanceBetween * 1.5f)
        {
            isChasing = false;
        }

        if (isChasing)
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(player.transform.position.x, player.transform.position.y + flightLowerBound), curSpeed * Time.deltaTime);
            animator.SetBool("IsAttacking", true);
            AdjustSpeed(enemyData.speed * enemyData.speedMultiplier);
            // transform.position = Vector2.MoveTowards(transform.position, new Vector2(player.transform.position.x, player.transform.position.y + flightLowerBound), curSpeed * Time.deltaTime);
            // if (playerInRange)
            // {
            //     isAttacking = true;
            //     AdjustSpeed(enemyData.speed * enemyData.speedMultiplier);
            //     animator.SetBool("IsAttacking", true);
            //     // OnAttack();
            // }
            // else if (!playerInRange)
            // {
            //     ResetSpeedAfterAttack();
            // }


            if (direction.x < 0)
            {
                facingDirection = -1;
            }
            else
            {
                facingDirection = 1;
            }
            FlipSprite();
        }
        else
        {
            animator.SetBool("IsAttacking", false);
            AdjustSpeed(enemyData.speed);
            Patrol();
        }
    }

    private void HandleRangedAI()
    {
        if (isFlashing) return;

        Vector2 direction = player.transform.position - transform.position;
        float distance = direction.magnitude;

        if (isRetreating)
        {
            Vector2 retreatDirection = (transform.position - player.transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, transform.position + (Vector3)retreatDirection * enemyData.retreatDistance, curSpeed * Time.deltaTime);
            if (Vector2.Distance(transform.position, player.transform.position) >= enemyData.retreatDistance)
            {
                isRetreating = false;
            }
            return;
        }

        if (distance < distanceBetween)
        {
            isChasing = true;
            Debug.Log("IsChasing");
        }
        else if (distance > distanceBetween * 1.5f)
        {
            isChasing = false;
        }

        if (isChasing)
        {
            // Move in a circle around the player
            float angle = Time.time * curSpeed;
            Vector2 offset = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)) * distanceBetween;
            transform.position = Vector2.MoveTowards(transform.position, (Vector2)player.transform.position + offset, curSpeed * Time.deltaTime);

            if (direction.x < 0)
            {
                facingDirection = -1;
            }
            else
            {
                facingDirection = 1;
            }
            FlipSprite();

            // Update fireball timer
            fireballTimer += Time.deltaTime;
            if (fireballTimer >= enemyData.attackInterval)
            {
                ShootFireball();
                fireballTimer = 0f;
            }
        }
        else
        {
            Patrol();
        }
    }

    private void ShootFireball()
    {
        Vector3 playerPosition = player.transform.position;

        // Calculate a random angle within the specified arc
        float halfArcAngle = fireballArcAngle / 2f;
        float randomAngle = UnityEngine.Random.Range(-halfArcAngle, halfArcAngle);

        // Convert the angle to radians
        float angleInRadians = randomAngle * Mathf.Deg2Rad;

        // Determine the position to instantiate the fireball based on the random angle
        Vector3 fireballPosition = transform.position + new Vector3(Mathf.Sin(angleInRadians), Mathf.Cos(angleInRadians), 0);

        // Instantiate the fireball at the calculated position
        GameObject fireball = Instantiate(fireballPrefab, fireballPosition, Quaternion.identity, transform);
        StartCoroutine(WaitForFireball(playerPosition, fireball));
    }

    private IEnumerator WaitForFireball(Vector3 targetPosition, GameObject fireball)
    {
        yield return new WaitUntil(() => fireball.GetComponent<Animator>().GetBool("Initialized"));
        Debug.Log("FireballNormal initialized");
        Vector3 direction = (targetPosition - fireball.transform.position).normalized;

        // Apply force to the fireball to shoot it towards the player
        Rigidbody2D rb = fireball.GetComponent<Rigidbody2D>();
        rb.AddForce(direction * fireballSpeed, ForceMode2D.Impulse);

        PlaySound(fireballSound);
    }

    private void CastFireWave()
    {
        float directionX = (player.transform.position.x - transform.position.x) > 0 ? 1 : -1;
        Vector3 firePosition = transform.position + new Vector3(directionX, 0, 0);

        GameObject fireWave = Instantiate(fireWavePrefab, firePosition, Quaternion.identity, transform);
        fireWave.GetComponent<SpriteRenderer>().flipX = directionX == 1;
        fireWave.GetComponent<Rigidbody2D>().AddForce(directionX * fireWaveSpeed * Vector3.right, ForceMode2D.Impulse);

        PlaySound(fireWallSound);
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        Vector2 targetPatrolPoint = patrolPoints[currentPatrolIndex];
        transform.position = Vector2.MoveTowards(transform.position, targetPatrolPoint, curSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPatrolPoint) <= 0.1f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }
        else
        {
            if (targetPatrolPoint.x < transform.position.x)
            {
                facingDirection = -1;
            }
            else
            {
                facingDirection = 1;
            }
            FlipSprite();
        }
    }

    public IEnumerator TurnAround()
    {
        isTurningAround = true;
        Debug.Log("Turning around");
        animator.Play("Move-Start");
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(animator != null ? animator.GetCurrentAnimatorStateInfo(0).length : .1f);
        float startingSpeed = curSpeed;
        AdjustSpeed(0);
        animator.Play("Idle");
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(animator != null ? animator.GetCurrentAnimatorStateInfo(0).length : .1f);
        FlipSprite();
        animator.Play("Move-Start");
        AdjustSpeed(startingSpeed);
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(animator != null ? animator.GetCurrentAnimatorStateInfo(0).length : .1f);
        animator.Play("Move");
        isTurningAround = false;
    }

    private void FlipSprite()
    {
        if (facingDirection == -1)
        {
            GetComponentInChildren<SpriteRenderer>().flipX = false;
            if (enemyAIType == EnemyAIType.Boss)
            {
                firePoint = firePointLeft;
            }
        }
        else if (facingDirection == 1)
        {
            GetComponentInChildren<SpriteRenderer>().flipX = true;

            if (enemyAIType == EnemyAIType.Boss)
            {
                firePoint = firePointRight;
            }
        }
    }

    // private void OnAttack()
    // {
    //     curSpeed *= enemyData.speedMultiplier;
    //     Debug.Log("Attacking");
    // }


    private void ResetSpeedAfterAttack()
    {
        animator.SetBool("IsAttacking", false);
        AdjustSpeed(enemyData.speed);
        isAttacking = false;
        isRetreating = true;
    }

    public void AdjustSpeed(float newSpeed)
    {
        curSpeed = newSpeed;
    }

    void OnEnemyHitBoxTriggerStay(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Hit-Player");
            if (!PlayerScript.instance.isInvincible)
            {
                PlayerScript.instance.PlayerHit(other.gameObject, enemyData, facingDirection);
                if (EnemyAIType.Flying == enemyAIType && EnemyAIType.Ranged == enemyAIType)
                {
                    ResetSpeedAfterAttack();
                }
                else if (EnemyAIType.Grounded == enemyAIType)
                {
                    // StartCoroutine(FlashOnDamage());
                    animator.Play("Attack");
                }
                else if (EnemyAIType.Boss == enemyAIType)
                {
                    // StartCoroutine(FlashOnDamage());
                }

            }
        }
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

    void OnEnemyDetectionTriggerStay(bool isPlayerDetected)
    {
        playerInRange = isPlayerDetected;
    }

    private void OnDrawGizmos()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
            return;

        Gizmos.color = Color.red;
        for (int i = 0; i < patrolPoints.Length; i++)
        {
            Gizmos.DrawWireSphere(patrolPoints[i], 0.2f);
            if (i < patrolPoints.Length - 1)
            {
                Gizmos.DrawLine(patrolPoints[i], patrolPoints[i + 1]);
            }
            else
            {
                Gizmos.DrawLine(patrolPoints[i], patrolPoints[0]);
            }
        }

        if (enemyAIType == EnemyAIType.Ranged)
        {
            Gizmos.color = Color.red;
            Vector3 direction = transform.right * sightRange;
            // Gizmos.DrawRay(transform.position, direction);

            Gizmos.color = Color.yellow;
            float halfArc = fireballArcAngle / 2;
            Vector3 leftArc = Quaternion.Euler(0, 0, -halfArc) * Vector3.up * sightRange;
            Vector3 rightArc = Quaternion.Euler(0, 0, halfArc) * Vector3.up * sightRange;
            Gizmos.DrawRay(transform.position, leftArc);
            Gizmos.DrawRay(transform.position, rightArc);
        }
    }

    private void PlaySound(AudioSource audio)
    {
        if (audio == null) return;

        audio.pitch = SFXHelper.GetRandomPitch();
        audio.Play();
    }

    private void InitiateBossDeath()
    {
        // animator.Play("Death");
        if (isBossDead) return;
        isBossDead = true;
        Debug.Log("Boss Death");
        animator.Play("Death");
        GetComponent<Collider2D>().enabled = false;
        // GetComponent<Rigidbody2D>().simulated = false;
        GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        telegrapher.SetActive(false);
        this.enabled = false;
    }
}
