using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticEvents 
{
    public static Action SpawnNewEntity;
    

    public static void CallSpawnNewEntity()
    {
        SpawnNewEntity?.Invoke();
    }
}
