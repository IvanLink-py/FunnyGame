using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class GunInfo : ScriptableObject
{
    public GameObject bulletPrefab;
    
    [Header("Bullet data")]
    public float speed;
    public float size;
    public float ttl;
    public float ttlRnd;
    
    public float damage;

    [Header("Weapon data")]
    public bool isFullAmmo;
    public bool isSingleAmmoLoad;
    public bool isPostLoad;
    
    public int count;
    public float recoil;
    public float rate;
    public int ammoInMag;
    public float reloadTime;
    public float scatter;
}
