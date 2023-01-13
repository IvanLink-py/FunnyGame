using System;
using System.Collections.Generic;
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
        PlayerControl.Main.HpChanged += UpdateHp;
        PlayerControl.Main.ArmorChanged += UpdateArmor;

        ShotingManager.Instance.ReloadBegin += OnShootingManagerEvent;
        ShotingManager.Instance.ReloadEnds += OnShootingManagerEvent;
        ShotingManager.Instance.ReloadAbort += OnShootingManagerEvent;
        ShotingManager.Instance.WeaponChange += OnShootingManagerEvent;
        ShotingManager.Instance.ShootEvent += OnShootingManagerEvent;
        
        UpdateAmmoInfo();

        UpdateIndicator(ref hpBar, PlayerControl.Main.maxHp, PlayerControl.Main.hp);
        UpdateIndicator(ref armorBar, PlayerControl.Main.armorMax, PlayerControl.Main.armor);
    }

    private void OnShootingManagerEvent(ShootingManagerEventArgs args)
    {
        UpdateAmmoInfo();
    }

    private void UpdateIndicator(ref UiBar bar, EntityIndicatorsChangedEventArgs args) =>
        UpdateIndicator(ref bar, args.MaxValue, args.NewValue);

    private void UpdateIndicator(ref UiBar bar, float max, float current)
    {
        bar.MaxValue = max;
        bar.CurrentValue = current;
    }

    private void UpdateHp(EntityIndicatorsChangedEventArgs arg0) => UpdateIndicator(ref hpBar, arg0);
    private void UpdateArmor(EntityIndicatorsChangedEventArgs arg0) => UpdateIndicator(ref armorBar, arg0);


    private void UpdateAmmoInfo()
    {
        ammoInMagLabel.text = ShotingManager.Instance.AmmoInMag.ToString();
        ammoMaxInMagLabel.text = ShotingManager.Instance.AmmoMaxInMag.ToString();
        ammoHaveLabel.text = ShotingManager.Instance.AmmoHave == 0 ? "" : ShotingManager.Instance.AmmoHave.ToString();

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