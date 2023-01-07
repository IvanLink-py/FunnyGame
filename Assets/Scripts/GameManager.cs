using System;
using GameObjects.Player;
using JetBrains.Annotations;
using UnityEditor.Search;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static PlayerControl Player;

    [SerializeField] private GameObject bulletPrefab;

    private void Awake() => _instance = this;

    public static void Shoot(Vector2 pos, Vector2 dir, GunInfo gunInfo, Entity shooter)
    {
        for (var i = 0; i < gunInfo.count; i++)
        {
            var b = Instantiate(_instance.bulletPrefab).transform;
            var bullet = b.GetComponent<Bullet>();
            bullet.myInfo = gunInfo;
            bullet.shooter = shooter;


            // Scatter - Разброс 
            var resultScatter = gunInfo.scatter * (Random.value - 0.5f);
            var resultAngle = (Vector2.SignedAngle(Vector2.right, dir) + resultScatter) * Mathf.Deg2Rad;
            var resultDirection = new Vector2(Mathf.Cos(resultAngle), Mathf.Sin(resultAngle));

            b.position = pos + dir;
            bullet.velocity = resultDirection * gunInfo.speed;

            // Size
            b.localScale = Vector3.one * gunInfo.size;
        }
    }

    public delegate void HitRegister(Damage damageInfo);

    public static event HitRegister OnHitRegister;


    public static void OnHit(Damage damageInfo)
    {
        OnHitRegister?.Invoke(damageInfo);
    }
}


public class Damage
{
    public Entity Target;
    [CanBeNull] public Entity Shooter;
    
    public DamageType Type;
    
    public float HpAmount;
    public float ArmorAmount;

    public Damage([CanBeNull] Entity shooter, Entity target, DamageType type, float hpAmount, float armorAmount)
    {
        Type = type;
        HpAmount = hpAmount;
        ArmorAmount = armorAmount;
        Shooter = shooter;
        Target = target;
    }
}

public enum DamageType
{
    Melee, 
    Shoot,
    Fire
}

