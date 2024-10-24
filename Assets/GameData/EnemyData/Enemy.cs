using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "GameData/EnemyData")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public float maxHealth;
    public float speedMultiplier = 1;
    public float speed;
    public int attackPower = 10;
    public float attackRange = 1f;
    public float knockbackForce = 0.1f;
    public float mass = 0.2f;
    public float retreatDistance = 0.5f;

}