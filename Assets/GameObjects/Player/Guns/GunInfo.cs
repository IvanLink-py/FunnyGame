using UnityEngine;

[CreateAssetMenu]
public class GunInfo : ScriptableObject
{
    public float speed;
    public float size;
    public float ttl;
    public float ttlRnd;
    
    public float damage;

    public bool isFullAmmo;
    public int count;
    public float recoil;
    public float rate;
    public int ammoInMag;
    public float reloadTime;
    public float scatter;
}
