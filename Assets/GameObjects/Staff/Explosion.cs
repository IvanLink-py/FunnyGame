using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Collider2D))]
public class Explosion : MonoBehaviour
{
    public float radius = 3;
    public float damage = 100;
    public float liftTime = 1;
    
    public GunInfo explosionGunInfo;

    private readonly List<GameObject> _explosionTouched = new();
    private void Awake()
    {
        GetComponent<CircleCollider2D>().radius = radius;
    }
    protected  void Start()
    {
        GameManager.Shoot(transform.position, Vector2.up, explosionGunInfo, null);
        Destroy(gameObject, liftTime);
    }
    
    public void OnTriggerStay2D(Collider2D other)
    {
        var o = other.gameObject;
        
        if (o.CompareTag("Explosive")) {
            other.gameObject.GetComponent<Explosive>().Boom();
            return;
        }
        
        if ((!o.CompareTag("Entity") && !o.CompareTag("Player")) || _explosionTouched.Contains(o)) return;
        o.GetComponent<Entity>().OnExplosionHit(this);
        _explosionTouched.Add(o);
        Debug.Log("Explosion touch");
    }
}