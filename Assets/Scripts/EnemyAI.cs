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
    [SerializeField]
    private float facingDirection;

    [SerializeField]
    private Vector2[] patrolPoints;
    private int currentPatrolIndex = 0;
    private bool isChasing = false;

    public float flightLowerBound;

    [SerializeField]
    private Animator animator;
    private bool isAttacking = false;
    private bool isRetreating = false;
    // public float retreatDistance = 1f; // Distance to retreat after attack

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        // Define patrol points (you can set these in the inspector or dynamically)
        // patrolPoints = new Vector2[] { new Vector2(-5, 5), new Vector2(5, 5), new Vector2(5, -5), new Vector2(-5, -5) };
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
                // Add cases for other AI types as needed
        }
    }

    private void HandleGroundedAI()
    {
        Vector2 direction = player.transform.position - transform.position;
        float distance = direction.magnitude;

        if (distance < distanceBetween)
        {
            // Move towards the player only in the X direction
            Vector2 targetPosition = new Vector2(player.transform.position.x, transform.position.y);
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, enemyData.speed * Time.deltaTime);
        }

        // Adjust facing direction based on player's position
        if (direction.x < 0)
        {
            facingDirection = -1;
        }
        else
        {
            facingDirection = 1;
        }
    }

    private void HandleFlyingAI()
    {
        Vector2 direction = player.transform.position - transform.position;
        float distance = direction.magnitude;

        if (isRetreating)
        {
            // Move away from the player
            Vector2 retreatDirection = (transform.position - player.transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, transform.position + (Vector3)retreatDirection * enemyData.retreatDistance, enemyData.speed * Time.deltaTime);
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
            // Move towards the player in both X and Y directions
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(player.transform.position.x, player.transform.position.y + flightLowerBound), enemyData.speed * Time.deltaTime);
            if (Math.Abs(distance) < enemyData.attackRange && !GetComponentInChildren<DetectPlayer>().inRange && !isAttacking)
            {
                isAttacking = true;
                OnAttack();
            }
        }
        else
        {
            // Patrol between points
            Vector2 targetPatrolPoint = patrolPoints[currentPatrolIndex];
            if (Vector2.Distance(transform.position, targetPatrolPoint) < 0.1f)
            {
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, targetPatrolPoint, enemyData.speed * Time.deltaTime);
            }
        }

        // Adjust facing direction based on player's position
        if (direction.x < 0)
        {
            facingDirection = -1;
        }
        else
        {
            facingDirection = 1;
        }
    }

    private void OnAttack()
    {
        // Attack the player
        enemyData.speed *= enemyData.speedMultiplier; // Speed up
        animator.SetTrigger("Attack"); // Trigger attack animation

        // Optionally, reset the speed after a delay
        StartCoroutine(ResetSpeedAfterAttack());
    }

    private IEnumerator ResetSpeedAfterAttack()
    {
        yield return new WaitForSeconds(1f); // Adjust the delay as needed
        enemyData.speed /= enemyData.speedMultiplier; // Reset speed
        isAttacking = false;
        isRetreating = true; // Start retreating after attack
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Hit-Player");
            // Implement damage or other interaction logic here
            if (!PlayerScript.instance.isInvincible)
            {
                PlayerScript.instance.PlayerHit(other.gameObject, enemyData, facingDirection);
            }

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
