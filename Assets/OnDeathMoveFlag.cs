using UnityEditor;
using UnityEngine;

public class OnDeathMoveFlag : MonoBehaviour
{
    public GameObject flag;
    [SerializeField]
    private Vector2 flagPosition;
    private Health health;
    private void Start()
    {
        health = GetComponent<Health>();
        health.OnDeath.AddListener(MoveFlag);
    }
    void MoveFlag()
    {
        flag.transform.position = flagPosition;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(flagPosition, 0.5f);
    }
}
