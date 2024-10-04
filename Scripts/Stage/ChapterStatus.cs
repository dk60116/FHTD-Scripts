using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct StageUnits
{
    public List<int> listLevel;
    public List<int> listId;
}

[Serializable]
public struct ChapterStatus
{
    [ReadOnlyInspector]
    public int iStageID;
    [ReadOnlyInspector]
    public string strChapterName;
    [ReadOnlyInspector]
    public int iPlayerMaxHp;
    [ReadOnlyInspector]
    public string[] strRewards;
    [ReadOnlyInspector]
    public int iMaxStage, iCrtStage;
    [ReadOnlyInspector]
    public Sprite imgStageEmblem;
    public List<StageUnits> listStageUnits;
}
