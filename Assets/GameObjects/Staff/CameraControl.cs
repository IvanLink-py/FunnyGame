using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float speedMod = 100;

    private void FixedUpdate()
    {
        var playerPos = player.position;
        var distance = (transform.position - playerPos).magnitude;
        var speed = 1 / -(speedMod * distance) + 1;                 // Расчёт скорости перемешения камеры
                                                                        // на основании дистанции до игрока
                                                                        // Дальше -> Быстрее

        transform.position = Vector3.Lerp(transform.position, playerPos, speed) - Vector3.forward;
    }
}