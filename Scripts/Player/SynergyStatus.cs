using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SynergyValue
{
    [SerializeField, ReadOnlyInspector]
    public float[] value;
}

[Serializable]
public struct SynergyStatus
{
    [ReadOnlyInspector]
    public int iNumber;
    [ReadOnlyInspector]
    public string strSynergy;
    [ReadOnlyInspector]
    public string strSynergyName;
    [SerializeField, ReadOnlyInspector]
    public List<int> listCount;
    [ReadOnlyInspector]
    public string strSynergyDescription;
    public List<SynergyValue> listValue;
}
