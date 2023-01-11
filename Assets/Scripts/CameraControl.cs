using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float speedMod = 100;
    [SerializeField] private float zoom = 10;
    [SerializeField] private float zoomMin = 5;
    [SerializeField] private float zoomMax = 20;
    [SerializeField] private float zoomSens = 0.1f;
    private Camera _camera;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    public float fov;

    private void FixedUpdate()
    {
        var playerPos = player.position;
        var mouseOffset = FovMouseOffset();
        CameraMove(playerPos + mouseOffset);
        UpdateZoom();
    }

    private void UpdateZoom()
    {
        // if (Input.GetAxis("Mouse ScrollWheel") > 0) Debug.Log(Input.GetAxis("Mouse ScrollWheel"));
        zoom = Mathf.Clamp(zoom + Input.GetAxis("Mouse ScrollWheel") * zoomSens, zoomMin, zoomMax);
        _camera.orthographicSize = zoom;
    }
    
    private Vector3 FovMouseOffset()
    {
        var screenCoords = Camera.main.ScreenToViewportPoint(Input.mousePosition) - Vector3.one / 2f;
        var mouseOffset = new Vector3(Screen.width * screenCoords.x, Screen.height * screenCoords.y) * (fov / 100);
        return mouseOffset;
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