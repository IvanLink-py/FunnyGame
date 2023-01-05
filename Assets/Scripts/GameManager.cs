using System;
using UnityEditor.Search;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    [SerializeField] private GameObject bulletPrefab;

    private void Awake() => _instance = this;

    public static Bullet Shoot(Vector2 pos, Vector2 dir)
    {
        var b = Instantiate(_instance.bulletPrefab).transform;
        var bullet = b.GetComponent<Bullet>();
        
        b.position = pos;
        bullet.velocity = dir * 50f;
        return bullet;
    }
    
}