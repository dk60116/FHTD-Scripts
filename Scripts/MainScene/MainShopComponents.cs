using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct MainShopComponents
{
    [ReadOnlyInspector]
    public List<string> requestFunctionName;
    [ReadOnlyInspector]
    public bool isDual;
    [ReadOnlyInspector]
    public List<bool> canSoldOut;
    [ReadOnlyInspector]
    public int category;
    [ReadOnlyInspector]
    public List<List<string>> itemList;
    [ReadOnlyInspector]
    public List<string> contentId, contentName, imageLink, itemDescription;
    [ReadOnlyInspector]
    public List<int> cost;
    [ReadOnlyInspector]
    public List<int> tag;
    [ReadOnlyInspector]
    public List<bool> stackable;
    [ReadOnlyInspector]
    public List<int> stackCount;
}
