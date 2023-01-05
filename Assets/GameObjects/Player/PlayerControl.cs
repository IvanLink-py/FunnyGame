using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerControl : MonoBehaviour
{
    [SerializeField] private float speed = 5;
    private Rigidbody2D _rigidbody2D;
    private Camera _mainCamera;

    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _mainCamera = Camera.main;
    }

    void Update()
    {
        Movement();
        Aim();
    }

    private void Aim()
    {
        var diff = _mainCamera.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        diff.Normalize();

        var rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rot_z);
    }

    private void Movement()
    {
        var control = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        _rigidbody2D.velocity = control * speed;
    }
}
