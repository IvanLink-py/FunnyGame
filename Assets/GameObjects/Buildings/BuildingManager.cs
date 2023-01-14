using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    private BuildingManager _instance;

    private void Awake()
    {
        _instance = this;
    }
    
}
