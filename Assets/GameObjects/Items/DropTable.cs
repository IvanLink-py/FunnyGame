using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "DropTable", menuName = "Drop table")]
public class DropTable : ScriptableObject
{
    public List<DropTableRecord> records;

    public List<Items> Realise()
        => records.Select(record => record.Realise()).Where(a => a is not null).ToList();
}

[Serializable]
public class DropTableRecord
{
    public ItemInfo item;
    [Range(0, 1)] public float rare;
    public int minAmount;
    public int maxAmount;

    [CanBeNull]
    public Items Realise()
    {
        return Random.value > rare ? null : new Items { item = item, count = Random.Range(minAmount, maxAmount) };
    }
}