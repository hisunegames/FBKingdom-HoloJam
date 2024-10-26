using UnityEngine;
using UnityEngine.Events;

public class CheckTriggerCollider : MonoBehaviour
{
    public UnityEvent<Collider2D> StayingInTrigger;
    private void OnTriggerStay2D(Collider2D other)
    {
        Debug.Log("Triggered: " + gameObject.name);
        StayingInTrigger?.Invoke(other);

    }
}
