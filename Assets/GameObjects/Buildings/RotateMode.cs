using System;
using UnityEngine;

[Serializable]
public struct RotateMode
{
    public bool canHorizontalFlip;
    public bool canVerticalFlip;
    
    public bool can90deg;
    public bool can180deg;
    public bool can270deg;

    public bool isRandom;
}