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

    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _mainCamera = Camera.main;
    }

    void FixedUpdate()
    {
        Movement();
        Aim();
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
}
