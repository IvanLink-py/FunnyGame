using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingPreview : MonoBehaviour
{
    private readonly List<GameObject> _inTouch = new();

    private (SpriteRenderer, Collider2D) _instance;
    private SpriteRenderer _spriteRenderer;

    public bool CanPlace => isActive && !_inTouch.Any();
    public bool HavePreview => transform.childCount >= 1;
    public bool isActive;

    public Color allowColor = new(0, 72, 186, 0.8f);
    public Color disallowColor = new(255, 8, 0, 0.8f);

    private void Awake()
    {
        // InitPreview();
        
    }

    public void InstantiatePreview(GameObject prefab)
    {
        if (HavePreview) ClearPreview();
        Instantiate(prefab, transform.position, Quaternion.identity, transform);
        InitPreview();
    }

    private void InitPreview()
    {
        _instance = new ValueTuple<SpriteRenderer, Collider2D>(
            GetComponentInChildren<SpriteRenderer>(),
            GetComponentInChildren<Collider2D>());

        _instance.Item2.isTrigger = true;
        _instance.Item1.transform.localScale *= 0.95f;
    }

    public void Materialize()
    {
        _instance.Item1.transform.SetParent(null);
        DisposePreview();
    }
    
    private void ClearPreview()
    {
        DestroyImmediate(_instance.Item1.gameObject);
    }
    
    
    private void DisposePreview()
    {
        _instance.Item2.isTrigger = false;
        _instance.Item1.color = new Color(1, 1, 1, 1);
        _instance.Item1.transform.localScale /= 0.95f;
    }


    private void Update()
    {
        UpdatePreviewColor();
    }

    private void UpdatePreviewColor()
    {
        if (!HavePreview) return;
        if (isActive)
        {
            _instance.Item1.color = CanPlace ? allowColor : disallowColor;
        }
        else
        {
            _instance.Item1.color = new Color(1, 1, 1, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (_inTouch.Contains(col.gameObject)) return;
        _inTouch.Add(col.gameObject);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (_inTouch.Contains(other.gameObject)) _inTouch.Remove(other.gameObject);
    }
}