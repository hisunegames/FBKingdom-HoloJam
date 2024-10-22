using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "GameData/EnemyData")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public float maxHealth;
    public float speed;
    public int attackPower;
    public float attackRange;
    public float knockbackForce;
}