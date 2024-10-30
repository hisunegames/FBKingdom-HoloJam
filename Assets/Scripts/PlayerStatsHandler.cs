using UnityEngine;

public static class PlayerStatsHandler
{
    public const string MAX_HEALTH_KEY = "MaxHealth";
    public const string MAX_MANA_KEY = "MaxMana";
    public const string SPEED_KEY = "Speed";
    public const string SWORD_DAMAGE_KEY = "SwordDamage";
    public const string GUN_DAMAGE_KEY = "GunDamage";

    private const float maxHealth = 100f;
    private const float maxMana = 100f;
    private const float speed = 1f;
    private const float swordDamage = 20f;
    private const float gunDamage = 10f;

    public static void ResetPlayerBuff()
    {
        PlayerPrefs.DeleteKey(MAX_HEALTH_KEY);
        PlayerPrefs.DeleteKey(MAX_MANA_KEY);
        PlayerPrefs.DeleteKey(SPEED_KEY);
        PlayerPrefs.DeleteKey(SWORD_DAMAGE_KEY);
        PlayerPrefs.DeleteKey(GUN_DAMAGE_KEY);
    }

    public static PlayerBuff GetPlayerBuff()
    {
        if (!PlayerPrefs.HasKey(MAX_HEALTH_KEY))
        {
            PlayerPrefs.SetFloat(MAX_HEALTH_KEY, maxHealth);
            PlayerPrefs.SetFloat(MAX_MANA_KEY, maxMana);
            PlayerPrefs.SetFloat(SPEED_KEY, speed);
            PlayerPrefs.SetFloat(SWORD_DAMAGE_KEY, swordDamage);
            PlayerPrefs.SetFloat(GUN_DAMAGE_KEY, gunDamage);
        }

        return new(PlayerPrefs.GetFloat(MAX_HEALTH_KEY),
                   PlayerPrefs.GetFloat(MAX_MANA_KEY),
                   PlayerPrefs.GetFloat(SPEED_KEY),
                   PlayerPrefs.GetFloat(SWORD_DAMAGE_KEY),
                   PlayerPrefs.GetFloat(GUN_DAMAGE_KEY)
        );
    }

    public static void SetPlayerBuff(PlayerBuff buff)
    {
        PlayerPrefs.SetFloat(MAX_HEALTH_KEY, buff.MaxHealth);
        PlayerPrefs.SetFloat(MAX_MANA_KEY, buff.MaxMana);
        PlayerPrefs.SetFloat(SPEED_KEY, buff.Speed);
        PlayerPrefs.SetFloat(SWORD_DAMAGE_KEY, buff.SwordDamage);
        PlayerPrefs.SetFloat(GUN_DAMAGE_KEY, buff.GunDamage);
    }
}

public class PlayerBuff
{
    public float MaxHealth;
    public float MaxMana;
    public float Speed;
    public float SwordDamage;
    public float GunDamage;

    public PlayerBuff(float maxHealth, float maxMana, float speed, float swordDamage, float gunDamage)
    {
        MaxHealth = maxHealth;
        MaxMana = maxMana;
        Speed = speed;
        SwordDamage = swordDamage;
        GunDamage = gunDamage;
    }
}
