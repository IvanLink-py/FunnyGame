using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    [FormerlySerializedAs("_rigidbody2D")] public Rigidbody2D rig2D;
    public float ttl = 2f;
    public GunInfo myInfo;
    public Vector2 velocity;
    public Entity shooter;

    protected virtual void Start()
    {
        rig2D = GetComponent<Rigidbody2D>();
        rig2D.velocity = velocity;
        ttl = myInfo.ttl - myInfo.ttl * Random.value * myInfo.ttlRnd;
        
        var targetRotation = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, targetRotation);
        
    }

    protected virtual void Update()
    {
        ttl -= Time.deltaTime;
        if (ttl <= 0 ||
            rig2D.velocity.magnitude < myInfo.speed / 2)
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