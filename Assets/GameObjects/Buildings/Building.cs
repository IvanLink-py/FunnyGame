using System;
using System.Collections;
using System.Collections.Generic;
using GameObjects.Player;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class Building : Destructible, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public DropTable breakDrop;
    public ItemInfo upDrop;

    public float upTime;

    private bool _isUpInProgress;

    private void Update()
    {
        
    }

    public static Vector3 GetNewBlockPosition(Vector3 oldPos, PlaceMode placeMode)
    {
        if (!placeMode.gridSnap)
        {
            oldPos.z = 0;
            return oldPos;
        }

        oldPos.x = Mathf.Floor(oldPos.x + 0.5f);
        oldPos.y = Mathf.Floor(oldPos.y + 0.5f);
        oldPos.z = 0;

        return oldPos;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CancelUp();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right) StartUp();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        CancelUp();
    }

    private void StartUp()
    {
        if (_isUpInProgress) return;
        _isUpInProgress = true;
        StartCoroutine(UpRoutine());
    }

    private void CancelUp()
    {
        if (!_isUpInProgress) return;
        StopCoroutine(nameof(UpRoutine));
        _isUpInProgress = false;
    }

    private IEnumerator UpRoutine()
    {
        yield return new WaitForSeconds(upTime);
        if (_isUpInProgress) Up();
    }

    private void Up()
    {
        Destroy(gameObject);
        PlayerControl.Main.myInventory.TryPut(new Items { item = upDrop, count = 1, metaInfo = new MetaItemInfo() });
    }
}