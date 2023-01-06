using UnityEngine;

public class Entity : MonoBehaviour
{
    public float hp;
    public float maxHp;
    public float armor;
    public float armorAbsorption;

    public void OnBulletHit(Bullet bullet)
    {
        OnDamageTake(bullet.myInfo.damage);
    }

    private void OnDamageTake(float damage)
    {
        damage -= armor * armorAbsorption;

        if (!(damage > 0)) return;
        hp -= damage;
        armor = Mathf.Max(0, armor - 1 / 20f);
        
        if (hp <= 0) Destroy(gameObject);
    }
}