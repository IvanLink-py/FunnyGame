using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class BuildingPreview : MonoBehaviour
{
    private readonly List<GameObject> _inTouch = new ();
    private SpriteRenderer _spriteRenderer;
    
    public bool CanPlace => isActive && !_inTouch.Any();
    public bool isActive;

    public Color allowColor = new (0,72,186, 0.8f);
    public Color disallowColor = new (255,8,0, 0.8f);

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (isActive)
        {
            _spriteRenderer.color = CanPlace ? allowColor : disallowColor;
        }
        else
        {
            _spriteRenderer.color = new Color(1, 1, 1, 0);
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
