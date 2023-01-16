using UnityEngine;

public interface IBuilding : IDestructible
{
    Sprite Sprite { get; }
    RotateMode RotateMode { get; }


    void Place();
    
    bool CanRepair();
    void Repair(float power);
}