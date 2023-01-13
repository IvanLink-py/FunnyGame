#nullable enable
using System.Collections;
using GameObjects.Player;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(PlayerControl))]
[RequireComponent(typeof(Inventory))]
public class ShotingManager : MonoBehaviour
{
    [SerializeField] private Transform gunFire;
    public WeaponInfo? currentGun;
    public static ShotingManager Instance;

    private bool _inMoving;
    private bool _inReload;
    private bool _inWaitingForTriggerRelease;

    private bool _reloadBreak;
    private IEnumerator _reloadRoutine;

    private bool _canShoot = true;

    private Inventory _myInv;
    private PlayerControl _player;
    private int _ammoInMag;

    public event UnityAction<ShootEventArgs> ShootEvent;
    public event UnityAction<ReloadBeginEventArgs> ReloadBegin;
    public event UnityAction<ReloadEndsEventArgs> ReloadEnds;
    public event UnityAction<ReloadAbortEventArgs> ReloadAbort;
    public event UnityAction<WeaponChangeEventArgs> WeaponChange;

    public GunInfo? CurrentGunInfo => currentGun?.gunInfo;

    public int AmmoInMag => currentGun is null ? 0 : _ammoInMag;
    public int AmmoMaxInMag => currentGun is null ? 0 : currentGun.gunInfo.ammoInMag;
    public int AmmoHave => (currentGun is null ? 0 : _myInv.Count(currentGun.ammoItemInfo)) + _ammoInMag;
    public Sprite? AmmoPic => currentGun?.ammoItemInfo.image;

    private void Awake()
    {
        Instance = this;
        _myInv = GetComponent<Inventory>();
        _player = GetComponent<PlayerControl>();
    }

    private void Start()
    {
        _myInv.ActiveSlotContentChanged += _ => CheckInventoryWeapon();
        CheckInventoryWeapon();
    }

    private void Update()
    {
        ShootControl();
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

        if (CurrentGunInfo.isSingleAmmoLoad)
        {
            StopCoroutine(_reloadRoutine);
            _inReload = false;
            ReloadAbort?.Invoke(new ReloadAbortEventArgs {CurrentAmmoInMag = _ammoInMag});
        }

        if (_inReload && !CurrentGunInfo.isSingleAmmoLoad) return;

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
        if (_inReload || AmmoHave == 0 ||
            (CurrentGunInfo.isSingleAmmoLoad && _ammoInMag == CurrentGunInfo.ammoInMag)) return;
        _reloadRoutine = ReloadCoroutine();
        StartCoroutine(_reloadRoutine);
        ReloadBegin?.Invoke(new ReloadBeginEventArgs
        {
            CurrentAmmoInMag = _ammoInMag,
            Duration = CurrentGunInfo.isSingleAmmoLoad
                ? CurrentGunInfo.reloadTime * (CurrentGunInfo.ammoInMag - _ammoInMag)
                : CurrentGunInfo.reloadTime
        });
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

        ShootEvent?.Invoke(new ShootEventArgs { CurrentAmmoInMag = _ammoInMag });

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
        
        WeaponChange?.Invoke(new WeaponChangeEventArgs
        {
            CurrentAmmoInMag = 0, CurrentAmmoType = newGun is null? null : newGun.ammoItemInfo
        });

        if (newGun is null) return;
        Reload();
    }

    private IEnumerator ReloadCoroutine()
    {
        _inReload = true;
        if (!CurrentGunInfo.isSingleAmmoLoad)
        {
            _myInv.PutOrDrop(new Items { item = currentGun.ammoItemInfo, count = _ammoInMag }, transform.position);
            _ammoInMag = 0;
        }

        yield return new WaitForSeconds(CurrentGunInfo.reloadTime);

        _ammoInMag += _myInv.TryTake(currentGun.ammoItemInfo,
            CurrentGunInfo.isSingleAmmoLoad ? 1 : CurrentGunInfo.ammoInMag);

        if (_ammoInMag < CurrentGunInfo.ammoInMag && CurrentGunInfo.isSingleAmmoLoad)
        {
            _reloadRoutine = ReloadCoroutine();
            StartCoroutine(_reloadRoutine);
        }
        else
        {
            _inReload = false;
            ReloadEnds?.Invoke(new ReloadEndsEventArgs {CurrentAmmoInMag = _ammoInMag});
        }
    }

    private IEnumerator MovingCoroutine()
    {
        _inMoving = true;
        yield return new WaitForSeconds(1 / CurrentGunInfo.rate);
        _inMoving = false;
    }
}