using GameObjects.Player;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

public class Entity : MonoBehaviour
{
    [Header("HP & Armor")] public float hp;
    public float maxHp;
    public float armor;
    public float armorMax;
    public float armorAbsorption;
    private bool _isDead;

    [Header("Drop")] [CanBeNull] public DropTable myDropTable;

    public event UnityAction<EntityHpChangedEventArgs> HpChanged;
    public event UnityAction<EntityArmorChangedEventArgs> ArmorChanged;
    public event UnityAction<EntityDeathEventArgs> Death;


    public void OnBulletHit(Bullet bullet)
    {
        OnDamageTake(bullet.myInfo.damage, bullet.shooter, DamageType.Shoot);
    }

    public void OnMeleeHit(EnemyConrol enemy)
    {
        OnDamageTake(enemy.damage, enemy, DamageType.Melee);
    }

    public void OnEnvHit(float damage)
    {
        OnDamageTake(damage, null, DamageType.Env);
    }

    public void OnExplosionHit(Explosion explosion)
    {
        OnDamageTake(explosion.damage, PlayerControl.Main, DamageType.Explosion);
    }

    public Vector3 Forward
    {
        get
        {
            var z = transform.rotation.eulerAngles.z * Mathf.Deg2Rad + Mathf.PI / 2;
            return Vector3.up * Mathf.Sin(z) + Vector3.right * Mathf.Cos(z);
        }
    }

    protected void LookAt(Vector3 pos)
    {
        LookAt(pos, transform.position);
    }

    protected void LookAt(Vector3 pos, Vector3 offset)
    {
        var diff = pos - transform.position + (transform.position - offset);

        // ↓ Костыль ↓ 

        if (diff.magnitude < 1.5f) diff = pos - offset + (offset - transform.position);

        // ↑ Костыль ↑ 

        // Метод LookAt вызывает осцилляцию игрового персонажа при прицеливании слишком близко.
        // Суть костыля в замене способа поворота при малой дальности прицеливания.
        // Для воспроизведения бага закомментируйте строчку выше
        // Гарантированно вознаграждение за исправление

        var targetRotation = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, targetRotation - 90);
    }

    public void Heal(float health, [CanBeNull] Entity source)
    {
        OnDamageTake(-health, source, DamageType.Heal);
    }

    private void OnDamageTake(float damage, [CanBeNull] Entity source, DamageType type)
    {
        var oldStat = (hp, armor);

        if (damage > 0)
        {
            var absorption = armor / armorMax * armorAbsorption;


            var armorOld = armor;
            armor = Mathf.Max(0, armor - damage / armorAbsorption);
            if (armorOld - armor != 0)
                ArmorChanged?.Invoke(new EntityArmorChangedEventArgs
                {
                    Entity = this, NewValue = hp, OldValue = armorOld, MaxValue = armorMax
                });

            damage -= absorption;

            if (!(damage > 0)) return;
        }

        var hpOld = hp;
        hp = Mathf.Min(hp - damage, maxHp);

        HpChanged?.Invoke(new EntityHpChangedEventArgs
        {
            Entity = this, NewValue = hp, OldValue = hpOld, MaxValue = maxHp
        });

        GameManager.OnHit(new Damage(
            source,
            this,
            type,
            hp - oldStat.hp,
            armor - oldStat.armor));

        if (hp <= 0) Die();
    }

    protected virtual void Die()
    {
        if (_isDead) return;
        _isDead = true;
        
        Death?.Invoke(new EntityDeathEventArgs());

        if (myDropTable is not null)
            foreach (var item in myDropTable.Realise())
                GameManager.ItemDrop(item, transform.position);

        Destroy(gameObject);
    }
}