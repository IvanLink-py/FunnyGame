using GameObjects.Player;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

public class Entity : Destructible
{
    [field: Header("HP & Armor")] 

    [Header("Drop")] [CanBeNull] public DropTable myDropTable;
    
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

    public override void DamageTake(float damage, GameObject source, DamageType type)
    {
        var oldStat = (Hp: hp, Armor: armor);

        base.DamageTake(damage, source, type);

        GameManager.OnHit(new Damage(
            source,
            this,
            type,
            hp - oldStat.Hp,
            armor - oldStat.Armor));

        CheckDie();
    }
    
    protected override bool Die()
    {
        if (!base.Die()) return false;
        if (myDropTable is null) return true;
        
        foreach (var item in myDropTable.Realise())
            GameManager.ItemDrop(item, transform.position);

        return true;
    }
}