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
        PlayerPrefs.SetInt(MAX_HEALTH_KEY, 0);
        PlayerPrefs.SetInt(RECOVER_MANA_KEY, 0);
        PlayerPrefs.SetInt(SPEED_KEY, 0);
        PlayerPrefs.SetInt(SWORD_DAMAGE_KEY, 0);
        PlayerPrefs.SetInt(GUN_DAMAGE_KEY, 0);
    }

    public static PlayerBuff GetPlayerBuff()
    {
        if (!PlayerPrefs.HasKey(MAX_HEALTH_KEY))
        {
            ResetPlayerBuff();
        }

        return new(PlayerPrefs.GetInt(MAX_HEALTH_KEY),
                   PlayerPrefs.GetInt(RECOVER_MANA_KEY),
                   PlayerPrefs.GetInt(SPEED_KEY),
                   PlayerPrefs.GetInt(SWORD_DAMAGE_KEY),
                   PlayerPrefs.GetInt(GUN_DAMAGE_KEY)
        );
    }

    public static void SavePlayerBuff(PlayerBuff buff)
    {
        PlayerPrefs.SetInt(MAX_HEALTH_KEY, buff.MaxHealth);
        PlayerPrefs.SetInt(RECOVER_MANA_KEY, buff.RecoverMana);
        PlayerPrefs.SetInt(SPEED_KEY, buff.Speed);
        PlayerPrefs.SetInt(SWORD_DAMAGE_KEY, buff.SwordDamage);
        PlayerPrefs.SetInt(GUN_DAMAGE_KEY, buff.GunDamage);
    }

    public static int GetBuffCount()
    {
        return GetPlayerBuff().GetSum();
    }
}

public class PlayerBuff
{
    public int MaxHealth;
    public int RecoverMana;
    public int Speed;
    public int SwordDamage;
    public int GunDamage;

    public PlayerBuff()
    {
        MaxHealth = 0;
        RecoverMana = 0;
        Speed = 0;
        SwordDamage = 0;
        GunDamage = 0;
    }

    public PlayerBuff(int maxHealth, int recoverMana, int speed, int swordDamage, int gunDamage)
    {
        MaxHealth = maxHealth;
        RecoverMana = recoverMana;
        Speed = speed;
        SwordDamage = swordDamage;
        GunDamage = gunDamage;
    }

    public int GetSum()
    {
        return MaxHealth + RecoverMana + Speed + SwordDamage + GunDamage;
    }
}
