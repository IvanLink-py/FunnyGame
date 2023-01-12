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

    private bool _inMoving;
    private bool _inReload;
    private bool _inWaitingForTriggerRelease;

    private bool _canShoot = true;

    private Inventory _myInv;
    private PlayerControl _player;
    private int _ammoInMag;

    public GunInfo? CurrentGunInfo => currentGun?.gunInfo;

    public int AmmoInMag => currentGun is null ? 0 : _ammoInMag;
    public int AmmoMaxInMag => currentGun is null ? 0 : currentGun.gunInfo.ammoInMag;
    public int AmmoHave => currentGun is null ? 0 : _myInv.Count(currentGun.ammoItemInfo);
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

    private void Update()
    {
        CheckReload();
    }

    private void CheckReload()
    {
        if (Input.GetKeyDown(KeyCode.R)) Reload();
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

        if (_myInv.selectedHotBarSlot.Items is not null && currentGun is not null &&
            _myInv.selectedHotBarSlot.Items.item != currentGun)
        {
            if (_myInv.selectedHotBarSlot.Items.item.GetType() == typeof(WeaponInfo))
                SwitchGun((WeaponInfo)_myInv.selectedHotBarSlot.Items.item);
            else
                SwitchGun(null);
            
        }
    }


    private void ShootControl()
    {
        if (!UiManager.CanShoot() || currentGun is null || _inMoving) return;

        if (Input.GetAxis("Fire1") < 0.5f || CurrentGunInfo.isFullAmmo) _inWaitingForTriggerRelease = false;
        if (Input.GetAxis("Fire1") < 0.5f) return;

        if (_ammoInMag <= 0)
        {
            Reload();
            return;
        }

        if (_inWaitingForTriggerRelease) return;

        Shoot();
    }

    private void Reload()
    {
        if (_inReload || AmmoHave == 0) return;
        StartCoroutine(ReloadCoroutine());
    }

    private void Moving()
    {
        if (_inMoving) return;
        StartCoroutine(MovingCoroutine());
    }

    private void Shoot()
    {
        var position = gunFire.position;

        GameManager.Shoot(position, _player.Forward, CurrentGunInfo, _player);
        _player.MyRigidbody.AddForce(-_player.Forward * CurrentGunInfo.recoil);

        Moving();
        _ammoInMag--;

        if (!CurrentGunInfo.isFullAmmo) _inWaitingForTriggerRelease = true;
        if (_ammoInMag == 0) Reload();
    }

    public void SwitchGun(WeaponInfo? newGun)
    {
        if (currentGun is not null)
            _myInv.PutOrDrop(new Items { item = currentGun.ammoItemInfo, count = _ammoInMag }, transform.position);

        currentGun = newGun;
        _ammoInMag = 0;
        
        StopAllCoroutines();
        
        _inMoving = false;
        _inReload = false;
        _inWaitingForTriggerRelease = false;

        if (newGun is null) return;
        _recallTimer = CurrentGunInfo.reloadTime;
        Reload();
    }

    private IEnumerator ReloadCoroutine()
    {
        _inReload = true;
        _myInv.PutOrDrop(new Items { item = currentGun.ammoItemInfo, count = _ammoInMag }, transform.position);
        _ammoInMag = 0;
        
        yield return new WaitForSeconds(CurrentGunInfo.reloadTime);
        
        _ammoInMag += _myInv.TryTake(currentGun.ammoItemInfo, CurrentGunInfo.ammoInMag);
        _inReload = false;
    }

    private IEnumerator MovingCoroutine()
    {
        _inMoving = true;
        yield return new WaitForSeconds(1 / CurrentGunInfo.rate);
        _inMoving = false;
    }
}