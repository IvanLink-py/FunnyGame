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

    public GameObject fragmentPrefab;
    public int fragmentCount;
    public float fragmentVel;
    private void Awake()
    {
        GetComponent<CircleCollider2D>().radius = radius;
    }
    protected  void Start()
    {
        Destroy(gameObject, 0.17f);
    }
    
    public void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Entity") || other.gameObject.CompareTag("Player"))
            other.gameObject.GetComponent<Entity>().OnExplosionHit(this);
    }
}