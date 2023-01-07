using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    private Rigidbody2D _rigidbody2D;
    private float ttl = 2f;
    public GunInfo myInfo;
    public Vector2 velocity;
    public Entity shooter;

    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _rigidbody2D.velocity = velocity;
        ttl = myInfo.ttl - myInfo.ttl * Random.value * myInfo.ttlRnd;
    }

    private void Update()
    {
        ttl -= Time.deltaTime;
        if (ttl <= 0 ||
            _rigidbody2D.velocity.magnitude < myInfo.speed / 2)
            Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        var go = col.gameObject;
        if (go.CompareTag("Terrain")) Destroy(gameObject);
        
        if (!go.CompareTag("Entity")) return;
        go.GetComponent<Entity>().OnBulletHit(this);
        Destroy(gameObject);
    }
}