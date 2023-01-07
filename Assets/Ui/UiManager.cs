#nullable enable
using System.Collections.Generic;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    private static UiManager _ui;
    public GameObject damageDrawPrefab;
    private static List<FloatingDamageInfo> _lastInfo = new();
    [SerializeField] private float snapRadius = 0.2f; // Радус поиска ближайшей надписи при появлении новой 


    private void Awake()
    {
        _ui = this;
    }

    private void Start()
    {
        GameManager.OnHitRegister += DrawDamage;
    }

    private void DrawDamage(Damage damageinfo)
    {
        var closestData = GetClosestInfo(damageinfo.Target.transform.position);
        if (closestData.Item1 is not null && closestData.Item2 < snapRadius)
            closestData.Item1.Amount += damageinfo.HpAmount;
        else DamageInfoAppear(damageinfo.Target.transform.position, damageinfo.HpAmount);
        
    }

    private static void DamageInfoAppear(Vector3 pos, float amount)
    {
        var floatingInfo = Instantiate(_ui.damageDrawPrefab);
        floatingInfo.transform.position = pos;
        _lastInfo.Add(floatingInfo.GetComponent<FloatingDamageInfo>());
        _lastInfo[^1].Amount = amount;
    }

    private (FloatingDamageInfo?, float?) GetClosestInfo(Vector3 pos)
    {
        if (_lastInfo.Count == 0) return (null, null);

        var closest = _lastInfo[0];
        var closestDistance = (_lastInfo[0].transform.position - pos).magnitude;

        foreach (var info in _lastInfo)
        {
            if (!((info.transform.position - pos).magnitude < closestDistance)) continue;
            closest = info;
            closestDistance = (info.transform.position - pos).magnitude;
        }

        return (closest, closestDistance);
    }

    public static void DamageInfoDisappear(FloatingDamageInfo info)
    {
        if (_lastInfo.Contains(info)) _lastInfo.Remove(info);
        Destroy(info.gameObject);
    }
}