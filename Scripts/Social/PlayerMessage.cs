using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct PlayerMessage
{
    [ReadOnlyInspector]
    public string code, title, sender, detail;
    [ReadOnlyInspector]
    public DateTime date;
    [ReadOnlyInspector]
    public int diamond;
    public List<PlayerItemBundle> presentList;
    [ReadOnlyInspector]
    public bool readed, received;
}
