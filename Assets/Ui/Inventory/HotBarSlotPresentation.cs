using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HotBarSlotPresentation : SlotPresentation
{
    [SerializeField] private RectTransform border;

    protected override void ShowItems(Items items, bool isSelected)
    {
        if (border is not null) border.localScale = Vector3.one * (isSelected ? 1.1f : 1);
        base.ShowItems(items, isSelected);
    }
}