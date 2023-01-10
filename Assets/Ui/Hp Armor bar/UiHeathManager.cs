using System;
using System.Collections;
using System.Collections.Generic;
using GameObjects.Player;
using UnityEngine;
using UnityEngine.UI;

public class UiHeathManager : MonoBehaviour
{
    [Header("Aim")] 
    [SerializeField] private Image aim;
    [Space]

    [Header("Bars")]
    [SerializeField] private UiBar hpBar; 
    [SerializeField] private UiBar armorBar; 
    [SerializeField] private UiBar staminaBar; 
    [SerializeField] private UiBar hungerBar; 
    [SerializeField] private UiBar thirstBar;

    private void Start()
    {
        Cursor.visible = false;
    }

    private void Show()
    {
        hpBar.MaxValue = PlayerControl.Main.maxHp;
        hpBar.CurrentValue = PlayerControl.Main.hp;

        armorBar.CurrentValue = PlayerControl.Main.armor;
        armorBar.MaxValue = PlayerControl.Main.armorMax;
    }

    private void FixedUpdate()
    {
        Show();
        UpdateAimPos();
    }

    private void UpdateAimPos()
    {
        aim.rectTransform.position = Input.mousePosition;
    }
    
    
}
