using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SlotsPresentation : MonoBehaviour
{
    [SerializeField] private bool autoGenerateSlots;
    [SerializeField] private Inventory inventory;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private SlotType slotType;
    [SerializeField] private List<SlotPresentation> slots;

    private RectTransform _cursorSlotTransform;

    private void Start()
    {
        if (!autoGenerateSlots) return;
        foreach (var slot in inventory.slots.Where(s => s.myType == slotType))
        {
            var slotP = Instantiate(slotPrefab, transform).GetComponent<SlotPresentation>();
            slotP.mySlot = slot;
            slots.Add(slotP);
        }
    }
    
    public void UpdateSlots()
    {
        foreach (var slot in slots) slot.UpdateItems();
    }
}