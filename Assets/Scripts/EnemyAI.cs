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
    private float _speed;

    public CheckTriggerCollider hitbox;
    public CheckTriggerCollider detection;

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

    private Vector3 startingScale;
    private bool isFlashing = false;

    [SerializeField]
    private GameObject fireballPrefab;
    public float fireballSpeed = 5f;
    public float fireballArcAngle = 45f;
    public float sightRange = 10f;

    void Awake()
    {
        hitbox.StayingInTrigger.AddListener(OnEnemyHitBoxTriggerStay);
    }

    void Start()
    {
        _speed = enemyData.speed;
        startingScale = transform.localScale;
        animator = GetComponentInChildren<Animator>();
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
            case EnemyAIType.Ranged:
                HandleFlyingAI();
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
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(player.transform.position.x, transform.position.y), _speed * enemyData.speedMultiplier * Time.deltaTime);
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
            transform.position = Vector2.MoveTowards(transform.position, transform.position + (Vector3)retreatDirection * enemyData.retreatDistance, _speed * Time.deltaTime);
            if (Vector2.Distance(transform.position, player.transform.position) >= enemyData.retreatDistance)
            {
                isRetreating = false;
            }
            return;
        }

        if (distance < distanceBetween)
        {
            isChasing = true;
        }
        else if (distance > distanceBetween * 1.5f)
        {
            isChasing = false;
        }

        if (isChasing)
        {
            if (enemyAIType == EnemyAIType.Flying)
            {
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(player.transform.position.x, player.transform.position.y + flightLowerBound), _speed * Time.deltaTime);
                if (Math.Abs(distance) < enemyData.attackRange && GetComponentInChildren<DetectPlayer>().inRange && !isAttacking)
                {
                    isAttacking = true;
                    animator.SetBool("IsAttacking", true);
                    OnAttack();
                }
            }
            else if (enemyAIType == EnemyAIType.Ranged)
            {
                // Move in a circle around the player
                // float angle = Time.time * _speed;
                float offset = Mathf.Sin(Time.time * _speed) * distanceBetween;
                // Vector2 offset = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)) * distanceBetween;
                // transform.position = Vector2.MoveTowards(transform.position, (Vector2)player.transform.position + offset, _speed * Time.deltaTime);
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(player.transform.position.x + offset, player.transform.position.y + flightLowerBound), _speed * Time.deltaTime);
                // Shoot fireballs at intervals
                if (!isAttacking)
                {
                    isAttacking = true;
                    StartCoroutine(ShootFireballAtIntervals());
                }
            }


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

    private IEnumerator ShootFireballAtIntervals()
    {
        while (isChasing)
        {
            ShootFireball();
            yield return new WaitForSeconds(enemyData.attackInterval);
        }
        isAttacking = false;
    }

    //TODO: Implement ShootFireball method by wait until the init state is finished then shoot
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
        // Calculate the direction from the fireball's position to the player's position
    }

    IEnumerator WaitForFireball(Vector3 pos, GameObject fireball)
    {
        yield return new WaitUntil(() => fireball.GetComponent<Animator>().GetBool("Initialized"));
        Debug.Log("Fireball initialized");
        Vector3 direction = (pos - fireball.transform.position).normalized;

        // Apply force to the fireball to shoot it towards the player
        Rigidbody2D rb = fireball.GetComponent<Rigidbody2D>();
        Animator f_animator = fireball.GetComponent<Animator>();
        rb.AddForce(direction * fireballSpeed, ForceMode2D.Impulse);
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        Vector2 targetPatrolPoint = patrolPoints[currentPatrolIndex];
        transform.position = Vector2.MoveTowards(transform.position, targetPatrolPoint, _speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPatrolPoint) < 0.1f)
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
        float startingSpeed = _speed;
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

    private void OnAttack()
    {
        _speed *= enemyData.speedMultiplier;
        Debug.Log("Attacking");
    }


    private void ResetSpeedAfterAttack()
    {
        animator.SetBool("IsAttacking", false);
        _speed /= enemyData.speedMultiplier;
        isAttacking = false;
        isRetreating = true;
    }

    public void AdjustSpeed(float newSpeed)
    {
        _speed = newSpeed;
    }

    void OnEnemyHitBoxTriggerStay(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Hit-Player");
            if (!PlayerScript.instance.isInvincible)
            {
                PlayerScript.instance.PlayerHit(other.gameObject, enemyData, facingDirection);
                if (EnemyAIType.Flying == enemyAIType)
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

    void OnEnemyDetectionTriggerStay(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player Detected");
            isChasing = true;
        }
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
