using System;
using System.Collections;
using System.Collections.Generic;
using GameObjects.Player;
using UnityEngine;

public class UiHeathManager : MonoBehaviour
{
    [Header("Bars")]
    [SerializeField] private UiBar hpBar; 
    [SerializeField] private UiBar armorBar; 
    [SerializeField] private UiBar staminaBar; 
    [SerializeField] private UiBar hungerBar; 
    [SerializeField] private UiBar thirstBar;
    
    private void Show()
    {
        hpBar.CurrentValue = PlayerControl.Main.hp;
    }

    private void FixedUpdate()
    {
        Show();
    }
}
