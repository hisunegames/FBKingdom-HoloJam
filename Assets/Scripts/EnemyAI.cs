using System;
using System.Collections;
using UnityEngine;
using UnityEngine.TextCore;

public class EnemyAI : MonoBehaviour
{
    public GameObject player;

    public EnemyData enemyData;
    public float distanceBetween;
    [SerializeField]
    private float facingDirection;

    [SerializeField]
    private float distance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        distance = transform.position.x - player.transform.position.x;
        float direction = player.transform.position.x - transform.position.x;
        //direction.Normalize();
        //float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (Math.Abs(distance) < distanceBetween)
        {
            transform.position = Vector2.MoveTowards(this.transform.position, new Vector2(player.transform.position.x, this.transform.position.y), enemyData.speed * Time.deltaTime);
            //transform.rotation = Quaternion.Euler(Vector3.forward * angle);
        }
        if (distance < 0)
        {
            facingDirection = -1;
        }
        else
        {
            facingDirection = 1;
        }

    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Hit-Player");
            if (!PlayerScript.instance.isInvincible)
            {
                PlayerScript.instance.PlayerHit(other.gameObject, enemyData, facingDirection);
            }

        }
    }



}
