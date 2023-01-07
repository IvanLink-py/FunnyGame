using JetBrains.Annotations;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public float hp;
    public float maxHp;
    public float armor;
    public float armorAbsorption;

    public void OnBulletHit(Bullet bullet)
    {
        OnDamageTake(bullet.myInfo.damage, bullet.shooter, DamageType.Shoot);
    }

    public void OnMeleeHit(EnemyConrol enemy)
    {
        OnDamageTake(enemy.damage, enemy, DamageType.Melee);
    }

    protected Vector3 Forward
    {
        get
        {
            var z = transform.rotation.eulerAngles.z * Mathf.Deg2Rad + Mathf.PI / 2;
            return Vector3.up * Mathf.Sin(z) + Vector3.right * Mathf.Cos(z);
        }
    }

    protected void LookAt(Vector3 pos)
    {
        var diff = pos - transform.position;
        diff.Normalize();

        var targetRotation = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, targetRotation - 90);
    }

    private void OnDamageTake(float damage, [CanBeNull] Entity source, DamageType type)
    {
        var oldStat = (hp, armor);

        damage -= armor * armorAbsorption;

        if (!(damage > 0)) return;
        hp = Mathf.Min(hp - damage, maxHp);
        armor = Mathf.Max(0, armor - 1 / 20f);

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
        Destroy(gameObject);
    }
}