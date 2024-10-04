using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SkillValue
{
    [ReadOnlyInspector]
    public float[] value;
}

[Serializable]
public struct HeroSkillComponent
{
    [ReadOnlyInspector]
    public int iSkillID;
    [ReadOnlyInspector]
    public string strHeroName;
    [ReadOnlyInspector]
    public string strSkillName;
    [ReadOnlyInspector]
    public Sprite imgSkillIcon;
    [ReadOnlyInspector]
    public SkillTiming eTiming;
    public List<SkillValue> listValue;
    [ReadOnlyInspector]
    public List<float> listFinalValue;
    [TextArea]
    public string strExplanation;
    [ReadOnlyInspector]
    public List<Transform> listSkillEffect;
}
