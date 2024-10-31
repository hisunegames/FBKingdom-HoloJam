using UnityEngine;

public class Bullet : MonoBehaviour
{
    public BulletObject bullet;
    private int direction;
    private bool hit = false;
    private float damage;

    public bool isPlayerBullet;

    public void Initialize(float _damage)
    {
        
        if (isPlayerBullet)
        {
            damage = _damage;
            direction = PlayerScript.instance.m_Controller.FacingDirection;
            Destroy(gameObject, bullet.bulletLifeTime);
        }
        else 
        {
            damage = _damage;
            if (transform.position.x < PlayerScript.instance.transform.position.x)
            {
                direction = 1;
            }
            else
            {
                direction = -1;
            }

            Destroy(gameObject, bullet.bulletLifeTime);
        }
    }

    void Update()
    {
        if (!hit)
            transform.Translate(bullet.bulletSpeed * Time.deltaTime * new Vector2(direction, 0));
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isPlayerBullet)
        {
            if (!other.CompareTag("Player"))
                hit = true;
            GetComponent<Animator>().Play("Burst");
            if (other.CompareTag("Enemy"))
            {
                GameObject enemy = other.gameObject;
                if (enemy != null)
                {
                    enemy.GetComponent<Health>().Damage(damage);
                    // if (enemy.TryGetComponent<Rigidbody2D>(out var rb))
                    // {
                    //     Vector2 knockbackDirection = (enemy.transform.position - transform.position).normalized;
                    //     StartCoroutine(PlayerScript.instance.FakeAddForceMotion(enemy, enemy.GetComponent<EnemyAI>().enemyData, knockbackDirection.x));
                    //     // rb.AddForce(-knockbackDirection * bullet.bulletKnockback, ForceMode2D.Impulse);
                    // }
                }

            }
        }
        else
        {
            if (other.CompareTag("Player"))
                hit = true;
            GetComponent<Animator>().Play("Burst");
            if (other.CompareTag("Player"))
            {

                other.GetComponent<PlayerScript>().PlayerHit(other.gameObject, new EnemyData() { attackPower = 10 }, direction);
            }
        }
        
    }



}