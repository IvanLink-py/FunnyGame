using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UiBar : MonoBehaviour
{
    [Header("Sprites")] 
    [SerializeField] private Image image;
    [SerializeField] private List<Sprite> stagesEl;

    [Header("Settings")] 
    [SerializeField] private Slider mySlider;
    [SerializeField] private float currentValue;
    [SerializeField] private float maxValue;
    [SerializeField] private float density;
    [SerializeField] private float smooth;
    private RectTransform _rectTransform;
    
    [Header("Heartbeat")] 
    [SerializeField] private bool heartBeat = false;
    [SerializeField] private float normalBpm = 80;
    [SerializeField] private float maxBpm = 200;
    [SerializeField] private float sizeDelta = .1f;
    private float _heartBeatTime = 0;

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public float MaxValue
    {
        get => maxValue;
        set
        {
            maxValue = Mathf.Max(value, 0);
            UpdateValue();
        }
    }

    private float Full => CurrentValue / MaxValue;


    public float CurrentValue
    {
        get => currentValue;
        set
        {
            currentValue = Mathf.Max(value, 0);
            UpdateValue();
        }
    }

    public void UpdateValue()
    {
        _rectTransform.sizeDelta = Vector2.Lerp(_rectTransform.sizeDelta, new Vector2(maxValue / density, _rectTransform.sizeDelta.y), smooth);
        mySlider.value = Mathf.Lerp(mySlider.value, Full, smooth);
        image.sprite = stagesEl[Mathf.Clamp(Mathf.CeilToInt(stagesEl.Count * Full), 0, stagesEl.Count-1)];

    }


    private void FixedUpdate()
    {
        // // Для дебага
        // CurrentValue = (Mathf.Sin(Time.frameCount / 1000f) + 1) * 50;
        // Debug.Log(CurrentValue);
        // // Можно удалить
        
        if (!heartBeat) return;
        _heartBeatTime += Time.deltaTime;
        
        var size = 1 + (Mathf.Sin(_heartBeatTime * Mathf.PI * Mathf.Lerp(maxBpm,normalBpm, Full) / 60) + 1) / 2 * sizeDelta;
        
        image.rectTransform.localScale = Vector3.one * size;
        

    }
}