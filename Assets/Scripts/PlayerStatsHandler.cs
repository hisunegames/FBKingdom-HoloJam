using UnityEngine;

public static class PlayerStatsHandler
{
    public const string MAX_HEALTH_KEY = "MaxHealth";
    public const string RECOVER_MANA_KEY = "RecoverMana";
    public const string SPEED_KEY = "Speed";
    public const string SWORD_DAMAGE_KEY = "SwordDamage";
    public const string GUN_DAMAGE_KEY = "GunDamage";

    public static void ResetPlayerBuff()
    {
        PlayerPrefs.SetFloat(MAX_HEALTH_KEY, 0);
        PlayerPrefs.SetFloat(RECOVER_MANA_KEY, 0);
        PlayerPrefs.SetFloat(SPEED_KEY, 0);
        PlayerPrefs.SetFloat(SWORD_DAMAGE_KEY, 0);
        PlayerPrefs.SetFloat(GUN_DAMAGE_KEY, 0);
    }

    public static PlayerBuff GetPlayerBuff()
    {
        if (!PlayerPrefs.HasKey(MAX_HEALTH_KEY))
        {
            ResetPlayerBuff();
        }

        return new(PlayerPrefs.GetFloat(MAX_HEALTH_KEY),
                   PlayerPrefs.GetFloat(RECOVER_MANA_KEY),
                   PlayerPrefs.GetFloat(SPEED_KEY),
                   PlayerPrefs.GetFloat(SWORD_DAMAGE_KEY),
                   PlayerPrefs.GetFloat(GUN_DAMAGE_KEY)
        );
    }

    public static void SetPlayerBuff(PlayerBuff buff)
    {
        PlayerPrefs.SetFloat(MAX_HEALTH_KEY, buff.MaxHealth);
        PlayerPrefs.SetFloat(RECOVER_MANA_KEY, buff.RecoverMana);
        PlayerPrefs.SetFloat(SPEED_KEY, buff.Speed);
        PlayerPrefs.SetFloat(SWORD_DAMAGE_KEY, buff.SwordDamage);
        PlayerPrefs.SetFloat(GUN_DAMAGE_KEY, buff.GunDamage);
    }
}

public class PlayerBuff
{
    public float MaxHealth;
    public float RecoverMana;
    public float Speed;
    public float SwordDamage;
    public float GunDamage;

    public PlayerBuff()
    {
        MaxHealth = 0;
        RecoverMana = 0;
        Speed = 0;
        SwordDamage = 0;
        GunDamage = 0;
    }

    public PlayerBuff(float maxHealth, float recoverMana, float speed, float swordDamage, float gunDamage)
    {
        MaxHealth = maxHealth;
        RecoverMana = recoverMana;
        Speed = speed;
        SwordDamage = swordDamage;
        GunDamage = gunDamage;
    }
}
