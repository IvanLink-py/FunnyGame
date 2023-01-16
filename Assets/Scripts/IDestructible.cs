using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

public interface IDestructible
{
    float Hp { get; }
    float MaxHp { get; }
    float Armor { get; }
    float ArmorMax { get; }
    float ArmorAbsorption { get; }
    bool IsDead { get; }
    
    event UnityAction<EntityHpChangedEventArgs> HpChanged;
    event UnityAction<EntityArmorChangedEventArgs> ArmorChanged;
    event UnityAction<EntityDeathEventArgs> Death;

    void DamageTake(float damage, [CanBeNull] GameObject source, DamageType type);
    void Die();
    
    

}