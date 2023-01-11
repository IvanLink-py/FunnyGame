#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using GameObjects.Player;
using JetBrains.Annotations;
using UnityEngine;

[RequireComponent(typeof(PlayerControl))]
[RequireComponent(typeof(Inventory))]
public class ShotingManager : MonoBehaviour
{
    [SerializeField] private Transform gunFire;
    public WeaponInfo? currentGun;
    public static ShotingManager Instance;
    private float _recallTimer;
    private bool _inReload;
    private bool _canShoot = true;

    private Inventory _myInv;
    private PlayerControl _player;
    private int _ammoInMag;

    public GunInfo? CurrentGunInfo => currentGun?.gunInfo;

    public int AmmoInMag => currentGun is null ? 0 : _ammoInMag;
    public int AmmoMaxInMag => currentGun is null ? 0 : currentGun.gunInfo.ammoInMag;
    public string AmmoHave => currentGun is null ? "" : _myInv.Count(currentGun.ammoItemInfo).ToString();
    public Sprite? AmmoPic => currentGun?.ammoItemInfo.image;

    private void Awake()
    {
        Instance = this;
        _myInv = GetComponent<Inventory>();
        _player = GetComponent<PlayerControl>();
    }

    private void FixedUpdate()
    {
        ShootControl();
        CheckInventoryWeapon();
    }

    private void CheckInventoryWeapon()
    {
        if (_myInv.selectedHotBarSlot.Items is null && currentGun is not null)
        {
            SwitchGun(null);
            return;
        }

        if (_myInv.selectedHotBarSlot.Items is not null && currentGun is null)
        {
            if (_myInv.selectedHotBarSlot.Items.item.GetType() == typeof(WeaponInfo))
                SwitchGun((WeaponInfo)_myInv.selectedHotBarSlot.Items.item);
            return;
        }
        
        if (_myInv.selectedHotBarSlot.Items is not null && currentGun is not null && _myInv.selectedHotBarSlot.Items.item != currentGun)
        {
            if (_myInv.selectedHotBarSlot.Items.item.GetType() == typeof(WeaponInfo))
                SwitchGun((WeaponInfo)_myInv.selectedHotBarSlot.Items.item);
        }
    }


    private void ShootControl()
    {
        if (!UiManager.CanShoot() || currentGun is null) return;

        _recallTimer -= Time.deltaTime;
        if (_recallTimer > 0) return;

        if (UiManager.IsPointerOverUIElement()) return;

        if (Input.GetAxis("Fire1") < 0.5f && !CurrentGunInfo.isFullAmmo) _canShoot = true;
        if (Input.GetAxis("Fire1") < 0.5f) return;

        if (_ammoInMag <= 0) { Reload(); return;}
        
        if (!_canShoot) return;

        Shoot();
    }

    private void Reload()
    {
        if (_inReload) return;
        StartCoroutine(ReloadBegin());
    }

    private void Shoot()
    {
        var position = gunFire.position;
        // GameManager.Shoot(position, _mainCamera.ScreenToWorldPoint(Input.mousePosition)-position, currentGun, this);
        GameManager.Shoot(position, _player.Forward, CurrentGunInfo, _player);
        _player.MyRigidbody.AddForce(-_player.Forward * CurrentGunInfo.recoil);

        _ammoInMag--;
        _recallTimer = 1 / CurrentGunInfo.rate;

        if (!CurrentGunInfo.isFullAmmo) _canShoot = false;
    }

    public void SwitchGun(WeaponInfo? newGun)
    {
        currentGun = newGun;
        _ammoInMag = 0;
        if (newGun is not null)
            _recallTimer = CurrentGunInfo.reloadTime;
    }

    private IEnumerator ReloadBegin()
    {
        _inReload = true;
        yield return new WaitForSeconds(CurrentGunInfo.reloadTime);
        _ammoInMag += _myInv.TryTake(currentGun.ammoItemInfo, CurrentGunInfo.ammoInMag);
        _inReload = false;
    }
}