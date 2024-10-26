using System;
using System.Collections;
using UnityEngine;
using UnityEngine.TextCore;
using FBK;

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
            // if (Math.Abs(distance) < enemyData.attackRange)
            // {
            StopCoroutine(TurnAround());
            animator.Play("Move");
            animator.SetBool("IsPatrolling", false);
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(player.transform.position.x, transform.position.y), _speed * enemyData.speedMultiplier * Time.deltaTime);
            // isAttacking = true;
            // animator.SetBool("IsAttacking", true);
            // OnAttack();
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
            if (patrolPoints != null && patrolPoints.Length > 0)
            {
                // Ensure currentPatrolIndex is within bounds
                currentPatrolIndex = Mathf.Clamp(currentPatrolIndex, 0, patrolPoints.Length - 1);
                animator.SetBool("IsPatrolling", true);
                Vector2 targetPatrolPoint = patrolPoints[currentPatrolIndex];
                if (Vector2.Distance(transform.position, targetPatrolPoint) < 0.5f && !isTurningAround)
                {
                    StartCoroutine(TurnAround());
                }

                if (Vector2.Distance(transform.position, targetPatrolPoint) < 0.1f)
                {
                    currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                }
                else
                {
                    transform.position = Vector2.MoveTowards(transform.position, targetPatrolPoint, enemyData.speed * Time.deltaTime);
                    // Adjust facing direction based on movement
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
        }
    }

    public IEnumerator TurnAround()
    {
        isTurningAround = true;
        Debug.Log("Turning around");
        // animator.SetTrigger("Flip");
        animator.Play("Move-Start");
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length); // Adjust the duration as needed
        float startingSpeed = _speed;
        AdjustSpeed(0);
        animator.Play("Idle");
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length); // Adjust the duration as needed
        FlipSprite();
        animator.Play("Move-Start");
        AdjustSpeed(startingSpeed);
        yield return new WaitForEndOfFrame();

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length); // Adjust the duration as needed
        animator.Play("Move");
        isTurningAround = false;
    }

    private void HandleFlyingAI()
    {
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
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(player.transform.position.x, player.transform.position.y + flightLowerBound), _speed * Time.deltaTime);
            if (Math.Abs(distance) < enemyData.attackRange && !GetComponentInChildren<DetectPlayer>().inRange && !isAttacking)
            {
                isAttacking = true;
                animator.SetBool("IsAttacking", true);
                OnAttack();
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
            Vector2 targetPatrolPoint = patrolPoints[currentPatrolIndex];
            if (Vector2.Distance(transform.position, targetPatrolPoint) < 0.1f)
            {
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, targetPatrolPoint, _speed * Time.deltaTime);
                // Adjust facing direction based on movement
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

        // Adjust facing direction based on movement

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
    }
}
