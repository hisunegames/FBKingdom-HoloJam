using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "GameData/EnemyData")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public float maxHealth;
    public float speedMultiplier = 1;
    public float speed;
    public int attackPower;
    public float attackRange;
    public float knockbackForce;
    public float mass = 0.2f;
    public float retreatDistance = 0.5f;

}