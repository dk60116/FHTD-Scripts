using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct CampStatus
{
    [ReadOnlyInspector]
    public int iCampLevel;
    [ReadOnlyInspector]
    public int iCampNeedExp, iCampCrtExp, iExpClickCount, iExpCost;
    [ReadOnlyInspector]
    public int iCampMaxHp, iCampCrtHp;
    [ReadOnlyInspector]
    public int iGold;
    [ReadOnlyInspector]
    public int iMaxUnitBatch;
    [ReadOnlyInspector]
    public int iExpIncreasePoint;
}
