using UnityEngine;

public class ObstacleDamagePlayer : MonoBehaviour
{
    public EnemyData enemyData;
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player hit by obstacle: " + other.gameObject.name);
            other.gameObject.GetComponent<PlayerScript>().PlayerObstacleHit(other.gameObject, enemyData);
        }
    }
}
