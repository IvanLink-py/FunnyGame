using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyConrol : RigidbodyEntity
{
    [SerializeField] private Animation myAnim;
    [SerializeField] private AIState myState;
    [SerializeField] [CanBeNull] private Entity target;
    [SerializeField] private float speed = 120;
    [SerializeField] private float attackTimer;
    [SerializeField] private float attackDelay;
    [SerializeField] private float attackRadius;
    public float damage;


    private void FixedUpdate()
    {
        AIControl();
    }

    private void AIControl()
    {
        switch (myState)
        {
            case AIState.Idle:
                target = GameManager.Player;
                myState = AIState.Going;
                break;

            case AIState.Going:
                if (target is not null)
                {
                    LookAt(target.transform.position);
                    MyRigidbody.AddForce(Forward * speed);
                    if (TargetInRadius())
                    {
                        attackTimer = attackDelay;
                        myState = AIState.PrepareToAttack;
                        myAnim.Play("Attack");
                    }
                }

                break;

            case AIState.PrepareToAttack:
                attackTimer -= Time.deltaTime;
                if (attackTimer < 0)
                {
                    myState = AIState.Attack;
                    MyRigidbody.AddForce(Forward * (speed * 5));
                }

                break;

            case AIState.Attack:
                if (TargetInRadius())
                {
                    target.OnMeleeHit(this);
                }

                myState = AIState.Going;

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private bool TargetInRadius()
    {
        return target is not null && (transform.position - target.transform.position).magnitude < attackRadius;
    }

    protected override void Die()
    {
        // GameManager.ItemDrop(new Items { item = GameManager.MainItemDB[0], count = 4 }, transform.position);
        
        GameManager.ItemDrop(Random.value > 0.5f
            ? new Items { item = GameManager.MainItemDB[0], count = Random.Range(1, 100) }
            : new Items { item = GameManager.MainItemDB[1], count = Random.Range(1, 100) }, transform.position);
        
        base.Die();
    }
}

public enum AIState
{
    Idle,
    Going,
    PrepareToAttack,

    Attack
    // TODO: Временная отключка при полученни урона
}