using UnityEngine;

[CreateAssetMenu(fileName = "Player Stats", menuName = "GameData/PlayerStats")]
public class PlayerStatsSO : ScriptableObject
{
    [Header("Health")]
    public float MaxHealth = 100f;
    public float BuffMaxHealth = 10f;

    [Header("Mana")]
    public float MaxMana = 100f;
    public float ManaRecoverPerSec = 10f;
    public float BuffManaRecover = 1;
    [Space(10)]
    public float DashManaCost = 30f;

    [Header("Speed")]
    public float SpeedMultiplier = 1f;
    public float BuffSpeedMultiplier = 0.1f;

    [Header("Sword")]
    public float SwordDamage = 20f;
    public float BuffSwordDamage = 4f;

    [Header("Gun")]
    public float GunDamage = 10f;
    public float BuffGunDamage = 2f;
}
