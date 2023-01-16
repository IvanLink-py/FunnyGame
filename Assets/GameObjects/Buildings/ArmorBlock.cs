using UnityEngine;
using UnityEngine.Events;

public abstract class ArmorBlock : MonoBehaviour, IBuilding
{
    public float Hp { get; set; }
    public float MaxHp { get; set; }
    public float Armor { get; set; }
    public float ArmorMax { get; set; }
    public float ArmorAbsorption { get; set; }
    public bool IsDead { get; set; }
    public event UnityAction<EntityHpChangedEventArgs> HpChanged;
    public event UnityAction<EntityArmorChangedEventArgs> ArmorChanged;
    public event UnityAction<EntityDeathEventArgs> Death;

    public void DamageTake(float damage, GameObject source, DamageType type)
    {
        Hp -= damage;
        
        HpChanged?.Invoke(new EntityHpChangedEventArgs
        {
            Entity = this, MaxValue = MaxHp, NewValue = Hp, OldValue = Hp + damage
        });
        
        if (Hp <= 0) Die();
    }

    public void Die()
    {
        if (IsDead) return;
        IsDead = true;
        
        
        Death?.Invoke(new EntityDeathEventArgs {Entity = this});
        Destroy(gameObject);
    }

    public Sprite Sprite { get; set; }
    public RotateMode RotateMode { get; set; }

    public virtual void Place()
    {
        Hp = MaxHp;
    }

    public bool CanRepair()
    {
        return Hp < MaxHp;
    }

    public void Repair(float power)
    {
        Hp = Mathf.Min(Hp + power, MaxHp);
    }
}