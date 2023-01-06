using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerControl : MonoBehaviour
{
    [SerializeField] private float speed = 5;
    private Rigidbody2D _rigidbody2D;
    private Camera _mainCamera;
    private float _recallTimer;
    private int _ammoInMag;
    private bool _canShoot;

    public GunInfo currentGun;

    public Vector3 Forward
    {
        get
        {
            var z = transform.rotation.eulerAngles.z * Mathf.Deg2Rad + Mathf.PI / 2;
            return Vector3.up * Mathf.Sin(z) + Vector3.right * Mathf.Cos(z);
        }
    }

    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _mainCamera = Camera.main;
        _ammoInMag = currentGun.ammoInMag;
    }

    void FixedUpdate()
    {
        Movement();
        Aim();
        ShootControl();
        // Debug.DrawLine(transform.position, transform.position + Forward);
    }

    private void Aim()
    {
        var diff = _mainCamera.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        diff.Normalize();

        var targetRotation = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, targetRotation - 90);
    }

    private void Movement()
    {
        var control = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        _rigidbody2D.AddForce(control * speed);
    }

    private void ShootControl()
    {
        if (Input.GetAxis("Fire1") < 0.5f && !currentGun.isFullAmmo) _canShoot = true;
        
        _recallTimer -= Time.deltaTime;
        if (Input.GetAxis("Fire1") < 0.5f) return;
        if (_recallTimer > 0) return;
        if (!_canShoot) return;

        Shoot();
    }

    private void Shoot()
    {
        GameManager.Shoot(transform.position, Forward, currentGun);
        _rigidbody2D.AddForce(-Forward*currentGun.recoil);
        
        _ammoInMag--;
        if (_ammoInMag > 0) 
            _recallTimer = 1 / currentGun.rate;
        else {
            _recallTimer = currentGun.reloadTime;
            _ammoInMag = currentGun.ammoInMag;
        }

        if (!currentGun.isFullAmmo) _canShoot = false;


    }
}