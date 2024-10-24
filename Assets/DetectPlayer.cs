using NUnit.Framework;
using UnityEngine;

public class DetectPlayer : MonoBehaviour
{

    public bool inRange = false;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            inRange = true;
            Debug.Log("Player detected!");
        }
    }

}
