using System;
using System.Collections.Generic;
using System.Linq;
using GameObjects.Player;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    private static UiManager _ui;
    private static readonly List<FloatingDamageInfo> _lastInfo = new();
    public static UiState CurrentState = UiState.Game;
    [Header("SubSystems")] public UiHeathManager heathManager;
    public static event UnityAction<UiStateChangedEventArgs> UiStateChanged;

    [Header("Aim")] [SerializeField] private Image aim;

    public static bool IsAimVisible
    {
        get => _ui.isAimVisible;
        set
        {
            if (value && CurrentState == UiState.Game)
            {
                _ui.isAimVisible = true;
                _ui.aim.gameObject.SetActive(true);
                return;
            }
            
            _ui.isAimVisible = false;
            _ui.aim.gameObject.SetActive(false);
        }
    }

    [SerializeField] private bool isAimVisible;

    [Space] [Header("Inventory")] [SerializeField]
    private RectTransform inventory;

    public static bool InUi => CurrentState != UiState.Game && CurrentState != UiState.None;

    [SerializeField] private SlotsPresentation invPresenter;
    [SerializeField] private KeyCode inventoryOpenKey;
    public InventorySlot cursorSlot;
    public RectTransform cursorSlotPresentation;
    [Space] [Header("FloatingDamageInfo")] public GameObject damageDrawPrefab;
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
        SetInventoryState();
    }

    private void Update()
    {
        InventoryUpdate();
    }

    private void FixedUpdate()
    {
        UpdateAimPos();
        UpdateCursorSlotPos();
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

    private void InventoryUpdate()
    {
        if (Input.GetKeyDown(inventoryOpenKey)) ChangeInventoryState();
        if (InUi && Input.GetMouseButtonDown(0)) CheckCursorDrop();
    }

    private void CheckCursorDrop()
    {
        if ((CurrentState == UiState.Inventory || CurrentState == UiState.Dialog) && !IsPointerOverUIElement())
            cursorSlot.Drop(PlayerControl.Main.transform.position + PlayerControl.Main.Forward);
    }

    private void ChangeInventoryState()
    {
        switch (CurrentState)
        {
            case UiState.Game:
                CurrentState = UiState.Inventory;
                UiStateChanged?.Invoke(new UiStateChangedEventArgs
                {
                    NewState = CurrentState, PrevState = UiState.Game
                });
                SetInventoryState();
                return;
            case UiState.Inventory:
                CurrentState = UiState.Game;
                UiStateChanged?.Invoke(new UiStateChangedEventArgs
                {
                    NewState = CurrentState, PrevState = UiState.Inventory
                });
                SetInventoryState();
                return;
        }
    }

    private void SetInventoryState()
    {
        inventory.gameObject.SetActive(InUi);
        invPresenter.UpdateSlots();
        Cursor.visible = InUi;
        IsAimVisible = !InUi;
        if (!InUi) cursorSlot.Drop(PlayerControl.Main.transform.position + PlayerControl.Main.Forward * 0.5f);
    }

    public static bool CanShoot()
    {
        return !(InUi || IsPointerOverUIElement());
    }

    private void UpdateAimPos()
    {
        if (!IsAimVisible) return;
        aim.rectTransform.position = Input.mousePosition;
    }

    private void UpdateCursorSlotPos()
    {
        if (CurrentState != UiState.Inventory && CurrentState != UiState.Dialog) return;
        cursorSlotPresentation.position = Input.mousePosition;
    }

    public static void OnSlotClick(InventorySlot slot, PointerEventData.InputButton button)
    {
        if (InUi)
        {
            if (_ui.cursorSlot.Items is null)
            {
                slot.TransferTo(_ui.cursorSlot,
                    button == PointerEventData.InputButton.Left ? TakeMode.Full : TakeMode.Half);
            }
            else
            {
                if (slot.Items is null)
                {
                    _ui.cursorSlot.TransferTo(slot,
                        button == PointerEventData.InputButton.Left ? TakeMode.Full : TakeMode.One);
                }
                else
                {
                    var a = _ui.cursorSlot.TransferTo(slot,
                        button == PointerEventData.InputButton.Left ? TakeMode.Full : TakeMode.One);

                    if (a != 0) return;
                    _ui.cursorSlot.Swap(slot);
                }
            }
        }
        else
        {
            if (slot.myType == SlotType.Hotbar) slot.inventory.SetSelectedSlot(slot);
        }
    }

    public static bool IsPointerOverUIElement()
    {
        return IsPointerOverUIElement(GetEventSystemRaycastResults());
    }

    private static bool IsPointerOverUIElement(IEnumerable<RaycastResult> eventSystemRaycastResults)
    {
        return eventSystemRaycastResults.Any(curRaycastResult =>
            curRaycastResult.gameObject.layer == LayerMask.NameToLayer("UI"));
    }

    private static IEnumerable<RaycastResult> GetEventSystemRaycastResults()
    {
        var eventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };
        var raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);
        return raycastResults;
    }
}

public enum UiState
{
    None,
    Game,
    Inventory,
    Dialog,
    Pause,
    Menu
}