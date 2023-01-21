using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class Building : Destructible
{
    [SerializeField] private float startHp;
    [SerializeField] private float startArmor;
    [SerializeField] private Sprite sprite;
    
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
        HpChanged?.Invoke(new EntityHpChangedEventArgs{Entity = this, MaxValue = MaxHp, NewValue = Hp});
    }

    public void Die()
    {
        Death?.Invoke(new EntityDeathEventArgs {Entity = this});
        Destroy(gameObject);
    }
}
