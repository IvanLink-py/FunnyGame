using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosive : Bullet
{
    public float time = 5;
    public float timeRemaining;
    public GameObject expPrefab;

    protected override void Start()
    {
        base.Start();
        timeRemaining = time;
    }

    protected override void Update()
    {
        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0) Boom();
    }

    public void Boom()
    {
        GameManager.Boom(transform.position, expPrefab);
        Destroy(gameObject);
    }
    
    private void OnCollisionEnter2D(Collision2D col)
    {
        var go = col.gameObject;
        if (!go.CompareTag("Entity")) return;
        (go.GetComponent(typeof(Destructible)) as Destructible)?.DamageTake(rig2D.velocity.magnitude/5f, null, DamageType.Env);
    }
    
}
