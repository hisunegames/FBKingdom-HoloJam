using System.Collections;
using UnityEngine;

public class OnWaveHitPlayer : MonoBehaviour
{
    [SerializeField]
    private EnemyData enemyData;
    private Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player hit by wave");
            Vector2 direction = rb.linearVelocity.normalized;
            PlayerScript.instance.PlayerHit(other.gameObject, enemyData, direction.x);
            Destroy(gameObject);
            // StartCoroutine(DestroySelf());
            // Destroy(other.gameObject);
        }
    }

    IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}
