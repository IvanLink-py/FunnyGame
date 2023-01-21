using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public abstract class Destructible : MonoBehaviour
{
    public float hp;
    public float maxHp;
    public float armor;
    public float armorMax;
    public float armorAbsorption;
    public bool isDead;

    public event UnityAction<EntityHpChangedEventArgs> HpChanged;
    public event UnityAction<EntityArmorChangedEventArgs> ArmorChanged;
    public event UnityAction<EntityDeathEventArgs> Death;

    public virtual void DamageTake(float damage, [CanBeNull] GameObject source, DamageType type)
    {
        var oldStat = (Hp: hp, Armor: armor);
        
        if (damage > 0)
        {
            var absorption = armor / armorMax * armorAbsorption;


            var armorOld = armor;
            armor = Mathf.Max(0, armor - damage / armorAbsorption);
            if (armorOld - armor != 0)
                ArmorChanged?.Invoke(new EntityArmorChangedEventArgs
                {
                    Entity = this, NewValue = armor, MaxValue = armorMax
                });

            damage -= absorption;

            if (!(damage > 0)) return;
        }

        hp = Mathf.Min(hp - damage, maxHp);

        HpChanged?.Invoke(new EntityHpChangedEventArgs
        {
            Entity = this, NewValue = hp, MaxValue = maxHp
        });

        GameManager.OnHit(new Damage(
            source,
            this,
            type,
            hp - oldStat.Hp,
            armor - oldStat.Armor));

        CheckDie();
    }

    protected virtual void CheckDie()
    {
        if (!isDead && hp <= 0) Die();
    }

    protected virtual bool Die()
    {
        if (isDead) return false;
        isDead = true;

        Death?.Invoke(new EntityDeathEventArgs());

        Destroy(gameObject);
        return true;
    }
}