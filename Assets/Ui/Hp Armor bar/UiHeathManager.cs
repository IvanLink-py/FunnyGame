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

    private void FixedUpdate()
    {
        Show();
    }

    private void Show()
    {
        hpBar.MaxValue = PlayerControl.Main.maxHp;
        hpBar.CurrentValue = PlayerControl.Main.hp;

        armorBar.CurrentValue = PlayerControl.Main.armor;
        armorBar.MaxValue = PlayerControl.Main.armorMax;

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