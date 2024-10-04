using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct CardFrame
{
    //[ReadOnlyInspector]
    public string itemId, itemName;
    //[ReadOnlyInspector]
    public Sprite frontSprite, backSprite;
    //[ReadOnlyInspector]
    public Color nameColor;
}
