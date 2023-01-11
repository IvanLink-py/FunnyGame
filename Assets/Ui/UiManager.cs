using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;
using Image = UnityEngine.UI.Image;

public class UiManager : MonoBehaviour
{
    private static UiManager _ui;
    [Header("SubSystems")] public UiHeathManager heathManager;

    [Header("Aim")] [SerializeField] private Image aim;

    [Space] [Header("Inventory")] [SerializeField]
    private RectTransform inventory;

    [SerializeField] private SlotsPresentation invPresenter;
    [SerializeField] private KeyCode inventoryOpenKey;
    public InventorySlot cursorSlot;
    public RectTransform cursorSlotPresentation;
    [Space] [Header("FloatingDamageInfo")] public GameObject damageDrawPrefab;
    private static List<FloatingDamageInfo> _lastInfo = new();
    [SerializeField] private float snapRadius = 0.2f; // Радус поиска ближайшей надписи при появлении новой 


    private void Awake()
    {
        _ui = this;
    }

    private void Start()
    {
        GameManager.OnHitRegister += DrawDamage;
        cursorSlot = GameManager.Player.myInventory.cursorSlot;
        cursorSlotPresentation.GetComponent<SlotPresentation>().mySlot = cursorSlot;
    }

    private void DrawDamage(Damage damageinfo)
    {
        var closestData = GetClosestInfo(damageinfo.Target.transform.position);
        if (closestData.Item1 is not null && closestData.Item2 < snapRadius)
            closestData.Item1.Amount += damageinfo.HpAmount;
        else DamageInfoAppear(damageinfo.Target.transform.position, damageinfo.HpAmount);
    }

    private static void DamageInfoAppear(Vector3 pos, float amount)
    {
        var floatingInfo = Instantiate(_ui.damageDrawPrefab);
        floatingInfo.transform.position = pos;
        _lastInfo.Add(floatingInfo.GetComponent<FloatingDamageInfo>());
        _lastInfo[^1].Amount = amount;
    }

    private (FloatingDamageInfo?, float?) GetClosestInfo(Vector3 pos)
    {
        if (_lastInfo.Count == 0) return (null, null);

        var closest = _lastInfo[0];
        var closestDistance = (_lastInfo[0].transform.position - pos).magnitude;

        foreach (var info in _lastInfo)
        {
            if (!((info.transform.position - pos).magnitude < closestDistance)) continue;
            closest = info;
            closestDistance = (info.transform.position - pos).magnitude;
        }

        return (closest, closestDistance);
    }

    public static void DamageInfoDisappear(FloatingDamageInfo info)
    {
        if (_lastInfo.Contains(info)) _lastInfo.Remove(info);
        Destroy(info.gameObject);
    }

    private void FixedUpdate()
    {
        UpdateAimPos();
        UpdateCursorSlotPos();
    }

    private void Update()
    {
        InventoryUpdate();
    }

    private void InventoryUpdate()
    {
        if (Input.GetKeyDown(inventoryOpenKey)) SetInventoryState(!inventory.gameObject.activeInHierarchy);
    }

    private void SetInventoryState(bool state)
    {
        inventory.gameObject.SetActive(state);
        invPresenter.UpdateSlots();
        Cursor.visible = state;
        aim.gameObject.SetActive(!state);
    }

    public static bool CanShoot() => !_ui.inventory.gameObject.activeInHierarchy;

    private void UpdateAimPos()
    {
        aim.rectTransform.position = Input.mousePosition;
    }

    private void UpdateCursorSlotPos()
    {
        if (!inventory.gameObject.activeInHierarchy) return;
        cursorSlotPresentation.position = Input.mousePosition;
    }

    public static void OnSlotClick(InventorySlot slot, PointerEventData.InputButton button)
    {
        if (_ui.cursorSlot.Items is null)
        {
            slot.TransferTo(_ui.cursorSlot, button == PointerEventData.InputButton.Left ? TakeMode.Full : TakeMode.Half);
        }
        else
        {
            if (slot.Items is null)
            {
                _ui.cursorSlot.TransferTo(slot, button == PointerEventData.InputButton.Left ? TakeMode.Full : TakeMode.One);
            }
            else
            {
                var a = _ui.cursorSlot.TransferTo(slot, button == PointerEventData.InputButton.Left ? TakeMode.Full : TakeMode.One);;

                if (a != 0) return;
                var temp = new Items { item = slot.Items.item, count = slot.Items.count };
                slot.Items = _ui.cursorSlot.Items;
                _ui.cursorSlot.Items = temp;
            }
        }
    }
}