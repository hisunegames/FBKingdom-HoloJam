using UnityEngine;
using UnityEngine.Events;

public class DetectPlayerMascot : MonoBehaviour
{
    public UnityEvent<bool> OnPlayerInRange;
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Debug.Log("Player Detected: " + other.gameObject.name);
            OnPlayerInRange?.Invoke(true);
        }

    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player Detected:" + other.gameObject.name);
            OnPlayerInRange?.Invoke(false);
        }
    }
}
