using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RigidbodyEntity : Entity
{
    public Rigidbody2D MyRigidbody;
    
    protected void Awake() => MyRigidbody = GetComponent<Rigidbody2D>();
    
}