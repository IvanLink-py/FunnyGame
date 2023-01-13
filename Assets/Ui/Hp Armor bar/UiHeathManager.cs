using System;
using GameObjects.Player;
using UnityEngine;
using UnityEngine.UI;

public class UiHeathManager : MonoBehaviour
{
    [Header("Bars")] [SerializeField] private UiBar hpBar;

    [SerializeField] private UiBar armorBar;
    [SerializeField] private UiBar staminaBar;
    [SerializeField] private UiBar hungerBar;
    [SerializeField] private UiBar thirstBar;

    [Header("Weapon info")] [SerializeField]
    private Text ammoInMagLabel;

    [SerializeField] private Text ammoMaxInMagLabel;
    [SerializeField] private Text ammoHaveLabel;
    [SerializeField] private Image ammoTypeImage;

    private void Start()
    {
        PlayerControl.Main.HpChanged += arg0 =>
        {
            hpBar.MaxValue = PlayerControl.Main.maxHp;
            hpBar.CurrentValue = arg0.NewValue;
        };
        
        PlayerControl.Main.ArmorChanged += arg0 =>
        {
            armorBar.MaxValue = PlayerControl.Main.armorMax;
            armorBar.CurrentValue = arg0.NewValue;
        };
        
        hpBar.MaxValue = PlayerControl.Main.maxHp;
        hpBar.CurrentValue = PlayerControl.Main.hp;
        armorBar.MaxValue = PlayerControl.Main.armorMax;
        armorBar.CurrentValue = PlayerControl.Main.armor;
    }

    private void FixedUpdate()
    {
        Show();
    }

    private void Show()
    {
        ammoInMagLabel.text = ShotingManager.Instance.AmmoInMag.ToString();
        ammoMaxInMagLabel.text = ShotingManager.Instance.AmmoMaxInMag.ToString();
        ammoHaveLabel.text = ShotingManager.Instance.AmmoHave <= 1 ? "" : ShotingManager.Instance.AmmoHave.ToString();

        if (ShotingManager.Instance.AmmoPic is not null)
        {
            ammoTypeImage.sprite = ShotingManager.Instance.AmmoPic;
            ammoTypeImage.color = Color.white;
        }
        else
        {
            ammoTypeImage.sprite = ShotingManager.Instance.AmmoPic;
            ammoTypeImage.color = new Color(0, 0, 0, 0);
        }
    }
}