using UnityEngine;
using System.Collections.Generic;

public class LookForOffScreenEnemies : MonoBehaviour
{
    public GameObject player;
    public GameObject directionalSpritePrefab;
    public float orbitRadius = 5.0f;
    public float playerYOffset;

    private List<GameObject> directionalSprites = new List<GameObject>();

    private void Update()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Vector3 playerPosition = player.transform.position;
        List<Vector3> offScreenEnemyDirections = new List<Vector3>();

        foreach (GameObject enemy in enemies)
        {
            Vector3 enemyPosition = enemy.transform.position;
            Vector3 directionToEnemy = enemyPosition - playerPosition;

            if (!IsOnScreen(enemyPosition))
            {
                offScreenEnemyDirections.Add(directionToEnemy);
            }
        }

        // Ensure we have enough directional sprites
        while (directionalSprites.Count < offScreenEnemyDirections.Count)
        {
            GameObject newSprite = Instantiate(directionalSpritePrefab, transform);
            directionalSprites.Add(newSprite);
        }

        // Update and position directional sprites
        for (int i = 0; i < directionalSprites.Count; i++)
        {
            if (i < offScreenEnemyDirections.Count)
            {
                directionalSprites[i].SetActive(true);
                Vector3 direction = offScreenEnemyDirections[i].normalized;
                Vector3 _playerPosition = new Vector3(playerPosition.x, playerPosition.y + playerYOffset, playerPosition.z);
                directionalSprites[i].transform.position = _playerPosition + direction * orbitRadius;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                directionalSprites[i].transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            }
            else
            {
                directionalSprites[i].SetActive(false);
            }
        }
    }

    private bool IsOnScreen(Vector3 position)
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(position);
        return screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
    }
}