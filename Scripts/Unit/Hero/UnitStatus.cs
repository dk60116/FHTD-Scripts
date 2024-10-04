using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public struct UnitStatus
{
    [ReadOnlyInspector]
    public UnitRank eRank;
    [ReadOnlyInspector]
    public float fOrgMoveSpeed, fCrtMoveSpeed;
    [ReadOnlyInspector]
    public int iMaxHp, iCrtHp;
    [ReadOnlyInspector]
    public int iMaxMp, iCrtMP;
    [ReadOnlyInspector]
    public int iShield;
    [ReadOnlyInspector]
    public int iOrgStartMp, iCrtStartMp;
    [ReadOnlyInspector]
    public int increaseMp_Hit;
    [ReadOnlyInspector]
    public int iOrgAttack, iCrtAttack;
    [ReadOnlyInspector]
    public int iAddAttack;
    [ReadOnlyInspector]
    public int iOrgAbilPower, iCrtAbilPower;
    [ReadOnlyInspector]
    public int iAddPower;
    [ReadOnlyInspector]
    public float fOrgAtkSpeed, fCrtAtkSpeed;
    [ReadOnlyInspector]
    public float fOrgCriRate, fCrtCriRate;
    [ReadOnlyInspector]
    public float fOrgCriDamage, fCrtCriDamage;
    [ReadOnlyInspector]
    public float fOrgEvasionRate, fCrtEvasionRate;
    [ReadOnlyInspector]
    public float fOrgAtkRange, fCrtAtkRange;
    [ReadOnlyInspector]
    public int iOrgADDef, iOrgAPDef;
    [ReadOnlyInspector]
    public int iCrtADDef, iCrtAPDef;
    [ReadOnlyInspector]
    public int iAddADDef, iAddAPDef;
    [ReadOnlyInspector]
    public float fOrgDmgIncrease, fCrtDmgIncrease;
    [ReadOnlyInspector]
    public float fOrgDmgReduction, fCrtDmgReduction;
    [ReadOnlyInspector]
    public float fHealingAmount;
    [ReadOnlyInspector]
    public bool bIsProjectile;
    [ReadOnlyInspector]
    public float fAtkDelay;
    [ReadOnlyInspector]
    public float fProjSpeed;
    [ReadOnlyInspector]
    public bool bDirectAttack;
    [ReadOnlyInspector]
    public bool isAir;
}
