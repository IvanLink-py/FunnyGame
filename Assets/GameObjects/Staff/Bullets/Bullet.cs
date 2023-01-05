using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    private Rigidbody2D _rigidbody2D;
    [SerializeField] private float ttl = 2f;
    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _rigidbody2D.velocity = velocity;
    }

    public Vector2 velocity;

    private void Update()
    {
        ttl -= Time.deltaTime;
        if (ttl <= 0 ) Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Terrain")) Destroy(gameObject);
    }
}