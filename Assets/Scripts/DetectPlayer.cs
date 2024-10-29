using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;

public class DetectPlayer : MonoBehaviour
{
    public UnityEvent<bool> FoundPlayer;
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            FoundPlayer?.Invoke(true);
            Debug.Log("Player detected!");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            FoundPlayer?.Invoke(false);
            Debug.Log("Player left!");
        }
    }
}
