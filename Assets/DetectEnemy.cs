using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class DetectEnemy : MonoBehaviour
{
    public UnityEvent<bool> OnEnemyInRange;
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            // Debug.Log("Enemy Detected and in range: " + other.gameObject.name);
            OnEnemyInRange?.Invoke(true);
        }

    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Enemy Detected and out of range: " + other.gameObject.name);
            OnEnemyInRange?.Invoke(false);
        }
    }
}
