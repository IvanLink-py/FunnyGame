using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    private UiManager _ui;
    public GameObject damageDrawPrefab;

    private void Start()
    {
        GameManager.OnHitRegister += DrawDamage;
    }

    private void DrawDamage(Damage damageinfo)
    {
        var floatingInfo = Instantiate(damageDrawPrefab);
        floatingInfo.transform.position = damageinfo.Target.transform.position;
        floatingInfo.GetComponent<FloatingDamageInfo>().Amount = damageinfo.HpAmount;
    }
    
}
