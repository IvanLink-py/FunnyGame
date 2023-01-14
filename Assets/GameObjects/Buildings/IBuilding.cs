using UnityEngine;

public interface IBuilding : IDestructible
{
    Sprite Sprite { get; }
    RotateMode RotateMode { get; }
}