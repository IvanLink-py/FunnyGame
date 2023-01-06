using System;
using GameObjects.Player;
using UnityEditor.Search;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static PlayerControl Player;

    [SerializeField] private GameObject bulletPrefab;

    private void Awake() => _instance = this;

    public static void Shoot(Vector2 pos, Vector2 dir, GunInfo gunInfo)
    {
        for (var i = 0; i < gunInfo.count; i++)
        {
            var b = Instantiate(_instance.bulletPrefab).transform;
            var bullet = b.GetComponent<Bullet>();
            bullet.myInfo = gunInfo;


            // Scatter - Разброс 
            var resultScatter = gunInfo.scatter * (Random.value - 0.5f);
            var resultAngle = (Vector2.SignedAngle(Vector2.right, dir) + resultScatter) * Mathf.Deg2Rad;
            var resultDirection = new Vector2(Mathf.Cos(resultAngle), Mathf.Sin(resultAngle));

            b.position = pos + dir;
            bullet.velocity = resultDirection * gunInfo.speed;

            // Size
            b.localScale = Vector3.one * gunInfo.size;
        }
    }
}