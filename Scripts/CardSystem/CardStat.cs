using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct CardStatus
{
    [ReadOnlyInspector]
    public int iCardID;
    [ReadOnlyInspector]
    public string strCardName;
    [ReadOnlyInspector]
    public CardType eCardType;
    [ReadOnlyInspector]
    public CardClass eCardClass;
    [ReadOnlyInspector]
    public CardCostType eCostType;
    [ReadOnlyInspector]
    public int iCost, iLevel;
    [ReadOnlyInspector]
    public CardUseTimiing eCastTiming;
    [ReadOnlyInspector]
    public CardUseTarget eCastTarget;
    [ReadOnlyInspector]
    public CardTear eCardTear;
    public List<float> listValue;
    [ReadOnlyInspector]
    public string strCardDescription;
    [ReadOnlyInspector]
    public Sprite imgCardIcon, imgEqupIcon;
    [ReadOnlyInspector]
    public SkillScopeType eScopeType;
    [ReadOnlyInspector]
    public GameObject goEffect;

    [Header("Equipmet")]
    [ReadOnlyInspector]
    public int iAddAD, iAddAP;
    [ReadOnlyInspector]
    public float fRatioAtk, fRatioAP;
    [ReadOnlyInspector]
    public int iAddHP, iAddMP, iAddDef, iAddAPDef;
    [ReadOnlyInspector]
    public float fRatioHP, fRatioMP, iRatioDef, iRatioAPDef;
    [ReadOnlyInspector]
    public float fAddAtkSpd, fAddMoveSpd, fRatioAtkSpd, fRatioMoveSpd;
    [ReadOnlyInspector]
    public int iSubMaxMP;
}
