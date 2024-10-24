using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public BulletObject bullet;
    private int direction;
    void Start()
    {
        direction = PlayerScript.instance.m_Controller.FacingDirection;
        Destroy(gameObject, bullet.bulletLifeTime);
    }

    void Update()
    {
        transform.Translate(bullet.bulletSpeed * Time.deltaTime * new Vector2(direction, 0));
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            GameObject enemy = other.gameObject;
            if (enemy != null)
            {
                enemy.GetComponent<Health>().Damage(bullet.bulletDamage);
                if (enemy.TryGetComponent<Rigidbody2D>(out var rb))
                {
                    Vector2 knockbackDirection = (enemy.transform.position - transform.position).normalized;
                    rb.AddForce(-knockbackDirection * bullet.bulletKnockback, ForceMode2D.Impulse);
                }
            }
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

}