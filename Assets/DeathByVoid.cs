using UnityEngine;

public class DeathByVoid : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CameraManager.instance.DisableCameraFollow();
            PlayerScript.instance.Damage(9999);
        }
    }
}
