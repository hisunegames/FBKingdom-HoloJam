using UnityEngine;
using FBK;

[CreateAssetMenu(fileName = "BulletObject", menuName = "FubukiGun/BulletObject", order = 1)]
public class BulletObject : ScriptableObject
{
    public float bulletSpeed;
    public float bulletDamage;
    public float bulletLifeTime;
    public float bulletKnockback;
    public float bulletSize;
    // public Sprite bulletSprite;
    public BulletEffect bulletEffect;
}

