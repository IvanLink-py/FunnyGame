using GameObjects.Player;
using JetBrains.Annotations;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public delegate void HitRegister(Damage damageInfo);

    private static GameManager _instance;
    public static PlayerControl Player;

    public ItemDB itemDb;
    public GameObject droppedItemPrefab;

    public static ItemDB MainItemDB => _instance.itemDb;

    private void Awake()
    {
        _instance = this;
    }

    public static void Shoot(Vector2 pos, Vector2 dir, GunInfo gunInfo, Entity shooter)
    {
        for (var i = 0; i < gunInfo.count; i++)
        {
            var b = Instantiate(gunInfo.bulletPrefab).transform;
            var bullet = b.GetComponent<Bullet>();
            bullet.myInfo = gunInfo;
            bullet.shooter = shooter;


            // Scatter - Разброс 
            var resultScatter = gunInfo.scatter * (Random.value - 0.5f);
            var resultAngle = (Vector2.SignedAngle(Vector2.right, dir.normalized) + resultScatter) * Mathf.Deg2Rad;
            var resultDirection = new Vector2(Mathf.Cos(resultAngle), Mathf.Sin(resultAngle));

            b.position = pos;
            bullet.velocity = resultDirection * gunInfo.speed;

            // Size
            b.localScale = Vector3.one * gunInfo.size;
        }
    }

    public static event HitRegister OnHitRegister;


    public static void OnHit(Damage damageInfo)
    {
        OnHitRegister?.Invoke(damageInfo);
    }

    public static void ItemDrop(Items items, Vector3 pos)
    {
        var obj = Instantiate(_instance.droppedItemPrefab, pos, Quaternion.identity);
        obj.GetComponent<DropedItem>().items = items;
        obj.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.value - 0.5f, Random.value - 0.5f);
    }
}


public class Damage
{
    public float ArmorAmount;

    public float HpAmount;
    [CanBeNull] public Entity Shooter;
    public Entity Target;

    public DamageType Type;

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
    Fire,
    Heal
}