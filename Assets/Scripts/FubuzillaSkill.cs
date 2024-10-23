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


    void Start()
    {
        laserSpriteRenderer.enabled = false;
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
        }
    }

    void ShootLaser(Vector3 targetPosition)
    {
        Vector3 startPoint = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0));
        startPoint.z = 0;

        Vector3 direction = targetPosition - startPoint;
        float distance = direction.magnitude;
        Vector3 midPoint = startPoint + direction / 2;

        laserSpriteRenderer.size = new Vector2(distance + sizeOffset, laserSpriteRenderer.size.y);
        laserSpriteRenderer.transform.SetPositionAndRotation(midPoint, Quaternion.FromToRotation(Vector3.right, direction));
    }

    void DealDamage(Vector3 targetPosition)
    {
        accumulatedDamage = damagePerSecond * Time.deltaTime;
        Collider2D targetCollider = Physics2D.OverlapPoint(targetPosition);
        if (targetCollider != null && targetCollider.CompareTag("Enemy"))
        {
            targetCollider.GetComponent<Health>().Damage(accumulatedDamage);
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
