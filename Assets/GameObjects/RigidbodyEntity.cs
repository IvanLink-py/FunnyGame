using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RigidbodyEntity : Entity
{
    protected Rigidbody2D MyRigidbody;
    
    protected void Start() => MyRigidbody = GetComponent<Rigidbody2D>();
    
    
    public new void OnBulletHit(Bullet bullet)
    {
        base.OnBulletHit(bullet);
        MyRigidbody.AddForce(bullet.velocity * bullet.myInfo.size);
    }
}