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

    public float flightLowerBound;

    [SerializeField]
    private Animator animator;
    private bool isAttacking = false;
    private bool isRetreating = false;

    private Vector3 startingScale;
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
        Vector2 direction = player.transform.position - transform.position;
        float distance = direction.magnitude;

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
            Vector2 targetPosition = new Vector2(player.transform.position.x, transform.position.y);
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, enemyData.speed * Time.deltaTime);

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

                Vector2 targetPatrolPoint = patrolPoints[currentPatrolIndex];
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
        // if (player == null)
        // {
        //     yield break;
        // }
        // yield return new WaitForSeconds(1f);
        animator.SetBool("IsAttacking", false);
        _speed /= enemyData.speedMultiplier;
        isAttacking = false;
        isRetreating = true;
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
                    ResetSpeedAfterAttack();
            }
        }
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
