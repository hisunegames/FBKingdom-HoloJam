using System.Collections;
using UnityEngine;

public class FubuzillaSkill : MonoBehaviour
{
    public SpriteRenderer laserSpriteRenderer;
    public float sizeOffset = 0.1f;
    public float damagePerSecond = 10f;
    private float accumulatedDamage = 0f;
    public bool isShooting = false;
    public float skillCooldown = 5f;
    public bool onCooldown = false;
    public float skillDuration = 3f;

    public ParticleSystem p_System;
    public ParticleSystem.EmissionModule emission;

    private int enemyLayerMask;

    void Awake()
    {
        p_System = GetComponentInChildren<ParticleSystem>();
        emission = p_System.emission;
        enemyLayerMask = LayerMask.GetMask("Enemy");
    }

    void Start()
    {
        laserSpriteRenderer.enabled = false;
        emission.enabled = false;
    }

    void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.nearClipPlane;
        Vector3 targetPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        targetPosition.z = 0;

        if (Input.GetKey(KeyCode.Q))
        {
            if (!onCooldown && !isShooting)
            {
                CameraManager.instance.SetActiveCamera(CameraManager.instance.fubuzillaCamera);
                StartCoroutine(UseSkill());
            }
        }
        if (isShooting)
        {
            ShootLaser(targetPosition);
            DealDamage(targetPosition);
        }
        else
        {
            laserSpriteRenderer.enabled = false;
            emission.enabled = false;
        }
    }

    void ShootLaser(Vector3 targetPosition)
    {
        Vector3 startPoint = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0));
        startPoint.z = 0;

        Vector3 direction = targetPosition - startPoint;
        RaycastHit2D hit = Physics2D.Raycast(startPoint, direction, Mathf.Infinity, enemyLayerMask);

        ParticleSystem.EmissionModule emission = p_System.emission;
        if (hit.collider != null)
        {
            Vector3 hitPoint = hit.point;
            float distance = Vector3.Distance(startPoint, hitPoint);
            Vector3 midPoint = startPoint + direction.normalized * (distance / 2);

            // Get the normal of the hit surface
            Vector3 hitNormal = hit.normal;
            // Calculate the angle between the laser direction and the hit normal
            float angle = Vector3.Angle(direction, hitNormal);

            // Set the rotation of the particle system based on the angle
            Vector3 p_rotation = new Vector3(angle, 0, 0);
            p_System.gameObject.transform.rotation = Quaternion.Euler(p_rotation);
            p_System.gameObject.transform.position = hitPoint;
            emission.enabled = true;

            // Set the rotation of the laser sprite based on the angle
            laserSpriteRenderer.size = new Vector2(distance + sizeOffset, laserSpriteRenderer.size.y);
            laserSpriteRenderer.transform.SetPositionAndRotation(midPoint, Quaternion.FromToRotation(Vector3.right, direction));
            // laserSpriteRenderer.transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        else
        {
            emission.enabled = false;
        }
    }

    void DealDamage(Vector3 targetPosition)
    {
        Vector3 startPoint = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0));
        startPoint.z = 0;

        Vector3 direction = targetPosition - startPoint;
        RaycastHit2D hit = Physics2D.Raycast(startPoint, direction, Mathf.Infinity, enemyLayerMask);

        if (hit.collider != null && hit.collider.CompareTag("Enemy"))
        {
            accumulatedDamage = damagePerSecond * Time.deltaTime;
            hit.collider.gameObject.GetComponent<Health>().Damage(accumulatedDamage);
        }
        else
        {
            Debug.Log("No enemy hit");
        }
    }

    IEnumerator UseSkill()
    {
        isShooting = true;
        laserSpriteRenderer.enabled = true;
        yield return new WaitForSeconds(skillDuration);
        CameraManager.instance.SetActiveCamera(CameraManager.instance.defaultCamera);
        isShooting = false;
        onCooldown = true;
        yield return new WaitForSeconds(skillCooldown);
        onCooldown = false;
        accumulatedDamage = 0;
    }
}
