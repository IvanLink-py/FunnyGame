using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UiBar : MonoBehaviour
{
    [Header("Sprites")] 
    [SerializeField] private Sprite emptyEl;
    [SerializeField] private List<Sprite> stagesEl;

    [Header("Settings")] 
    [SerializeField] private GameObject elementPrefab;

    public float CurrentValue
    {
        get => currentValue;
        set { currentValue = Mathf.Max(value,0); UpdateValue();}
    }

    public float elementCost;
    public float maxValue;
    public bool showOverflow;

    private List<Image> _elements = new ();
    [SerializeField] private float currentValue;

    private void Start()
    {
        UpdateChild();
    }

    public void UpdateValue()
    {
        var rest = CurrentValue;
        
        foreach (var image in _elements)
        {
            if (rest >= elementCost)
            {
                image.sprite = stagesEl[^1];
                rest -= elementCost;
            }
            else
            {
                var stageCost = elementCost / stagesEl.Count;
                image.sprite = stagesEl[(int)(rest / stageCost)];
                rest = 0;
            }
            
            // if (rest <= 0) break;
            
        }
    }

    private void UpdateChild()
    {
        for (var i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(0).gameObject);
        }
        
        for (var i = 0; i < Mathf.Ceil(maxValue / elementCost); i++)
        {
            var image = Instantiate(elementPrefab, transform).GetComponent<Image>();
            _elements.Add(image);
        }
        _elements.Reverse();
        
        UpdateValue();
    }

    // private void FixedUpdate()
    // {
    //     CurrentValue = (Mathf.Sin(Time.frameCount/1000f)+1) * 50;
    //     Debug.Log(CurrentValue);
    // }
}
