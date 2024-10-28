using System.Collections;
using UnityEngine;

public class FireballController : MonoBehaviour
{
    private Animator f_animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        f_animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(DestroyFireball());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            f_animator.Play("Explode");

            // Get the Rigidbody2D component
            Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();

            // Calculate the direction from the velocity
            Vector2 direction = rb != null ? rb.linearVelocity.normalized : Vector2.zero;

            // Call the PlayerHit method with the direction
            PlayerScript.instance.PlayerHit(other.gameObject, gameObject.transform.parent.gameObject.GetComponent<EnemyAI>().enemyData, direction.x);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("World"))
        {
            f_animator.Play("Explode");
        }
    }

    private IEnumerator DestroyFireball()
    {
        yield return new WaitForSeconds(8f);
        Destroy(gameObject);
    }
}
