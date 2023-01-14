using GameObjects.Player;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

public class Entity : MonoBehaviour, IDestructible
{
    [field: Header("HP & Armor")] public float Hp { get; set; } = 100;
    public float MaxHp { get; set; } = 100;
    public float Armor { get; set; } = 100;
    public float ArmorMax { get; set; } = 100;
    public float ArmorAbsorption { get; set; } = 10;
    public bool IsDead { get; private set; }

    [Header("Drop")] [CanBeNull] public DropTable myDropTable;

    public event UnityAction<EntityHpChangedEventArgs> HpChanged;
    public event UnityAction<EntityArmorChangedEventArgs> ArmorChanged;
    public event UnityAction<EntityDeathEventArgs> Death;
    
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

    public void Heal(float health)
    {
        DamageTake(-health, null, DamageType.Heal);
    }

    public void DamageTake(float damage, GameObject source, DamageType type)
    {
        var oldStat = (Hp, Armor);

        if (damage > 0)
        {
            var absorption = Armor / ArmorMax * ArmorAbsorption;


            var armorOld = Armor;
            Armor = Mathf.Max(0, Armor - damage / ArmorAbsorption);
            if (armorOld - Armor != 0)
                ArmorChanged?.Invoke(new EntityArmorChangedEventArgs
                {
                    Entity = this, NewValue = Armor, OldValue = armorOld, MaxValue = ArmorMax
                });

            damage -= absorption;

            if (!(damage > 0)) return;
        }

        var hpOld = Hp;
        Hp = Mathf.Min(Hp - damage, MaxHp);

        HpChanged?.Invoke(new EntityHpChangedEventArgs
        {
            Entity = this, NewValue = Hp, OldValue = hpOld, MaxValue = MaxHp
        });

        GameManager.OnHit(new Damage(
            source,
            this,
            type,
            Hp - oldStat.Hp,
            Armor - oldStat.Armor));

        if (Hp <= 0) Die();
    }

    public virtual void Die()
    {
        if (IsDead) return;
        IsDead = true;

        Death?.Invoke(new EntityDeathEventArgs());

        if (myDropTable is not null)
            foreach (var item in myDropTable.Realise())
                GameManager.ItemDrop(item, transform.position);

        Destroy(gameObject);
    }
}