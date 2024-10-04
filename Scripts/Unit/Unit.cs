using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
using System.Linq;
using System.Collections;
using UnityEngine.AI;
using UnityEngine.XR;
using DG.Tweening;
using System;
using AmazingAssets.AdvancedDissolve;
using System.Reflection;

public enum UnitRank { None = 0, Common, UnCommon, Rare, Epic, Legendary }
public enum DamageType { AD, AP, True, Heal }
public enum UnitAniActionParam { Idle = 0, Run, Attack, DefSkill, AtkSkill, StopSkill, Damage, Die, Attack_Arrangement, Airborne, Fall }

public enum BuffType { Airborne, Stern, Stiff, Silent, Attack, Power, MaxHp, MaxMp, PlusMp, Shield, ADDef, APDef, Unspecified,
    SendDamage, ReciveDamage, AtkSpeed, MoveSpeed, Range, CriticalRatio, CriticalDamage, HealAmount, Taunt, Count }

[Serializable]
public class BuffValue
{
    [ReadOnlyInspector]
    public BuffType eBuffType;
    [ReadOnlyInspector]
    public Unit cCaster;
    [ReadOnlyInspector]
    public string strBuffCode;
    [ReadOnlyInspector]
    public bool bHalfInfinity, bInfiniy;
    [ReadOnlyInspector]
    public float fValue;
    [ReadOnlyInspector]
    public bool bIsPositive;
    [ReadOnlyInspector]
    public float fFullTime, fCrtTime;

    public BuffValue(BuffType _eCCType, Unit _cCaster, float _fValue, float _fTime, bool _bHalfInfinity = false, bool _bInfinity = false, string _strCustomConde = "000000")
    {
        eBuffType = _eCCType;
        cCaster = _cCaster;
        strBuffCode = $"{((int)_eCCType).ToString("00")}.{Time.time}.{cCaster.unitID}.{UnityEngine.Random.Range(100000, 999999)}.{_strCustomConde}";
        fValue = _fValue;
        bIsPositive = _fValue > 0;
        fFullTime = _fTime;
        fCrtTime = 0;
        bHalfInfinity = _bHalfInfinity;
        bInfiniy = _bInfinity;
    }

    public void AddValue(float _fValue)
    {
        fValue += _fValue;
    }


    public void AddTime(float _fTime)
    {
        fFullTime += _fTime;
    }
}

public abstract class Unit : MonoBehaviour
{
    [SerializeField, ReadOnlyInspector]
    protected int iUnitID;
    [SerializeField, ReadOnlyInspector]
    protected string strUnitName;

    [SerializeField, ReadOnlyInspector]
    protected Sprite imgUnitIllust, imgFaceIllust;


    protected NavMeshAgent navAgent;
    protected bool bNavStarted;
    [SerializeField, ReadOnlyInspector]
    protected bool bArriveTarget;

    [SerializeField, ReadOnlyInspector]
    protected Transform tfBodyHolder, tfUnitBody;
    [SerializeField, ReadOnlyInspector]
    protected Collider unitCollider;
    protected MeshRenderer[] renMeshRenders;
    protected SkinnedMeshRenderer[] renSMeshRenders;

    [SerializeField, ReadOnlyInspector]
    protected List<QuickOutline> listModelOutlines;

    [SerializeField, ReadOnlyInspector]
    protected bool bIsOwn, bIsPlaced;

    [SerializeField, ReadOnlyInspector]
    protected Transform tfBlock;
    protected MeshRenderer renBlock;
    protected Transform tfRangeCircle;

    [SerializeField, ReadOnlyInspector]
    protected bool bIsAlive;

    [SerializeField]
    protected UnitStatus sUnitStat;
    [SerializeField, ReadOnlyInspector]
    protected int iUnitLevel;

    [SerializeField]
    protected UnitStatus sOriginUnitStatus;
    [SerializeField]
    protected HeroStatus sOriginHeroStatus;
    protected Vector3 v3OriginSize;

    [SerializeField, ReadOnlyInspector]
    protected UnitHUD cHUDUI;

    [SerializeField]
    protected bool bBattleUnit, bAttackUnit;
    [SerializeField]
    protected bool bCanNotBlock, bCanNotMove, bCanNotKeepSlot, bCanNotSell;

    protected Unit cAtkTargetUnit, cTargetMineUnit, cDamageMineUnit;
    protected Unit cCrtHitUnit;
    protected bool bCastingSkill;

    
    [SerializeField, ReadOnlyInspector]
    protected HeroEffect cAttackEffect, cHitEffect;

    [SerializeField, ReadOnlyInspector]
    protected Tile cCurrentTile;

    [SerializeField, ReadOnlyInspector] 
    protected Animator anmAnimator;
    [SerializeField, ReadOnlyInspector]
    protected PoolingManager cObjPool;

    protected bool bHit;
    protected float fAttackDelay;
    protected int iCrtAtkAddDmg;
    protected const float fDelay = 0.3f;
    protected const float fOne = 1f;

    [SerializeField, ReadOnlyInspector]
    protected float fSendDmage, fReciveDmage;
    [SerializeField, ReadOnlyInspector]
    protected int iPlusMp;

    [SerializeField, ReadOnlyInspector]
    protected HeroSkill cSkill;
    public delegate void AttackChain();
    public AttackChain skillChain_AtkEnd;
    public delegate void CastSkillChain();
    public CastSkillChain defSkillChain, atkSkilChain;
    public delegate void HitChain();
    public CastSkillChain hitSkillChain;

    [SerializeField, ReadOnlyInspector]
    protected int iEveHp;

    [SerializeField]
    protected List<BuffValue> listBuff;
    [SerializeField]
    protected bool[] bIsBuffs_Positive, bIsBuffs_Negative;
    protected Dictionary<string, bool> dicCustomMark;

    [SerializeField, ReadOnlyInspector]
    protected bool bCanNotGetMp;

    protected float fAnmMoveSpd, fAnimAtkSpd;
    protected Vector3 v3CrtTargetPos;
    protected int iCrtIndex;

    private WaitForSeconds _cNewWait_HideEffect = new WaitForSeconds(3f);
    private WaitForSeconds _cNewWait_HideProjfectile = new WaitForSeconds(1f);

    [SerializeField, ReadOnlyInspector]
    protected SpawnPartnerUnit cPartnerSpawn;
    [SerializeField, ReadOnlyInspector]
    protected Unit cFreindUnit;

    [SerializeField, ReadOnlyInspector]
    protected int iHeroCost;

    [SerializeField, ReadOnlyInspector]
    private bool bLeveling;

    protected virtual void Awake()
    {
        tfBodyHolder = transform.GetChild(0);
        tfUnitBody = tfBodyHolder.GetChild(0);
        unitCollider = GetComponent<Collider>();

        listModelOutlines = new List<QuickOutline>();

        tfUnitBody.TryGetComponent(out anmAnimator);

        renMeshRenders = tfBodyHolder.GetComponentsInChildren<MeshRenderer>();
        foreach (var item in renMeshRenders)
        {
            listModelOutlines.Add(item.gameObject.AddComponent<QuickOutline>());
            item.gameObject.AddComponent<MeshCollider>();
            item.gameObject.layer = LayerMask.NameToLayer("UnitBody");
        }

        renSMeshRenders = tfBodyHolder.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var item in renSMeshRenders)
        {
            listModelOutlines.Add(item.gameObject.AddComponent<QuickOutline>());
            item.gameObject.AddComponent<MeshCollider>();
            item.gameObject.layer = LayerMask.NameToLayer("UnitBody");
        }

        foreach (var item in listModelOutlines)
        {
            item.OutlineColor = Color.green;
            item.enabled = false;
        }

        cObjPool = GetComponentInChildren<PoolingManager>();

        if (bAttackUnit)
            cHUDUI = Resources.Load<UnitHUD>("GUI/HUD/HUD_AttackHero");
        else
            cHUDUI = Resources.Load<UnitHUD>("GUI/HUD/HUD_DefenseHero");
        navAgent = GetComponent<NavMeshAgent>();

        tfBlock = transform.GetChild(1);
        renBlock = tfBlock.GetComponent<MeshRenderer>();
        tfRangeCircle = transform.GetChild(2);
        iUnitLevel = 1;
        TryGetComponent(out cSkill);
        listBuff = new List<BuffValue>();
        bIsBuffs_Positive = new bool[(int)BuffType.Count];
        bIsBuffs_Negative = new bool[(int)BuffType.Count];
        dicCustomMark = new Dictionary<string, bool>();

        TryGetComponent(out cPartnerSpawn);

        if (navAgent != null)
            navAgent.enabled = bAttackUnit;
    }

    protected virtual void Start()
    {
        InitEffectPool();
        CreateHUD();
        v3OriginSize = tfUnitBody.localScale;

        sOriginUnitStatus = sUnitStat;
    }

    protected virtual void OnEnable()
    {
        bIsAlive = true;
        bArriveTarget = false;
        bNavStarted = false;

        SetupStat();
    }

    void OnDisable()
    {
        if (cHUDUI != null)
            cHUDUI.gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        if (cCurrentTile != null)
            cCurrentTile.SellUnit();
    }

    void FixedUpdate()
    {
        if (cHUDUI != null && bBattleUnit)
            cHUDUI.FollowUnit();
    }

    public virtual void SetTile(Tile _cTile)
    {
        cCurrentTile = _cTile;
    }

    public virtual void OutTile()
    {
        cCurrentTile = null;
    }

    public void SwitchOutLine(bool _bOn)
    {
        if (listModelOutlines[0] != null && listModelOutlines[0].enabled == _bOn)
            return;

        foreach (var item in listModelOutlines)
            item.enabled = _bOn;
        if (cFreindUnit != null)
        {
            if (cFreindUnit is FriendUnit)
                (cFreindUnit as FriendUnit).OnOffLinkLine(_bOn && cFreindUnit.transform.parent != InGameManager.instance.tfTempUnitHolder);
        }    
    }

    public void ChangeOutlineColor(Color _cColor)
    {
        foreach (var item in listModelOutlines)
            item.OutlineColor = _cColor;
    }

    public virtual void OnSpawn()
    {
        if (cPartnerSpawn != null)
        {
            if (cPartnerSpawn.timing == PartnerSpawnTiming.WhenAHero || cPartnerSpawn.timing == PartnerSpawnTiming.WhenPlaceHero)
                cPartnerSpawn.SpawnUnit();
        }

        SetRank(unitStat.eRank);
    }

    public virtual void OnSlot()
    {
        bIsPlaced = false;

        if (cPartnerSpawn != null)
        {
            if (cPartnerSpawn.timing == PartnerSpawnTiming.WhenPlaceHero)
                cPartnerSpawn.DisableUnit();
        }
    }

    public virtual void OnPlace()
    {
        bIsPlaced = true;

        if (cPartnerSpawn != null)
        {
            if (cPartnerSpawn.timing == PartnerSpawnTiming.WhenPlaceHero)
                cPartnerSpawn.PlaceUnit();
        }
    }

    public virtual void OnRemove()
    {   
        isOwn = false;
        bIsPlaced = false;
        cCurrentTile.UnLinkPlaceUnit();

        if (cPartnerSpawn != null)
        {
            if (cPartnerSpawn.timing == PartnerSpawnTiming.WhenPlaceHero)
                cPartnerSpawn.DestroyUnit();
        }
    }

    public virtual void SetupStat()
    {
        if (!bBattleUnit)
            return;

        block.gameObject.SetActive(!bAttackUnit);

        bIsAlive = true;

        if (!bAttackUnit)
            tfUnitBody.localPosition = Vector3.up * 0.1f;

        iCrtAtkAddDmg = 0;

        bArriveTarget = false;
        sUnitStat.iCrtHp = sUnitStat.iMaxHp;
        iEveHp = sUnitStat.iCrtHp;
        sUnitStat.iCrtMP = sUnitStat.iOrgStartMp;
        sUnitStat.iShield = 0;
        sUnitStat.iCrtAttack = sUnitStat.iOrgAttack;
        sUnitStat.fCrtAtkRange = sUnitStat.fOrgAtkRange;
        sUnitStat.fOrgCriRate = 10f;
        sUnitStat.fCrtCriRate = sUnitStat.fOrgCriRate;
        sUnitStat.fOrgCriDamage = 1.5f;
        sUnitStat.fCrtCriDamage = sUnitStat.fOrgCriDamage;
        sUnitStat.fOrgEvasionRate = 1f;
        sUnitStat.fCrtEvasionRate = sUnitStat.fOrgEvasionRate;
        sUnitStat.fOrgDmgIncrease = 0;
        sUnitStat.fCrtDmgIncrease = sUnitStat.fOrgDmgIncrease;
        sUnitStat.fOrgDmgReduction = 0;
        sUnitStat.fCrtDmgReduction = sUnitStat.fOrgDmgReduction;
        sUnitStat.fHealingAmount = 1f;
        fSendDmage = 0;
        fReciveDmage = 0;
        iPlusMp = 0;

        sUnitStat.iCrtAbilPower = sUnitStat.iOrgAbilPower;
        SetMoveSpeed(sUnitStat.fOrgMoveSpeed);
        sUnitStat.iCrtADDef = sUnitStat.iOrgADDef;
        sUnitStat.iCrtAPDef = sUnitStat.iOrgAPDef;

        if (!bAttackUnit && cCurrentTile != null)
        {
            if (cCurrentTile.listTileBuff[(int)TileBuff.Attack])
            {
                sUnitStat.iCrtAttack *= 2;
                sUnitStat.iAddAttack *= 2;
            }
            if (cCurrentTile.listTileBuff[(int)TileBuff.AbilPower])
            {
                sUnitStat.iCrtAbilPower *= 2;
                sUnitStat.iAddPower *= 2;
            }
            if (cCurrentTile.listTileBuff[(int)TileBuff.AtkSpd])
                sUnitStat.fCrtAtkSpeed *= 2f;
        }

        bCanNotGetMp = false;
        bCastingSkill = false;
        cSkill.StopSkill();
        OnOffRangeCircle(false);
        SetAttackRange(sUnitStat.fOrgAtkRange);
        PlayAnimation(UnitAniActionParam.StopSkill);

        if (attackUnit)
            PlayAnimation(UnitAniActionParam.Run);

        ClearBuff();
        ClearCC();
        UpdateBuffValue();

        cSkill.UpdateFinalValue();

        dicCustomMark.Clear();

        cHUDUI.ResetHP();
        cHUDUI.ResetBuffIcons();
        cHUDUI.ResetDmgText();
    }

    public void SetRank(UnitRank _eRank)
    {
        UnitStatus _sTempStat = unitStat;
        _sTempStat.eRank = _eRank;

        sUnitStat = _sTempStat;

        switch (sUnitStat.eRank)
        {
            case UnitRank.Common:
                renBlock.material.color = new Color(0.6f, 0.6f, 0.7f); ;
                break;
            case UnitRank.UnCommon:
                renBlock.material.color = new Color(0.5f, 0.8f, 0.7f);
                break;
            case UnitRank.Rare:
                renBlock.material.color = new Color(0.3f, 0.5f, 0.8f);
                break;
            case UnitRank.Epic:
                renBlock.material.color = new Color(0.5f, 0.3f, 0.7f);
                break;
            case UnitRank.Legendary:
                renBlock.material.color = new Color(0.9f, 0.5f, 0.1f);
                break;
            default:
                break;
        }
    }

    public void InitEffectPool()
    {
        if (cObjPool != null)
        {
            if (cAttackEffect != null)
            {
                cObjPool.AddTargetObj(cAttackEffect.gameObject);
                cObjPool.AddListCount(3);
            }
            else
            {
                GameObject _goEmptyPool = new GameObject();
                _goEmptyPool.name = "EnmpyPool0";
                _goEmptyPool.transform.SetParent(transform);
                cObjPool.AddTargetObj(_goEmptyPool);
                cObjPool.AddListCount(0);
            }

            if (cHitEffect != null)
            {
                cObjPool.AddTargetObj(cHitEffect.gameObject);
                cObjPool.AddListCount(3);
            }
            else
            {
                GameObject _goEmptyPool = new GameObject();
                _goEmptyPool.name = "EnmpyPool1";
                _goEmptyPool.transform.SetParent(transform);
                cObjPool.AddTargetObj(_goEmptyPool);
                cObjPool.AddListCount(0);
            }

            for (int i = 0; i < 3; i++)
            {
                try
                {
                    cObjPool.AddTargetObj(cSkill.skillComponent_Def.listSkillEffect[i].gameObject);
                    cObjPool.AddListCount(2);
                }
                catch
                {
                    GameObject _emptyObj = new GameObject();
                    _emptyObj.name = "DEmpty";
                    _emptyObj.transform.SetParent(transform);
                    cObjPool.AddTargetObj(_emptyObj);
                    cObjPool.AddListCount(0);
                }
            }

            for (int i = 0; i < 3; i++)
            {
                try
                {
                    cObjPool.AddTargetObj(cSkill.skillComponent_Atk.listSkillEffect[i].gameObject);
                    cObjPool.AddListCount(2);
                }
                catch
                {
                    GameObject _emptyObj = new GameObject();
                    _emptyObj.name = "AEmpty";
                    _emptyObj.transform.SetParent(transform);
                    cObjPool.AddTargetObj(_emptyObj);
                    cObjPool.AddListCount(0);
                }
            }

            cObjPool.Init();
        }
    }

    public void SetAtkUnit(bool _bValue)
    {
        bAttackUnit = _bValue;
    }

    public void CreateHUD()
    {
        if (!bBattleUnit)
            return;

        if (bAttackUnit)
            cHUDUI = Instantiate(InGameManager.instance.hud_Atk, InGameManager.instance.uiHudCanvas.transform);
        else
            cHUDUI = Instantiate(InGameManager.instance.hud_Def, InGameManager.instance.uiHudCanvas.transform);

        cHUDUI.SetUnit(this);

        cHUDUI.gameObject.SetActive(false);
        StartCoroutine(AppearHUD());

        cHUDUI.OnOffEquipSlot(false);
        cHUDUI.equpSlot.DisableAllItem();

        cHUDUI.gameObject.name = $"HUD ({strUnitName})";
    }

    IEnumerator AppearHUD()
    {
        for (int i = 0; i < 3; i++)
            yield return new WaitForEndOfFrame();

        cHUDUI.gameObject.SetActive(true);
        cHUDUI.OnOffEmblem(!bAttackUnit);

        cHUDUI.updateHUD.Invoke(false);
    }

    private void SearchTargetUnit()
    {
        if (!gameObject.activeSelf || (cCurrentTile != null && cCurrentTile.bSlot))
            return;

        if (bIsBuffs_Negative[(int)BuffType.Stern] || bIsBuffs_Negative[(int)BuffType.Airborne] || bCastingSkill)
            return;

        if (!InGameManager.instance.bIsRoundStart)
            return;

        if (bAttackUnit && !listBuff.Any(x => x.strBuffCode.Contains("201050")))
            return;

        float _fShortestDest = Mathf.Infinity;
        Unit _cNearstEmemy = null;

        foreach (var item in InGameManager.instance.cUnitController.attackUnitList.Where(x => x.gameObject.activeSelf && !x.IsUnspecified()))
        {
            float _fDistanceToEnemy = Vector3.Distance(new Vector3(tfUnitBody.position.x, item.transform.position.y, tfUnitBody.position.z), item.transform.position);

            if (item.gameObject.activeSelf && item.bIsAlive && item != this && item.bNavStarted && !item.bArriveTarget)
            {
                if (listBuff.Any(x => x.eBuffType == BuffType.Taunt))
                {
                    _fShortestDest = _fDistanceToEnemy;
                    _cNearstEmemy = item;
                    break;
                }
                else if (_fDistanceToEnemy < _fShortestDest)
                {
                    _fShortestDest = _fDistanceToEnemy;
                    _cNearstEmemy = item;
                }
            }
        }

        if (_cNearstEmemy != null && _fShortestDest <= sUnitStat.fCrtAtkRange + 0.5f)
        {
            cAtkTargetUnit = _cNearstEmemy;

            AttackUnit(cAtkTargetUnit as Hero);
        }
        else
            cAtkTargetUnit = null;
    }

    public void StartMoveToTarget()
    {
        navAgent.SetDestination(InGameManager.instance.cAStar.targetTile.tilePos);
        PlayAnimation(UnitAniActionParam.Run);

        if (navAgent.enabled)
            StartCoroutine(CheckArriveCoroutine());
    }

    IEnumerator CheckArriveCoroutine()
    {
        for (int i = 0; i < 10; i++)
            yield return null;

        bNavStarted = true;
        cHUDUI.gameObject.SetActive(true);

        yield return new WaitUntil(() => Vector3.Distance(transform.position, InGameManager.instance.cAStar.targetTile.tilePos) <= 0.1f);

        bArriveTarget = true;

        SetMoveSpeed(0);
        DisableObject();

        int _iDamage = (this is Hero) ? (int)(this as Hero).sUnitStat.eRank : 1;

        InGameManager.instance.cPlayerController.GetCampDamage(_iDamage);
    }

    public void AttackUnit(Unit _cTarget, int _iAddDamge = 0)
    {
        cCrtHitUnit = _cTarget;
        iCrtAtkAddDmg = _iAddDamge;

        Vector3 _v3Height = _cTarget.attackUnit ? Vector3.zero : Vector3.up * 0.1f;
        body.DOLookAt(_cTarget.transform.position + _v3Height, 0.1f);
        PlayAnimation(UnitAniActionParam.Attack);

        if (sUnitStat.bIsProjectile && cAttackEffect != null)
            Invoke(nameof(ShootProjectile), (sUnitStat.fAtkDelay * (fOne / fAnimAtkSpd)) + (0.1f / fAnimAtkSpd));
        else if (!sUnitStat.bDirectAttack)
            NormalAttack();
        else
            Invoke(nameof(DirectAttack), fDelay * (fDelay / (sUnitStat.fCrtAtkSpeed)));
    }

    private void NormalAttack()
    {
        Invoke(nameof(HitUnit), (sUnitStat.fAtkDelay * (fOne / fAnimAtkSpd)) + (0.1f * fAnimAtkSpd));
    }

    private void ShootProjectile()
    {
        Invoke(nameof(HitUnit), Vector3.Distance(body.position, cCrtHitUnit.body.position) / sUnitStat.fProjSpeed);
        TrackingProjectile _cProjectile = cObjPool.GetObj(0).GetComponent<TrackingProjectile>();
        _cProjectile.transform.position = tfUnitBody.position + Vector3.up * 0.5f;
        _cProjectile.transform.rotation = tfUnitBody.rotation;
        _cProjectile.SetTarget(cCrtHitUnit.transform, sUnitStat.fProjSpeed);
        StartCoroutine(DisableEffect(_cProjectile.gameObject, 1));
    }


    private void DirectAttack()
    {
        AbleAttackEffect();
        Invoke(nameof(HitUnit), (sUnitStat.fAtkDelay * fOne / sUnitStat.fCrtAtkSpeed) + Vector3.Distance(transform.position, cCrtHitUnit.transform.position) * fDelay * 0.2f / sUnitStat.fCrtAtkSpeed);
        Transform _tfBodyParent = tfUnitBody.parent;
        Vector3 _v3OrgPos = _tfBodyParent.localPosition;
        Sequence _secquance = DOTween.Sequence();
        _secquance.Append(_tfBodyParent.DOMove(cCrtHitUnit.transform.position + Vector3.down * 0.3f, 0.2f * (fOne / sUnitStat.fCrtAtkSpeed)));
        _secquance.Append(_tfBodyParent.DOLocalMove(_v3OrgPos, 0.2f * (fOne / sUnitStat.fCrtAtkSpeed)));
    }

    private void HitUnit()
    {
        AbleAttackEffect();

        Invoke(nameof(TrueHit), Time.deltaTime);

        AddMP(10 + unitStat.increaseMp_Hit);

        cCrtHitUnit.AddHP(this, cCrtHitUnit.CalcDamage(DamageType.AD, this, sUnitStat.iCrtAttack + sUnitStat.iAddAttack + iCrtAtkAddDmg), DamageType.AD, false, true, true);
        cCrtHitUnit.TakeHit();

        InGameManager.instance.cHeroInfoPanel.UpdateStatus();

        Invoke(nameof(FalseHit), Time.deltaTime * 5f);

        if (skillChain_AtkEnd != null)
        {
            skillChain_AtkEnd.Invoke();
        }

        AbleHitEffect();
    }

    public int CalcDamage(DamageType _eDmgType, Unit _cCastUnit, int _iDamage)
    {
        float _fFinalDamage;

        float _fDefnce = 0;

        switch (_eDmgType)
        {
            case DamageType.AD:
                _fDefnce = sUnitStat.iCrtADDef + sUnitStat.iAddADDef;
                break;
            case DamageType.AP:
                _fDefnce = sUnitStat.iCrtAPDef + sUnitStat.iAddAPDef;
                break;
            case DamageType.True:
                _fDefnce = 0;
                break;
        }

        if (_cCastUnit != null)
        {
            _fFinalDamage = _iDamage * (100f / (100f + _fDefnce)) * (1f + _cCastUnit.sUnitStat.fCrtDmgIncrease * 0.1f) * (1f - sUnitStat.fCrtDmgReduction * 0.1f);
            _fFinalDamage *= _cCastUnit.fSendDmage * fReciveDmage;
        }
        else
            _fFinalDamage = _iDamage * (100f / (100f + _fDefnce)) * (1f - sUnitStat.fCrtDmgReduction * 0.1f);

        return (int)-_fFinalDamage;
    }

    public void TrueHit()
    {
        bHit = true;
    }

    public void FalseHit()
    {
        bHit = false;
    }

    public void PlayAnimation(UnitAniActionParam _eParam, bool _bSkillBool = false)
    {
        switch (_eParam)
        {
            case UnitAniActionParam.Idle:
                anmAnimator.SetBool("bIsRun", false);
                break;
            case UnitAniActionParam.Run:
                anmAnimator.SetBool("bIsRun", true);
                break;
            case UnitAniActionParam.Attack:
                anmAnimator.SetTrigger("tAttack");
                break;
            case UnitAniActionParam.DefSkill:
                if (!_bSkillBool)
                    anmAnimator.SetTrigger("tSkill_D");
                else
                    anmAnimator.SetBool("bSkill_D", true);
                break;
            case UnitAniActionParam.AtkSkill:
                if (!_bSkillBool)
                    anmAnimator.SetTrigger("tSkill_A");
                else
                    anmAnimator.SetBool("bSkill_A", true);
                break;
            case UnitAniActionParam.StopSkill:
                if (_bSkillBool)
                    anmAnimator.SetBool("bSkill_D", false);
                anmAnimator.SetBool("bSkill_A", false);
                break;
            case UnitAniActionParam.Damage:
                anmAnimator.SetTrigger("tDamage");
                break;
            case UnitAniActionParam.Die:
                anmAnimator.SetTrigger("tDie");
                break;
            case UnitAniActionParam.Attack_Arrangement:
                anmAnimator.SetTrigger("tArrangement");
                break;
            case UnitAniActionParam.Airborne:
                anmAnimator.SetBool("bAirbone", true);
                break;
            case UnitAniActionParam.Fall:
                anmAnimator.SetBool("bAirbone", false);
                break;
        }
    }

    private void AbleAttackEffect()
    {
        if (cAttackEffect == null)
            return;

        GameObject _goEffct;

        if (!sUnitStat.bIsProjectile)
        {
            _goEffct = cObjPool.GetObj(0);

            _goEffct.transform.position = tfUnitBody.position;
            _goEffct.transform.rotation = Quaternion.Euler(new Vector3(cAttackEffect.transform.rotation.eulerAngles.x, tfUnitBody.rotation.eulerAngles.y, 0));
            StartCoroutine(DisableEffect(_goEffct.gameObject, 0));
        }
    }

    private void AbleHitEffect()
    {
        if (cHitEffect == null)
            return;

        GameObject _goEffct = cObjPool.GetObj(1);

        _goEffct.transform.position = cAtkTargetUnit.body.position;
        _goEffct.transform.rotation = Quaternion.Euler(new Vector3(cHitEffect.transform.rotation.eulerAngles.x, tfUnitBody.rotation.eulerAngles.y, 0));
        StartCoroutine(DisableEffect(_goEffct.gameObject, 1));
    }


    IEnumerator DisableEffect(GameObject _goObj, int _iType)
    {
        switch (_iType)
        {
            case 0:
                yield return _cNewWait_HideEffect;
                break;
            case 1:
                yield return _cNewWait_HideProjfectile;
                break;
            default:
                break;
        }

        cObjPool.ReturnObj(_goObj, 0);
    }


    public virtual void AddHP(Unit _cCaster, int _iValue, DamageType _eDmgType, bool _bNoneTarget = false, bool _bIsCriticalPossible = false, bool _bIsEvationPossible = false, int _iDamageCode = 0)
    {
        if (!bIsAlive)
            return;

        if (_iValue < 0 && IsUnspecified())
            return;

        iEveHp = sUnitStat.iCrtHp;

        bool _bIsCritical = false;
        bool _bIsEvation = false;

        if (_bIsCriticalPossible)
            _bIsCritical = Tools.GetRandomForPercentResult(_cCaster.sUnitStat.fCrtCriRate);
        if (_bIsEvationPossible)
            _bIsEvation = Tools.GetRandomForPercentResult(_cCaster.sUnitStat.fCrtEvasionRate);

        float _fRatio = _bIsCritical ? _cCaster.sUnitStat.fCrtCriDamage : 1;

        _iValue = Mathf.RoundToInt(_iValue * _fRatio);

        if (_bIsEvation)
            _iValue = 0;

        if (sUnitStat.iShield <= 0)
        {
            int _iFinalValue = sUnitStat.iCrtHp + _iValue;
            _iFinalValue = Mathf.Clamp(_iFinalValue, 0, sUnitStat.iMaxHp);
            sUnitStat.iCrtHp = _iFinalValue;
        }
        else if (_iValue < 0)
        {
            if (Mathf.Abs(_iValue) < sUnitStat.iShield)
            {
                float _iTempDamage = Mathf.Abs(_iValue);

                List<BuffValue> _listBuff = listBuff;

                for (int i = 0; i < listBuff.Count; i++)
                {
                    if (listBuff[i].eBuffType == BuffType.Shield)
                    {
                        if (_iTempDamage > 0)
                        {
                            BuffValue _cTempBuff = _listBuff[i];
                            _cTempBuff.fValue -= _iTempDamage;
                            _listBuff[i] = _cTempBuff;
                            _iTempDamage -= _cTempBuff.fValue;
                        }
                    }

                    if (_iTempDamage <= 0)
                        break;
                }

                listBuff.RemoveAll(x => x.eBuffType == BuffType.Shield && x.fValue <= 0);

                UpdateBuffValue();
            }
            else
            {
                int _iFinalValue = sUnitStat.iCrtHp - (Mathf.Abs(_iValue) - sUnitStat.iShield);

                listBuff.RemoveAll(x => x.eBuffType == BuffType.Shield);
                UpdateBuffValue();

                _iFinalValue = Mathf.Clamp((int)(_iFinalValue), 0, sUnitStat.iMaxHp);
                sUnitStat.iCrtHp = _iFinalValue;
            }
        }

        cHUDUI.UpdateHP(_bNoneTarget);
        cHUDUI.AbleDamageText(Mathf.Abs(_iValue), _eDmgType, _bIsCritical, _bIsEvation, _iDamageCode);

        if (InGameManager.instance.cHeroInfoPanel.gameObject.activeSelf)
            InGameManager.instance.cHeroInfoPanel.UpdateStatus();

        cDamageMineUnit = _cCaster;

        if (hitSkillChain != null)
        {
            hitSkillChain.Invoke();
        }

        if (sUnitStat.iCrtHp <= 0)
            DieUnit();
    }

    public void ChangeMP(int _ivalue)
    {
        if (!bIsAlive)
            return;

        int _iFinalValue = _ivalue;
        _iFinalValue = Mathf.Clamp(_iFinalValue, 0, sUnitStat.iMaxMp);

        sUnitStat.iCrtMP = _iFinalValue;
        cHUDUI.UpdateMP();
        InGameManager.instance.cHeroInfoPanel.UpdateHPMP();
    }

    public void AddMP(int _iValue)
    {
        if (!bIsAlive || bCanNotGetMp || bIsBuffs_Negative[(int)BuffType.Silent])
            return;

        int _iFinalValue = sUnitStat.iCrtMP + _iValue + iPlusMp;
        _iFinalValue = Mathf.Clamp(_iFinalValue, 0, sUnitStat.iMaxMp);

        sUnitStat.iCrtMP = _iFinalValue;
        cHUDUI.UpdateMP();

        if (!bAttackUnit)
            InGameManager.instance.cHeroInfoPanel.UpdateHPMP();

        if (sUnitStat.iCrtMP >= sUnitStat.iMaxMp)
            StartCoroutine(CastingSkillCoroutine(bAttackUnit));
    }

    public void SetFreindUnit(Unit _cPartnerUnit)
    {
        cFreindUnit = _cPartnerUnit;
    }

    public void SetMoveSpeed(float _fValue)
    {
        sUnitStat.fCrtMoveSpeed = _fValue;
        if (navAgent != null)
            navAgent.speed = sUnitStat.fCrtMoveSpeed;

        if (bIsBuffs_Negative[(int)BuffType.Stern] || bIsBuffs_Negative[(int)BuffType.Airborne] || bIsBuffs_Negative[(int)BuffType.Stiff])
            navAgent.speed = 0;

        fAnmMoveSpd = sUnitStat.fCrtMoveSpeed;
        fAnmMoveSpd = Mathf.Clamp(fAnmMoveSpd, 0, 2f);
        anmAnimator.SetFloat("fMoveSpeed", _fValue);

        if (InGameManager.instance.cHeroInfoPanel.gameObject.activeSelf)
            InGameManager.instance.cHeroInfoPanel.UpdateStatus();
    }

    public void SetLevel(int _iLevel, int _iUnitId = 0)
    {
        if (_iLevel == 1)
            return;

        if (_iUnitId == 0)
            _iUnitId = iUnitID;

        sOriginUnitStatus = GameManager.instance.GetHeroWithId(_iUnitId).sUnitStat;
        iUnitLevel = _iLevel;
        v3OriginSize = Vector3.one;
        sUnitStat.iMaxHp = (int)((float)sOriginUnitStatus.iMaxHp * 1.8f * (_iLevel == 3 ? 1.8f : 1f));
        sUnitStat.iOrgAttack = (int)((float)sOriginUnitStatus.iOrgAttack * 1.8f * (_iLevel == 3 ? 1.8f : 1f));
        tfUnitBody.localScale = v3OriginSize * (1f + (0.2f * (iUnitLevel)));
        foreach (var item in renMeshRenders)
        {
            switch (_iLevel)
            {
                case 3:
                    item.material.SetColor("rimColor0", Color.yellow);
                    break;
            }
        }
        iHeroCost = ((int)sUnitStat.eRank * ((_iLevel == 2) ? 3 : 9)) - 1;
        SetupStat();
        if (this is Hero && InGameManager.instance.cHeroInfoPanel.currentHero == this)
            InGameManager.instance.cHeroInfoPanel.UpdateInfo(this as Hero);
        cSkill.UpdateFinalValue();
        InGameManager.instance.cAStar.PathEvent();

        if (cPartnerSpawn)
            cPartnerSpawn.OnSpawn();
    }

    // ·¹º§¾÷
    public void LevelUp(List<Unit> _listSubhero)
    {
        if (InGameManager.instance.canLevelUp)
            StartCoroutine(LevelUpCoroutine(_listSubhero));
    }

    IEnumerator LevelUpCoroutine(List<Unit> _listSubhero)
    {
        bLeveling = true;

        foreach (var item in _listSubhero)
        {
            item.PlayDissolve(2, 0, 1f, 0.5f);
            item.bLeveling = true;
        }

        yield return new WaitForSeconds(0.5f);

        foreach (var item in _listSubhero)
        {
            item.currentTile.SellUnit(false);
            item.bLeveling = false;

            Transform _levelUpEffect = Instantiate(Resources.Load<Transform>("Unit/BasicEffect/LevelUp Effect"));

            _levelUpEffect.gameObject.SetActive(true);
            _levelUpEffect.position = item.tfBodyHolder.position;
            _levelUpEffect.LookAt(currentTile.tilePos);
            _levelUpEffect.DOMove(currentTile.tilePos, 1f).OnComplete(() => Destroy(_levelUpEffect.gameObject));
        }

        yield return new WaitForSeconds(0.25f);

        PlayDissolve(0, 0f, 0.5f, 0.25f);

        yield return new WaitForSeconds(0.25f);

        PlayDissolve(0, 0.5f, 0f, 0.25f);
        SetupStat();
        iUnitLevel += 1;
        sUnitStat.iMaxHp = (int)((float)sOriginUnitStatus.iMaxHp * 1.8f * (iUnitLevel == 3 ? 1.8f : 1f));
        sUnitStat.iOrgAttack = (int)((float)sOriginUnitStatus.iOrgAttack * 1.8f * (iUnitLevel == 3 ? 1.8f : 1f));
        tfUnitBody.DOScale(v3OriginSize * (1f + (0.2f * (iUnitLevel - 1))), 0.25f);
        foreach (var item in renMeshRenders)
        {
            switch (iUnitLevel)
            {
                case 3:
                    item.material.SetColor("rimColor0", Color.yellow);
                    break;
            }
        }

        iHeroCost = ((int)sUnitStat.eRank * ((iUnitLevel == 2) ? 3 : 9)) - 1;
        SetupStat();
        tfUnitBody.gameObject.SetActive(true);
        cHUDUI.updateHUD(false);
        if (this is Hero && InGameManager.instance.cHeroInfoPanel.currentHero == this)
            InGameManager.instance.cHeroInfoPanel.UpdateInfo(this as Hero);
        cSkill.UpdateFinalValue();
        InGameManager.instance.cAStar.PathEvent();
        if (cPartnerSpawn)
            cPartnerSpawn.OnSpawn();

        yield return null;

        bLeveling = false;
    }

    public BuffValue GetBuff(BuffType _eBuffType, Unit _cCaster, float _fValue, float _fTime, bool _bHalfInfinity = false, bool _bInfinity = false, string _strCustomCode = "000000")
    {
        if (!bIsAlive)
            return null;

        BuffValue _sBuffValue = new BuffValue(_eBuffType, _cCaster, _fValue, _fTime, _bHalfInfinity, _bInfinity, _strCustomCode);

        StartCoroutine(GetBuffCoroutine(_sBuffValue));

        return _sBuffValue;
    }

    IEnumerator GetBuffCoroutine(BuffValue _sBuffValue)
    {
        listBuff.Add(_sBuffValue);

        if (_sBuffValue.bIsPositive)
            bIsBuffs_Positive[(int)_sBuffValue.eBuffType] = true;
        else
            bIsBuffs_Negative[(int)_sBuffValue.eBuffType] = true;

        bool _isFall = false;

        if (_sBuffValue.eBuffType == BuffType.Airborne)
        {
            tfUnitBody.DOLocalMove(Vector3.up * 1f, 0.25f);
            PlayAnimation(UnitAniActionParam.Airborne);
        }

        UpdateBuffValue();

        if (_sBuffValue.bHalfInfinity)
            yield break;

        while (_sBuffValue.fCrtTime < _sBuffValue.fFullTime)
        {
            _sBuffValue.fCrtTime += Time.deltaTime;

            if (_sBuffValue.fCrtTime >= _sBuffValue.fFullTime)
                break;

            yield return null;

            if (_sBuffValue.eBuffType == BuffType.Airborne)
            {
                if (_sBuffValue.fCrtTime >= _sBuffValue.fFullTime - 0.25f && !_isFall)
                {
                    tfUnitBody.DOLocalMove(Vector3.zero, 0.25f);
                    _isFall = true;
                }
            }
        };

        listBuff.RemoveAll(x => x.strBuffCode == _sBuffValue.strBuffCode);

        UpdateBuffValue();
    }

    public void ExitBuff(string _strBuffCode)
    {
        listBuff.RemoveAll(x => x.strBuffCode.Contains(_strBuffCode));
        UpdateBuffValue();
    }

    public bool GetCustomMark(string _key)
    {
        if (dicCustomMark.ContainsKey(_key))
            return dicCustomMark[_key];

        return false;
    }

    public void ClearBuff()
    {
        for (int i = 0; i < bIsBuffs_Positive.Length; i++)
            bIsBuffs_Positive[i] = false;

        listBuff.RemoveAll(x => x.bIsPositive && !x.bInfiniy);
    }

    public void ClearCC()
    {
        for (int i = 0; i < bIsBuffs_Positive.Length; i++)
            bIsBuffs_Negative[i] = false;

        listBuff.RemoveAll(x => !x.bIsPositive && !x.bInfiniy);
    }

    public void SetAttackSpeed(float _fValue)
    {
        sUnitStat.fCrtAtkSpeed = _fValue;
        sUnitStat.fCrtAtkSpeed = Mathf.Clamp(sUnitStat.fCrtAtkSpeed, 0, 5f);
        fAttackDelay = fOne / sUnitStat.fCrtAtkSpeed;

        fAnimAtkSpd = sUnitStat.fCrtAtkSpeed;
        fAnimAtkSpd = Mathf.Clamp(fAnimAtkSpd, 1f, 2f);

        anmAnimator.SetFloat("fAttackSpeed", fAnimAtkSpd);

        CancelInvoke(nameof(SearchTargetUnit));
        InvokeRepeating(nameof(SearchTargetUnit), fAttackDelay, fAttackDelay);
    }

    public void SetAttackRange(float _fValue)
    {
        sUnitStat.fCrtAtkRange = _fValue;

        ParticleSystem.MainModule _cPtcM = tfRangeCircle.GetComponentInChildren<ParticleSystem>().main;
        _cPtcM.startSize = (sUnitStat.fCrtAtkRange + 0.5f) * 3f;

        if (tfRangeCircle.gameObject.activeSelf)
        {
            OnOffRangeCircle(false);
            OnOffRangeCircle(true);
        }
    }

    public void OnOffRangeCircle(bool _bValue)
    {
        tfRangeCircle.gameObject.SetActive(_bValue);
    }

    IEnumerator CastingSkillCoroutine(bool _bAttackUnit)
    {
        yield return new WaitUntil(() => !bIsBuffs_Negative[(int)BuffType.Airborne]);
        yield return new WaitUntil(() => !bIsBuffs_Negative[(int)BuffType.Stern]);
        yield return new WaitUntil(() => !bIsBuffs_Negative[(int)BuffType.Silent]);

        if (_bAttackUnit)
            CastAttackSkill();
        else
            CastDefenceSkill();
    }

    public void SetCastingSkill(bool _bValue)
    {
        bCastingSkill = _bValue;
    }

    private void CastAttackSkill()
    {
        if (atkSkilChain == null || bIsBuffs_Negative[(int)BuffType.Stern] || bIsBuffs_Negative[(int)BuffType.Airborne] || bIsBuffs_Negative[(int)BuffType.Silent])
            return;

        atkSkilChain.Invoke();
    }

    public void CastDefenceSkill()
    {
        if (defSkillChain == null || bIsBuffs_Negative[(int)BuffType.Stern] || bIsBuffs_Negative[(int)BuffType.Airborne] || bIsBuffs_Negative[(int)BuffType.Silent])
            return;

        if (listBuff.Any(x => x.strBuffCode.Contains("201050")))
            return;

        if (!attackUnit)
            defSkillChain.Invoke();
    }

    public GameObject SpawnAtkSkillEffect(int _iIndex, float _fLifeTime)
    {
        GameObject _goEffect = cObjPool.GetObj(5 + _iIndex);

        if (_fLifeTime > 0)
            StartCoroutine(ReturnAtkSkillEffectCoroutine(_fLifeTime, _goEffect.GetComponent<PoolableObject>()));

        return _goEffect;
    }

    IEnumerator ReturnAtkSkillEffectCoroutine(float _fLifeTime, PoolableObject _cObj)
    {
        yield return new WaitForSeconds(_fLifeTime);

        _cObj.ReturnObj();
    }

    public void AppearBody(bool _bValue)
    {
        tfUnitBody.gameObject.SetActive(_bValue);
        SetupStat();
    }

    public GameObject SpawnDefSkillEffect(int _iIndex, float _fLifeTime)
    {
        GameObject _goEffect = cObjPool.GetObj(2 + _iIndex);

        if (_fLifeTime > 0)
            StartCoroutine(ReturnDefSkillEffectCoroutine(_fLifeTime, _goEffect.GetComponent<PoolableObject>()));

        return _goEffect;
    }

    IEnumerator ReturnDefSkillEffectCoroutine(float _fLifeTime, PoolableObject _cObj)
    {
        yield return new WaitForSeconds(_fLifeTime);

        _cObj.ReturnObj();
    }


    protected virtual void TakeHit()
    {
        AddMP(10);
    }

    public void UpdateBuffValue()
    {
        float[] _fValues = new float[(int)BuffType.Count];

        for (int i = 0; i < _fValues.Length; i++)
        {
            if (i <= (int)BuffType.APDef)
                _fValues[i] = 0;
            else
                _fValues[i] = 1f;
        }

        for (int i = 0; i < listBuff.Count; i++)
        {
            for (int j = 0; j < _fValues.Length; j++)
            {
                if (listBuff[i].eBuffType == (BuffType)j)
                {
                    if (j <= (int)BuffType.APDef)
                        _fValues[j] += listBuff[i].fValue;
                    else
                        _fValues[j] *= 1f + listBuff[i].fValue * 0.01f;
                }
            }
        }

        for (int i = 0; i < bIsBuffs_Positive.Length; i++)
        {
            int _index = i;
            bIsBuffs_Positive[_index] = listBuff.Any(x => (x.eBuffType == (BuffType)_index) && x.bIsPositive);
            bIsBuffs_Negative[_index] = listBuff.Any(x => (x.eBuffType == (BuffType)_index) && !x.bIsPositive);

            cHUDUI.OnOffBuffIcon((BuffType)_index, true, bIsBuffs_Positive[_index]);
            cHUDUI.OnOffBuffIcon((BuffType)_index, false, bIsBuffs_Negative[_index]);
        }

        unitCollider.enabled = !IsUnspecified();

        sUnitStat.iAddAttack = 0;
        sUnitStat.iAddPower = 0;

        sUnitStat.iAddAttack = (int)_fValues[(int)BuffType.Attack];
        sUnitStat.iAddPower = (int)_fValues[(int)BuffType.Power];
        sUnitStat.iShield = (int)_fValues[(int)BuffType.Shield];
        sUnitStat.iCrtADDef = sUnitStat.iOrgADDef + (int)_fValues[(int)BuffType.ADDef];
        sUnitStat.iCrtAPDef = sUnitStat.iOrgAPDef + (int)_fValues[(int)BuffType.APDef];
        iPlusMp = (int)_fValues[(int)BuffType.PlusMp];
        fSendDmage = _fValues[(int)BuffType.SendDamage];
        fReciveDmage = _fValues[(int)BuffType.ReciveDamage];

        SetMoveSpeed(sUnitStat.fOrgMoveSpeed * _fValues[(int)BuffType.MoveSpeed]);
        SetAttackSpeed(sUnitStat.fOrgAtkSpeed * _fValues[(int)BuffType.AtkSpeed]);
        //SetAttackRange(sStat.fOrgAtkRange * _fValues[(int)BuffType.Range]);

        if (bIsBuffs_Negative[(int)BuffType.Airborne] || bIsBuffs_Negative[(int)BuffType.Stern] || bIsBuffs_Negative[(int)BuffType.Stiff])
            navAgent.speed = 0;

        if (bIsBuffs_Negative[(int)BuffType.Stern] || bIsBuffs_Negative[(int)BuffType.Stiff])
            PlayAnimation(UnitAniActionParam.Idle);
        else if (bAttackUnit)
            PlayAnimation(UnitAniActionParam.Run);
        if (bIsBuffs_Negative[(int)BuffType.Airborne])
            PlayAnimation(UnitAniActionParam.Airborne);

        try
        {
            cHUDUI.updateHUD(true);
        }
        catch { }
        InGameManager.instance.cHeroInfoPanel.UpdateStatus();
    }

    public bool IsUnspecified()
    {
        return bIsBuffs_Positive[12] || bIsBuffs_Negative[12];
    }

    public void DieUnit()
    {
        bIsAlive = false;

        StopAllCoroutines();
        SetMoveSpeed(0);

        if (navAgent.enabled)
            navAgent.SetDestination(body.position);

        PlayAnimation(UnitAniActionParam.StopSkill);
        PlayAnimation(UnitAniActionParam.Die);

        ClearBuff();

        CancelInvoke();

        if (bAttackUnit)
            Invoke(nameof(DisableObject), 3f);
    }

    private void DisableObject()
    {
        if (cHUDUI != null)
            cHUDUI.gameObject.SetActive(false);
        gameObject.SetActive(false);
        InGameManager.instance.stageController.CheckComplete();
    }

    public void AddCustomMark(string _key, bool _value)
    {
        dicCustomMark.Add(_key, _value);
    }

    public void RemvoeCustomMark(string _key)
    {
        dicCustomMark.Remove(_key);
    }

    public void SetCanNotGetMp(bool _value)
    {
        bCanNotGetMp = _value;
    }

    public void OnOffNavAgent(bool _bOn)
    {
        navAgent.enabled = _bOn;

        if (_bOn)
        {
            navAgent.SetDestination(InGameManager.instance.cAStar.targetTile.tilePos);
            PlayAnimation(UnitAniActionParam.Run);
        }
    }

    public Tile GetStayTile()
    {
        Ray _ray = new Ray(transform.position + Vector3.up * 0.5f, -transform.up);
        RaycastHit _hit;
        LayerMask _layer = 1 << LayerMask.NameToLayer("Tile");

        if (Physics.Raycast(_ray, out _hit, 2f, _layer))
            return _hit.transform.GetComponent<Tile>();
        else
            return null;
    }

    public void PlayDissolve(int _iType, float _fStartValue, float _fTargetValue, float _fDuration)
    {
        StartCoroutine(PlayDissolveCoroutine(_iType, _fStartValue, _fTargetValue, _fDuration));
    }

    IEnumerator PlayDissolveCoroutine(int _iType, float _fStartValue, float _fTargetValue, float _fDuration)
    {
        foreach (var item in renMeshRenders)
            AdvancedDissolveProperties.Cutout.Standard.UpdateLocalProperty(item.material, AdvancedDissolveProperties.Cutout.Standard.Property.Clip, _fStartValue);
        foreach (var item in renSMeshRenders)
            AdvancedDissolveProperties.Cutout.Standard.UpdateLocalProperty(item.material, AdvancedDissolveProperties.Cutout.Standard.Property.Clip, _fStartValue);

        SpawnBlockAnimation(_iType);

        yield return new WaitForSeconds(0.1f);

        SetDissolveValue(_fStartValue, _fTargetValue, _fDuration);
    }

    private void SpawnBlockAnimation(int _iType)
    {
        Vector3 _blockOrgSize = new Vector3(1.4f, 0.1f, 1.4f);

        switch (_iType)
        {
            case 0:
                tfBlock.transform.localScale = _blockOrgSize;
                return;
            case 1:
                tfBlock.transform.localScale = Vector3.zero;
                tfBlock.transform.DOScale(_blockOrgSize, 0.1f);
                break;
            case 2:
                tfBlock.transform.localScale = _blockOrgSize;
                tfBlock.transform.DOScale(Vector3.zero, 0.1f);
                break;
        }
    }

    private void SetDissolveValue(float _fStartValue, float _fTargetValue, float _fDuration)
    {
        float _fCrtValue = _fStartValue;

        foreach (var item in renMeshRenders)
        {
            DOTween.To(() => _fCrtValue, x => _fCrtValue = x, _fTargetValue, _fDuration).SetEase(Ease.Linear).OnUpdate(() => OnTweenUpdate(item.material, _fCrtValue));
        }

        foreach (var item in renSMeshRenders)
        {
            AdvancedDissolveProperties.Cutout.Standard.UpdateLocalProperty(item.material, AdvancedDissolveProperties.Cutout.Standard.Property.Clip, 1f);

            DOTween.To(() => _fCrtValue, x => _fCrtValue = x, _fTargetValue, _fDuration).SetEase(Ease.Linear).OnUpdate(() => OnTweenUpdate(item.material, _fCrtValue));
        }
    }

    private void OnTweenUpdate(Material _mat, float _value)
    {
        AdvancedDissolveProperties.Cutout.Standard.UpdateLocalProperty(_mat, AdvancedDissolveProperties.Cutout.Standard.Property.Clip, _value);
    }

    public int unitID { get => iUnitID; set => iUnitID = value; }
    public string unitName { get => strUnitName; set => strUnitName = value; }
    public Tile currentTile { get => cCurrentTile; }
    public Transform bodyHolder { get => tfBodyHolder; }
    public Transform body { get => tfUnitBody; }
    public Sprite unitIllust { get => imgUnitIllust; set => imgUnitIllust = value; }
    public Sprite faceIllust { get => imgFaceIllust; set => imgFaceIllust = value; }
    public bool battleUnit { get => bBattleUnit; }
    public bool attackUnit { get => bAttackUnit; set => bAttackUnit = value; }
    public bool canNotBlock { get => bCanNotBlock; }
    public bool canNotMove { get => bAttackUnit || bCanNotMove; }
    public bool isSlot { get => cCurrentTile.bSlot; }
    public bool canNotKeepSlot { get => bCanNotKeepSlot; }
    public bool canNotSell { get => bCanNotSell; }
    public int unitLevel { get => iUnitLevel; }
    public bool hit { get => bHit; }
    public float atkDelay { get => fAttackDelay; }
    public float anmMoveSpeed { get => fAnmMoveSpd; }
    public float anmAtkSpeed { get => fAnimAtkSpd; }
    public Unit targetUnit { get => cAtkTargetUnit; }
    public Unit damagedMineUnit { get => cDamageMineUnit; }
    public bool alive { get => bIsAlive; }
    public bool isOwn { get => bIsOwn; set => bIsOwn = value; }
    public bool isPlaced { get => bIsPlaced; set => bIsPlaced = value; }
    public bool navStarted { get => navStarted; }
    public bool arriveTarget { get => bArriveTarget; }
    public NavMeshAgent agent { get => navAgent; }
    public UnitStatus unitStat { get => sUnitStat; set => sUnitStat = value; }
    public HeroSkill skill { get => cSkill; set => cSkill = value; }
    public HeroEffect atkEffect { get => cAttackEffect; set => cAttackEffect = value; }
    public HeroEffect hitEffect { get => cHitEffect; set => cHitEffect = value; }
    public UnitHUD hud { get => cHUDUI; }
    public int eveHp { get => iEveHp; }
    public Transform block { get => tfBlock; }
    public bool navStart { get => bNavStarted; }
    public Unit crtHitUnit { get => cCrtHitUnit; }
    public List<BuffValue> buffList { get => listBuff; }
    public bool[] buffPositive { get => bIsBuffs_Positive; }
    public bool[] buffNegative { get => bIsBuffs_Negative; }
    public Unit friendUnit { get => cFreindUnit; }
    public int heroCost { get => iHeroCost; }
    public bool leveling { get => bLeveling; }
}
