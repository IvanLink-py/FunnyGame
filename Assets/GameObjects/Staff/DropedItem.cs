using System;
using System.Collections;
using System.Collections.Generic;
using GameObjects.Player;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(CircleCollider2D))]
public class DropedItem : MonoBehaviour
{
    public Items items;
    // public float pullForce;

    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = items.item.image;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            var rest = PlayerControl.Main.myInventory.TryPut(items);
            if (rest == 0) Destroy(gameObject);
            else items.count = rest;
        }
        else if (collision.gameObject.CompareTag("Item"))
        {
            TryStack(collision.gameObject.GetComponent<DropedItem>());
        }
    }

    private void TryStack(DropedItem other)
    {
        if (items.item != other.items.item || items.count + other.items.count > items.item.stackSize || items.count == 0) return;
        items.count += other.items.count;
        other.items.count = 0; 
        Destroy(other.gameObject);
    }
    
}