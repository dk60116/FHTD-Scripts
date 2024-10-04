using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct PlayerItemBundle
{
    [ReadOnlyInspector]
    public string ItemID;
    [ReadOnlyInspector]
    public int count;
}
