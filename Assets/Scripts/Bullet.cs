using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public BulletObject bullet;
    private int direction;
    private bool hit = false;
    void Start()
    {
        direction = PlayerScript.instance.m_Controller.FacingDirection;
        Destroy(gameObject, bullet.bulletLifeTime);
    }

    void Update()
    {
        if (!hit)
            transform.Translate(bullet.bulletSpeed * Time.deltaTime * new Vector2(direction, 0));
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            hit = true;
        GetComponent<Animator>().Play("Burst");
        if (other.CompareTag("Enemy"))
        {
            GameObject enemy = other.gameObject;
            if (enemy != null)
            {
                enemy.GetComponent<Health>().Damage(bullet.bulletDamage);
                // if (enemy.TryGetComponent<Rigidbody2D>(out var rb))
                // {
                //     Vector2 knockbackDirection = (enemy.transform.position - transform.position).normalized;
                //     StartCoroutine(PlayerScript.instance.FakeAddForceMotion(enemy, enemy.GetComponent<EnemyAI>().enemyData, knockbackDirection.x));
                //     // rb.AddForce(-knockbackDirection * bullet.bulletKnockback, ForceMode2D.Impulse);
                // }
            }

        }
    }



}