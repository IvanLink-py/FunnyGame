using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float speedMod = 100;
    public float fov;

    private void FixedUpdate()
    {
        var playerPos = player.position;
        var screenCoords = Camera.main.ScreenToViewportPoint(Input.mousePosition) - Vector3.one / 2f;
        var mouseOffset = new Vector3(Screen.width * screenCoords.x, Screen.height * screenCoords.y) * (fov / 100);
        CameraMove(playerPos + mouseOffset);
    }

    private void CameraMove(Vector3 playerPos)
    {
        var distance = (transform.position - playerPos).magnitude;

        var speed = 1 / -(speedMod * distance) + 1;
        // Расчёт скорости перемешения камеры
        // на основании дистанции до игрока
        // Дальше -> Быстрее

        transform.position = Vector3.Lerp(transform.position, playerPos, speed) - Vector3.forward;
    }
}