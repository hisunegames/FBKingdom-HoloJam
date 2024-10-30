using System;
using System.Collections;
using UnityEngine;
using UnityEngine.TextCore;
using FBK;
using Unity.VisualScripting;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyAIType
    {
        Grounded,
        Flying,
        Ranged,
        Jumper,
        Boss,
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

    void Awake()
    {
        hitbox.StayingInTrigger.AddListener(OnEnemyHitBoxTriggerStay);
        detection.FoundPlayer.AddListener(OnEnemyDetectionTriggerStay);
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
                break;
        }
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


    private IEnumerator ShootFireballAtIntervals()
    {
        ShootFireball();
        yield return new WaitForSeconds(enemyData.attackInterval);
        isAttacking = false;
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
        Debug.Log("Fireball initialized");
        Vector3 direction = (targetPosition - fireball.transform.position).normalized;

        // Apply force to the fireball to shoot it towards the player
        Rigidbody2D rb = fireball.GetComponent<Rigidbody2D>();
        rb.AddForce(direction * fireballSpeed, ForceMode2D.Impulse);
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
        }
        else if (facingDirection == 1)
        {
            GetComponentInChildren<SpriteRenderer>().flipX = true;
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
                    StartCoroutine(FlashOnDamage());
                    animator.Play("Attack");
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
}
