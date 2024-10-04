using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.UI.Image;

public enum SkillTiming { Active, Passive_Always, Passive_Attack_End, Passive_HitEnd, Passive_BasicHitEnd }

public class HeroSkill : MonoBehaviour
{
    public delegate void CastAtkSkill();

    [SerializeField, ReadOnlyInspector]
    private Unit cUnit;
    [SerializeField]
    private HeroSkillComponent sDefSkillComponent, sAtkSkillComponent;

    public CastAtkSkill cCastAtkSkill;
    private LayerMask cLayerMask;

    [SerializeField, ReadOnlyInspector]
    private List<HitBox> listHitBox;

    private WaitForSeconds waitDefault;
    private WaitForEndOfFrame waitFrame;

    void Awake()
    {
        waitDefault = new WaitForSeconds(0.1f);
        waitFrame = new WaitForEndOfFrame();

        cLayerMask = 1 << LayerMask.NameToLayer("Unit");
        if (GetComponentInChildren<HitBox>() != null)
            listHitBox = GetComponentsInChildren<HitBox>().ToList();

        foreach (var item in listHitBox)
            item.gameObject.SetActive(false);
    }

    void Start()
    {
        SetDefSkill();
        SetAtkSkill();
    }

    [ContextMenu("Init")]
    private void Init()
    {
        cUnit = GetComponent<Unit>();
        cUnit.skill = this;
    }

    private void SetDefSkill()
    {
        switch (sDefSkillComponent.eTiming)
        {
            case SkillTiming.Active:
                if (cUnit.unitStat.iMaxMp != 0)
                {
                    cUnit.defSkillChain -= DefenseCast;
                    cUnit.defSkillChain += DefenseCast;
                }
                break;
            case SkillTiming.Passive_Always:
                break;
            case SkillTiming.Passive_Attack_End:
                cUnit.skillChain_AtkEnd -= DefenseCast;
                cUnit.skillChain_AtkEnd += DefenseCast;
                break;
            case SkillTiming.Passive_HitEnd:
                cUnit.hitSkillChain -= Hit_EndCast;
                cUnit.hitSkillChain += Hit_EndCast;
                break;
            case SkillTiming.Passive_BasicHitEnd:
                break;
        }
    }

    private void SetAtkSkill()
    {
        switch (sAtkSkillComponent.eTiming)
        {
            case SkillTiming.Active:
                if (cUnit.unitStat.iMaxMp != 0)
                    cUnit.atkSkilChain += AttackCast;
                break;
            case SkillTiming.Passive_Always:
                break;
            case SkillTiming.Passive_Attack_End:
                cUnit.skillChain_AtkEnd += AttackCast;
                break;
        }
    }

    public void UpdateFinalValue()
    {
        ConvertFinalValue(sDefSkillComponent);
        ConvertFinalValue(sAtkSkillComponent);
    }

    private void ConvertFinalValue(HeroSkillComponent _sSkill)
    {
        string _strResult = _sSkill.strExplanation.Replace("<Attack>", (cUnit.unitStat.iCrtAttack + cUnit.unitStat.iAddAttack).ToString());
        _strResult = _strResult.Replace("<Level>", cUnit.unitLevel.ToString());
        _strResult = _strResult.Replace("<AP>", (cUnit.unitStat.iCrtAbilPower + cUnit.unitStat.iAddPower).ToString());
        _strResult = _strResult.Replace("<Power>", ((cUnit.unitStat.iCrtAbilPower + cUnit.unitStat.iAddPower) * 0.01f).ToString());
        _strResult = _strResult.Replace("<ADDef>", cUnit.unitStat.iCrtADDef.ToString());
        _strResult = _strResult.Replace("<APDef>", cUnit.unitStat.iCrtAPDef.ToString());

        _strResult = _strResult.Replace("<*>", "*");
        _strResult = _strResult.Replace("</>", "/");
        _strResult = _strResult.Replace("<+>", "+");
        _strResult = _strResult.Replace("<->", "-");

        for (int i = 0; i < _sSkill.listValue.Count; i++)
        {
            int _iIndex = i;

            if (_sSkill.listValue[i].value.Length <= 1)
                _strResult = _strResult.Replace($"<Value0{_iIndex}>", _sSkill.listValue[_iIndex].value[0].ToString());
            if (_sSkill.listValue[i].value.Length > 1)
                _strResult = _strResult.Replace($"<Value0{_iIndex}>", _sSkill.listValue[_iIndex].value[cUnit.unitLevel - 1].ToString());
        }

        if (_strResult.Contains('['))
        {
            string _strFinal_0 = Tools.GetMiddleString(_strResult, "[", "]");
            _sSkill.listFinalValue[0] = (float)Tools.GetStringCalculator(_strFinal_0);
        }
        if (_strResult.Contains('|'))
        {
            string _strFinal_1 = Tools.GetMiddleString(_strResult, "|", "|");
            _sSkill.listFinalValue[1] = (float)Tools.GetStringCalculator(_strFinal_1);
        }
        if (_strResult.Contains('#'))
        {
            string _strFinal_2 = Tools.GetMiddleString(_strResult, "#", "#");
            _sSkill.listFinalValue[2] = (float)Tools.GetStringCalculator(_strFinal_2);
        }
    }

    public void DefenseCast()
    {
        Invoke($"Skill_{sDefSkillComponent.strHeroName}_{sDefSkillComponent.iSkillID}_Defense", 0);
    }

    public void AttackCast()
    {
        Invoke($"Skill_{sAtkSkillComponent.strHeroName}_{sAtkSkillComponent.iSkillID}_Attack", 0);
    }

    private void Hit_EndCast()
    {
        if (!cUnit.attackUnit)
            Invoke($"Skill_{sDefSkillComponent.strHeroName}_{sDefSkillComponent.iSkillID}_Defense", 0);
        else
            Invoke($"Skill_{sDefSkillComponent.strHeroName}_{sDefSkillComponent.iSkillID}_Attack", 0);
    }

    public void StopSkill()
    {
        StopAllCoroutines();
    }

    private Unit GetNearUnit(float _fRange = Mathf.Infinity)
    {
        float _fShortestDest = Mathf.Infinity;
        Unit _cNearstEmemy = null;

        if (!cUnit.attackUnit)
        {
            foreach (var item in InGameManager.instance.cUnitController.attackUnitList.Where(x => x.gameObject.activeSelf && !x.IsUnspecified()))
            {
                float _fDistanceToEnemy = Vector3.Distance(new Vector3(cUnit.bodyHolder.position.x, item.transform.position.y, cUnit.bodyHolder.position.z), item.transform.position);

                if (item.gameObject.activeSelf && item.alive && item.navStart && !item.arriveTarget)
                {
                    if (_fDistanceToEnemy < _fShortestDest)
                    {
                        _fShortestDest = _fDistanceToEnemy;
                        if (item.alive)
                            _cNearstEmemy = item;
                    }
                }
            }
        }
        else
        {
            foreach (var item in InGameManager.instance.cPlayerController.placedUnits.Where(x => x.gameObject.activeSelf && !x.IsUnspecified()))
            {
                float _fDistanceToEnemy = Vector3.Distance(new Vector3(cUnit.bodyHolder.position.x, item.transform.position.y, cUnit.bodyHolder.position.z), item.transform.position);

                if (item.gameObject.activeSelf && item.alive && !item.arriveTarget)
                {
                    if (_fDistanceToEnemy < _fShortestDest)
                    {
                        _fShortestDest = _fDistanceToEnemy;
                        if (item.alive)
                            _cNearstEmemy = item;
                    }
                }
            }
        }

        return _fShortestDest <= _fRange ? _cNearstEmemy : null;
    }

    private List<Unit> GetNearUnits(int _iCount, float _fRange = Mathf.Infinity)
    {
        List<Unit> _listAtkUnit = InGameManager.instance.cUnitController.attackUnitList;
        List<Unit> _listNearEmemy = new List<Unit>();

        _listAtkUnit = _listAtkUnit.OrderBy(x => Vector3.Distance(cUnit.transform.position, x.transform.position)).ToList();

        for (int i = 0; i < _listAtkUnit.Count; i++)
        {
            if (_listNearEmemy.Count <= i && Vector3.Distance(cUnit.bodyHolder.position, _listAtkUnit[i].bodyHolder.position) <= _fRange)
            {
                if (_listAtkUnit[i].alive)
                {
                    _listNearEmemy.Add(_listAtkUnit[i]);

                    if (_listNearEmemy.Count >= _iCount)
                        break;
                }
            }
        }

        return _listNearEmemy;
    }

    private Vector3 GetRandomRout()
    {
        int _iRandom = UnityEngine.Random.Range(1, InGameManager.instance.cAStar.finalTileList.Count - 1);

        return InGameManager.instance.cAStar.finalTileList[_iRandom].tilePos;
    }

    private Vector3 NormalizeRotation()
    {
        float _fRotateY = cUnit.body.transform.rotation.eulerAngles.y;

        if (_fRotateY >= 45 && _fRotateY < 135f)
            return Vector3.up * 90f;
        else if (_fRotateY >= 135f && _fRotateY < 225f)
            return Vector3.up * 180f;
        else if (_fRotateY >= 225f && _fRotateY < 315)
            return Vector3.up * 270;
        else
            return Vector3.zero;
    }

    public float GetDistance(Hero _cTargetHero)
    {
        return Vector3.Distance(new Vector3(cUnit.transform.position.x, 0.5f, cUnit.transform.position.z), new Vector3(_cTargetHero.transform.position.x, 0.5f, _cTargetHero.transform.position.z));
    }

    #region Belber
    private void Skill_Belber_201000_Defense()
    {
        StopCoroutine(nameof(Skill_Belber_201000_Defense_Coroutine));
        StartCoroutine(nameof(Skill_Belber_201000_Defense_Coroutine));
    }

    IEnumerator Skill_Belber_201000_Defense_Coroutine()
    {
        cUnit.SetCastingSkill(true);

        WaitForSeconds _newWait = new WaitForSeconds(sDefSkillComponent.listValue[3].value[0]);

        cUnit.PlayAnimation(UnitAniActionParam.DefSkill, true);

        Transform _tfEffect = cUnit.SpawnDefSkillEffect(0, sDefSkillComponent.listValue[2].value[0]).transform;
        _tfEffect.transform.SetParent(cUnit.body);
        _tfEffect.transform.localPosition = Vector3.zero;

        yield return _newWait;

        cUnit.ChangeMP(0);

        for (int i = 0; i < sDefSkillComponent.listValue[2].value[0]/sDefSkillComponent.listValue[3].value[0]; i++)
        {
            yield return _newWait;

            foreach (var item in InGameManager.instance.cUnitController.attackUnitList)
            {
                if (item.gameObject.activeSelf && item.alive && item.navStart && !item.arriveTarget)
                {
                    if (Vector3.Distance(new Vector3(cUnit.body.position.x, item.transform.position.y, cUnit.body.position.z), item.transform.position) < sDefSkillComponent.listValue[4].value[0])
                        item.AddHP(cUnit, item.CalcDamage(DamageType.AP, cUnit, (int)sDefSkillComponent.listFinalValue[0]), DamageType.AP, false, false, false, 201000);
                }
            }
        }

        cUnit.PlayAnimation(UnitAniActionParam.StopSkill, true);

        cUnit.SetCastingSkill(false);
    }

    private void Skill_Belber_202000_Attack()
    {
        StopCoroutine(nameof(Skill_Belber_202000_Attack_Coroutine));
        StartCoroutine(nameof(Skill_Belber_202000_Attack_Coroutine));
    }

    IEnumerator Skill_Belber_202000_Attack_Coroutine()
    {
        cUnit.SetCastingSkill(true);

        yield return waitDefault;

        cUnit.ChangeMP(0);
        cUnit.SetCanNotGetMp(true);

        cUnit.PlayAnimation(UnitAniActionParam.AtkSkill, true);
        Transform _tfEffect = cUnit.SpawnAtkSkillEffect(0, sAtkSkillComponent.listValue[1].value[0]).transform;
        _tfEffect.SetParent(cUnit.body);
        _tfEffect.localPosition = Vector3.zero;

        cUnit.GetBuff(BuffType.MoveSpeed, cUnit, -sAtkSkillComponent.listValue[0].value[0], sAtkSkillComponent.listValue[1].value[0]);
        cUnit.GetBuff(BuffType.ADDef, cUnit, sAtkSkillComponent.listValue[2].value[cUnit.unitLevel - 1], sAtkSkillComponent.listValue[1].value[0]);
        cUnit.GetBuff(BuffType.APDef, cUnit, sAtkSkillComponent.listValue[2].value[cUnit.unitLevel - 1], sAtkSkillComponent.listValue[1].value[0]);

        GetNearUnit().AddHP(cUnit, -10, DamageType.AP);

        yield return new WaitForSeconds(sAtkSkillComponent.listValue[1].value[0]);

        cUnit.SetCanNotGetMp(false);

        cUnit.PlayAnimation(UnitAniActionParam.StopSkill);

        yield return waitDefault;

        cUnit.SetCastingSkill(false);
    }
    #endregion

    #region Cabino
    private void Skill_Cabino_201001_Defense()
    {
        StopCoroutine(nameof(Skill_Cabino_201001_Defense_Coroutine));
        StartCoroutine(nameof(Skill_Cabino_201001_Defense_Coroutine));
    }

    IEnumerator Skill_Cabino_201001_Defense_Coroutine()
    {
        Unit _cTarget = null;

        while (_cTarget == null)
        {
            if (_cTarget != null)
                break;

            _cTarget = GetNearUnit(2f);
            yield return null;
        }

        cUnit.SetCastingSkill(true);

        yield return new WaitForSeconds(0.8f);

        cUnit.ChangeMP(0);

        cUnit.PlayAnimation(UnitAniActionParam.DefSkill);
        Vector3 _v3TargetPos = new Vector3(_cTarget.body.position.x, cUnit.transform.position.y, _cTarget.body.position.z);
        cUnit.body.DOLookAt(_v3TargetPos, 0.1f);

        yield return new WaitForSeconds(0.8f);

        _cTarget.AddHP(cUnit, cUnit.CalcDamage(DamageType.AP, cUnit, (int)skillComponent_Def.listFinalValue[0]), DamageType.AP);
        _cTarget.GetBuff(BuffType.Stern, cUnit, 0, sDefSkillComponent.listValue[1].value[cUnit.unitLevel - 1]);

        yield return new WaitForSeconds(0.5f);

        cUnit.SetCastingSkill(false);
    }
    #endregion

    #region Dinko
    private void Skill_Dinko_201002_Defense()
    {
        StopCoroutine(nameof(Skill_Dinko_201002_Defense_Coroutine));
        StartCoroutine(nameof(Skill_Dinko_201002_Defense_Coroutine));
    }

    IEnumerator Skill_Dinko_201002_Defense_Coroutine()
    {
        yield return waitDefault;

        cUnit.SetCastingSkill(true);

        WaitForSeconds _cNewWait = new WaitForSeconds(sDefSkillComponent.listValue[2].value[0]);

        cUnit.PlayAnimation(UnitAniActionParam.DefSkill, true);

        yield return new WaitForSeconds(0.75f);

        cUnit.ChangeMP(0);

        listHitBox[0].hitBoxEnterChain -= DinkoDamage;
        listHitBox[0].hitBoxEnterChain += DinkoDamage;

        GameObject _goEffect = cUnit.SpawnDefSkillEffect(0, 3.5f);
        _goEffect.transform.position = cUnit.body.position + cUnit.body.forward * 0.5f + Vector3.up * 0.5f;
        _goEffect.transform.rotation = cUnit.body.rotation;

        for (int i = 0; i < sDefSkillComponent.listValue[1].value[0] / sDefSkillComponent.listValue[2].value[0]; i++)
        {
            listHitBox[0].gameObject.SetActive(true);

            yield return _cNewWait;

            listHitBox[0].gameObject.SetActive(false);
        }

        cUnit.PlayAnimation(UnitAniActionParam.StopSkill, true);

        yield return new WaitForSeconds(0.5f);

        cUnit.SetCastingSkill(false);

        yield return new WaitForSeconds(3f);
    }

    private void DinkoDamage(Unit _cTarget, out BuffValue? _sBuffValue)
    {
        if (_cTarget.attackUnit)
            _cTarget.AddHP(cUnit, cUnit.CalcDamage(DamageType.AP, cUnit, (int)skillComponent_Def.listFinalValue[0]), DamageType.AP, true);

        _sBuffValue = null;
    }
#endregion

    #region Azzang
    private void Skill_Azzang_201003_Defense()
    {
        StopCoroutine(nameof(Skill_Azzang_201003_Defense_Coroutine));
        StartCoroutine(nameof(Skill_Azzang_201003_Defense_Coroutine));
    }

    IEnumerator Skill_Azzang_201003_Defense_Coroutine()
    {
        cUnit.SetCastingSkill(true);

        WaitForSeconds _cNewWait = new WaitForSeconds(sDefSkillComponent.listValue[3].value[0]);

        yield return new WaitForSeconds(0.8f);

        cUnit.ChangeMP(0);

        cUnit.body.DORotate(NormalizeRotation(), 0.1f);

        yield return waitDefault;

        cUnit.PlayAnimation(UnitAniActionParam.DefSkill);

        yield return new WaitForSeconds(0.5f);

        cUnit.SetCastingSkill(false);

        GameObject _goEffect = cUnit.SpawnDefSkillEffect(0, 10f);
        _goEffect.transform.position = cUnit.body.position + cUnit.body.transform.forward * 1f + Vector3.down * 0.09f;

        listHitBox[0].hitBoxEnterChain -= AzzangDamage;
        listHitBox[0].hitBoxEnterChain += AzzangDamage;
        listHitBox[1].hitBoxEnterChain -= AzzangSlow_Enter;
        listHitBox[1].hitBoxEnterChain += AzzangSlow_Enter;
        listHitBox[1].hitBoxExitChain -= AzzangSlow_Exit;
        listHitBox[1].hitBoxExitChain += AzzangSlow_Exit;

        listHitBox[0].transform.position = cUnit.body.position + cUnit.body.transform.forward * 1f + Vector3.up * 0.25f;
        listHitBox[1].transform.position = cUnit.body.position + cUnit.body.transform.forward * 1f + Vector3.up * 0.25f;
        listHitBox[0].gameObject.SetActive(true);
        listHitBox[1].gameObject.SetActive(true);

        for (int i = 0; i < sDefSkillComponent.listValue[2].value[0] / sDefSkillComponent.listValue[3].value[0]; i++)
        {
            listHitBox[0].gameObject.SetActive(true);

            yield return _cNewWait;

            listHitBox[0].gameObject.SetActive(false);
        }

        yield return null;

        listHitBox[0].gameObject.SetActive(false);
        listHitBox[1].gameObject.SetActive(false);

        yield return new WaitForSeconds(1.2f);

        cUnit.SetCastingSkill(false);
    }

    private void AzzangDamage(Unit _cTarget, out BuffValue? _sBuffValue)
    {
        if (_cTarget.attackUnit)
            _cTarget.AddHP(cUnit, _cTarget.CalcDamage(DamageType.AP, cUnit, (int)sDefSkillComponent.listFinalValue[0]), DamageType.AP, true);

        _sBuffValue = null;
    }

    private void AzzangSlow_Enter(Unit _cTarget, out BuffValue _sBuffValue)
    {
        if (_cTarget.attackUnit)
            _sBuffValue = _cTarget.GetBuff(BuffType.MoveSpeed, cUnit, -sDefSkillComponent.listValue[1].value[cUnit.unitLevel - 1], 0, true, false, "201003");
        else
            _sBuffValue = null;
    }

    private void AzzangSlow_Exit(Unit _cTarget, BuffValue _sBuffValue)
    {
        if (_cTarget.attackUnit)
            _cTarget.ExitBuff(_sBuffValue.strBuffCode.Substring(_sBuffValue.strBuffCode.Length - 6));
    }
    #endregion Moya

    #region Moya
    private void Skill_Moya_201004_Defense()
    {
        StopCoroutine(Skill_Moya_201004_Defense_Coroutine());
        StartCoroutine(Skill_Moya_201004_Defense_Coroutine());
    }

    IEnumerator Skill_Moya_201004_Defense_Coroutine()
    {
        yield return new WaitForSeconds(0.2f);

        cUnit.ChangeMP(0);
        cUnit.SetCastingSkill(true);
        cUnit.PlayAnimation(UnitAniActionParam.DefSkill);

        yield return new WaitForSeconds(0.5f);

        GameObject _goEffect = cUnit.SpawnDefSkillEffect(0, sDefSkillComponent.listValue[1].value[0]);
        _goEffect.transform.position = cUnit.body.position + Vector3.up * 0.2f;
        _goEffect.transform.SetParent(cUnit.body);

        cUnit.SetCastingSkill(false);

        cUnit.GetBuff(BuffType.Attack, cUnit, sDefSkillComponent.listFinalValue[0], sDefSkillComponent.listValue[1].value[0]);
    }
    #endregion

    #region FingerElfe
    private void Skill_FingerElfe_201005_Defense()
    {
        StopCoroutine(Skill_FingerElfe_201005_Defense_Coroutine());
        StartCoroutine(Skill_FingerElfe_201005_Defense_Coroutine());
    }

    IEnumerator Skill_FingerElfe_201005_Defense_Coroutine()
    {
        if (!Tools.GetRandomForPercentResult(sDefSkillComponent.listFinalValue[0]))
            yield break;

        Unit _cTarget = cUnit.targetUnit;

        yield return new WaitForSeconds(0.25f * (1f / cUnit.unitStat.fCrtAtkSpeed));

        cUnit.AttackUnit(_cTarget, (int)sDefSkillComponent.listFinalValue[1]);
    }
    #endregion

    #region Wacool
    private void Skill_Wacool_201006_Defense()
    {
        StopCoroutine(nameof(Skill_Wacool_201006_Defense_Coroutine));
        StartCoroutine(nameof(Skill_Wacool_201006_Defense_Coroutine));
    }

    IEnumerator Skill_Wacool_201006_Defense_Coroutine()
    {
        yield return new WaitForSeconds(0.25f);

        cUnit.SetCastingSkill(true);

        cUnit.ChangeMP(0);

        Vector3 _v3TargetPos = GetRandomRout();

        cUnit.PlayAnimation(UnitAniActionParam.DefSkill);
        cUnit.body.DOLookAt(_v3TargetPos, 0.1f);

        yield return new WaitForSeconds(1f);

        GameObject _goSkillEffect = cUnit.SpawnDefSkillEffect(0, 0);
        _goSkillEffect.gameObject.transform.position = cUnit.body.position + Vector3.up * 0.2f;
        _goSkillEffect.gameObject.transform.rotation = cUnit.body.rotation;
        _goSkillEffect.gameObject.transform.localScale = Vector3.one;

        HitBox _cHitBox = _goSkillEffect.GetComponent<HitBox>();
        _cHitBox.hitBoxEnterChain -= WacoolTrigger_Enter;
        _cHitBox.hitBoxEnterChain += WacoolTrigger_Enter;
        _cHitBox.OnOffCollider(false);

        float _fJumpTime = 0.15f * Vector3.Distance(cUnit.body.position, _v3TargetPos);
        _goSkillEffect.transform.DOJump(_v3TargetPos, 2f, 1, _fJumpTime);

        yield return new WaitForSeconds(_fJumpTime);

        _goSkillEffect.transform.transform.DOScale(Vector3.one * 1.5f, 0.1f);

        cUnit.SetCastingSkill(false);

        yield return new WaitForSeconds(0.2f);

        _cHitBox.OnOffCollider(true);

        _cHitBox.AutoReturn(100f);

        yield return new WaitUntil(() => _cHitBox.entered);

        _cHitBox.OnOffCollider(false);

        yield return new WaitForSeconds(0.25f);

        _goSkillEffect.GetComponent<PoolableObject>().ReturnObj();
    }

    private void WacoolTrigger_Enter(Unit _cTarget, out BuffValue? _sBuffValue)
    {
        StartCoroutine(WacoolTrigger_Enter_Coroutine(_cTarget));

        _sBuffValue = null;
    }

    IEnumerator WacoolTrigger_Enter_Coroutine(Unit _cTarget)
    {
        yield return new WaitForSeconds(0.5f);

        GameObject _goEffect = cUnit.SpawnDefSkillEffect(1, 1f);
        _goEffect.transform.position = _cTarget.body.position + Vector3.up * 0.5f;

        if (_cTarget.attackUnit)
        {
            Collider[] _cTargetHeros = Physics.OverlapSphere(_cTarget.bodyHolder.position + Vector3.up * 0.5f, sDefSkillComponent.listValue[3].value[0] * 0.5f, cLayerMask.value);
            List<Unit> _listTarget = new List<Unit>();

            for (int i = 0; i < _cTargetHeros.Length; i++)
                _listTarget.Add(_cTargetHeros[i].GetComponent<Hero>());

            WacoolExplosion(_listTarget);
        }
    }

    private void WacoolExplosion(List<Unit> _listTarget)
    {
        foreach (var item in _listTarget)
        {
            if (item.attackUnit)
            {
                item.AddHP(cUnit, item.CalcDamage(DamageType.AP, cUnit, (int)skillComponent_Def.listFinalValue[0]), DamageType.AP);
                item.GetBuff(BuffType.MoveSpeed, cUnit, -sDefSkillComponent.listValue[1].value[cUnit.unitLevel - 1], skillComponent_Def.listValue[2].value[0]);
            }
        }
    }

    #endregion

    #region Soskereu
    private void Skill_Soskereu_201007_Defense()
    {
        StopCoroutine(nameof(Skill_Soskereu_201007_Defense_Coroutine));
        StartCoroutine(nameof(Skill_Soskereu_201007_Defense_Coroutine));
    }

    IEnumerator Skill_Soskereu_201007_Defense_Coroutine()
    {
        yield return waitDefault;

        cUnit.SetCastingSkill(true);
        cUnit.ChangeMP(0);

        Unit _cTarget = GetNearUnit();

        cUnit.body.DOLookAt(new Vector3(_cTarget.body.position.x, cUnit.transform.position.y, _cTarget.body.position.z), 0.1f);

        yield return waitDefault;

        cUnit.PlayAnimation(UnitAniActionParam.DefSkill);

        yield return new WaitForSeconds(0.6f);

        TrackingProjectile _cProjectile = cUnit.SpawnDefSkillEffect(0, 0).GetComponent<TrackingProjectile>();
        _cProjectile.transform.position = cUnit.body.position + Vector3.up * 0.5f;
        float _fPrjSpeed = 5f;
        if (_cTarget == null)
            _cProjectile.GetComponent<PoolableObject>().ReturnObj();
        else
            _cProjectile.SetTarget(_cTarget.body, _fPrjSpeed);

        cUnit.SetCastingSkill(false);

        yield return new WaitForSeconds(Vector3.Distance(cUnit.body.position, _cTarget.body.position) / _fPrjSpeed);

        _cTarget.AddHP(cUnit, _cTarget.CalcDamage(DamageType.AP, cUnit, (int)sDefSkillComponent.listFinalValue[0]), DamageType.AP);
    }
    #endregion

    #region Keurot
    private void Skill_Keurot_201008_Defense()
    {
        StopCoroutine(nameof(Skill_Keurot_201008_Defense_Coroutine));
        StartCoroutine(nameof(Skill_Keurot_201008_Defense_Coroutine));
    }

    IEnumerator Skill_Keurot_201008_Defense_Coroutine()
    {
        cUnit.SetCastingSkill(true);

        yield return new WaitForSeconds(0.25f);

        cUnit.ChangeMP(0);

        yield return new WaitForSeconds(0.5f);

        cUnit.PlayAnimation(UnitAniActionParam.DefSkill);

        yield return new WaitForSeconds(0.3f);

        cUnit.SetCastingSkill(false);

        Collider[] _cTargetHeros = Physics.OverlapSphere(cUnit.bodyHolder.position + Vector3.up * 0.5f, sDefSkillComponent.listValue[1].value[0] * 0.5f, cLayerMask.value);
        List<Hero> _listTargetHero = new List<Hero>();

        for (int i = 0; i < _cTargetHeros.Length; i++)
            _listTargetHero.Add(_cTargetHeros[i].GetComponent<Hero>());

        yield return new WaitForSeconds(0.2f);

        foreach (var item in _listTargetHero)
        {
            if (item.attackUnit)
            {
                item.AddHP(cUnit, item.CalcDamage(DamageType.AP, cUnit, (int)sDefSkillComponent.listFinalValue[0]), DamageType.AP);
                item.GetBuff(BuffType.ADDef, cUnit, -sDefSkillComponent.listValue[3].value[cUnit.unitLevel - 1], sDefSkillComponent.listValue[2].value[0]);
            }
        }
    }
    #endregion

    #region Witch
    private void Skill_Witch_201009_Defense()
    {
        StopCoroutine(nameof(Skill_Witch_201009_Defense_Coroutine));
        StartCoroutine(nameof(Skill_Witch_201009_Defense_Coroutine));
    }

    IEnumerator Skill_Witch_201009_Defense_Coroutine()
    {
        cUnit.SetCastingSkill(true);

        Unit _cTarget = GetNearUnit();

        yield return new WaitForSeconds(0.2f);

        cUnit.body.DOLookAt(new Vector3(_cTarget.body.position.x, cUnit.transform.position.y, _cTarget.body.position.z), 0.1f);

        yield return waitDefault;

        cUnit.ChangeMP(0);

        cUnit.PlayAnimation(UnitAniActionParam.DefSkill);

        yield return new WaitForSeconds(0.5f);

        TrackingProjectile _cProjectile = cUnit.SpawnDefSkillEffect(0, 0).GetComponent<TrackingProjectile>();
        _cProjectile.transform.position = cUnit.body.position + Vector3.up * 0.5f;
        float _fPrjSpeed = 7f;
        if (_cTarget == null)
            _cProjectile.GetComponent<PoolableObject>().ReturnObj();
        else
            _cProjectile.SetTarget(_cTarget.body, _fPrjSpeed);

        cUnit.SetCastingSkill(false);

        yield return new WaitForSeconds(Vector3.Distance(cUnit.body.position, _cTarget.body.position) / _fPrjSpeed);

        _cTarget.AddHP(cUnit, _cTarget.CalcDamage(DamageType.AP, cUnit, (int)sDefSkillComponent.listFinalValue[0]), DamageType.AP);

        yield return new WaitForSeconds(sDefSkillComponent.listValue[3].value[0]);

        GameObject _goSkillEffect_Exposion = cUnit.SpawnDefSkillEffect(1, 0.5f);
        _goSkillEffect_Exposion.transform.position = _cTarget.body.position + Vector3.up * 0.5f;

        Collider[] _cWeeklyHeros = Physics.OverlapSphere(_cTarget.bodyHolder.position + Vector3.up * 0.5f, sDefSkillComponent.listValue[2].value[0] * 0.5f, cLayerMask.value);

        foreach (var item in _cWeeklyHeros)
        {
            Hero _cHero = item.GetComponent<Hero>();
            if (_cHero.attackUnit)
                _cHero.AddHP(cUnit, _cHero.CalcDamage(DamageType.AP, cUnit, (int)sDefSkillComponent.listFinalValue[1]), DamageType.AP);
        }
    }
    #endregion

    #region Assasin
    private void Skill_Assassin_201010_Defense()
    {
        StopCoroutine(nameof(Skill_Assassin_201010_Defense_Coroutine));
        StartCoroutine(nameof(Skill_Assassin_201010_Defense_Coroutine));
    }

    IEnumerator Skill_Assassin_201010_Defense_Coroutine()
    {
        Unit _cTarget = null;

        while (_cTarget == null)
        {
            _cTarget = GetNearUnit(sDefSkillComponent.listValue[2].value[0]);
            yield return null;
        }

        float _fTurnTime = 0.1f;
        WaitForSeconds _cNewWait = new WaitForSeconds(_fTurnTime);

        yield return _cNewWait;

        cUnit.body.DOLookAt(_cTarget.transform.position, _fTurnTime);

        yield return new WaitForSeconds(_fTurnTime);

        cUnit.SetCastingSkill(true);
        cUnit.ChangeMP(0);

        cUnit.PlayAnimation(UnitAniActionParam.DefSkill);

        yield return new WaitForSeconds(0.5f);

        TrackingProjectile _cProjectile = cUnit.SpawnDefSkillEffect(0, 0).GetComponent<TrackingProjectile>();
        _cProjectile.transform.position = cUnit.body.position + Vector3.up * 0.5f;
        _cProjectile.transform.rotation = cUnit.body.transform.rotation;
        float _fPrjSpeed = 12f;
        _cProjectile.SetTarget(_cTarget.transform, _fPrjSpeed);

        yield return new WaitForSeconds(Vector3.Distance(cUnit.body.position, _cTarget.body.position) / _fPrjSpeed);

        _cTarget.AddHP(cUnit, _cTarget.CalcDamage(DamageType.AD, cUnit, (int)sDefSkillComponent.listFinalValue[0]), DamageType.AD);

        yield return _cNewWait;

        cUnit.SetCastingSkill(false);
    }

    #endregion

    #region Mocomoco
    private void Skill_Mocomoco_201011_Defense()
    {
        StopCoroutine(nameof(Skill_Mocomoco_201011_Defense_Coroutine));
        StartCoroutine(nameof(Skill_Mocomoco_201011_Defense_Coroutine));
    }

    IEnumerator Skill_Mocomoco_201011_Defense_Coroutine()
    {
        yield return new WaitForSeconds(0.3f);

        cUnit.SetCastingSkill(true);
        cUnit.ChangeMP(0);
        cUnit.PlayAnimation(UnitAniActionParam.DefSkill);
        listHitBox[0].hitBoxEnterChain -= MocomocoDamage;
        listHitBox[0].hitBoxEnterChain += MocomocoDamage;
        listHitBox[0].hitBoxEnterChain -= MocomocoSilentEnter;
        listHitBox[0].hitBoxEnterChain += MocomocoSilentEnter;

        yield return new WaitForSeconds(0.2f);

        GameObject _goEffect = cUnit.SpawnDefSkillEffect(0, 0.5f);
        _goEffect.transform.position = _goEffect.transform.position = cUnit.body.position + cUnit.body.forward * 0.5f + Vector3.up * 0.5f;
        _goEffect.transform.rotation = cUnit.body.rotation;

        listHitBox[0].gameObject.SetActive(true);

        yield return null;

        listHitBox[0].gameObject.SetActive(false);

        yield return new WaitForSeconds(0.2f);

        cUnit.SetCastingSkill(false);
    }

    private void MocomocoDamage(Unit _cTarget, out BuffValue _sBuffValue)
    {
        if (_cTarget.attackUnit)
            _cTarget.AddHP(cUnit, _cTarget.CalcDamage(DamageType.AP, cUnit, (int)skillComponent_Def.listFinalValue[0]), DamageType.AP);

        _sBuffValue = null;
    }

    private void MocomocoSilentEnter(Unit _cTarget, out BuffValue _sBuffValue)
    {
        if (_cTarget.attackUnit)
            _cTarget.GetBuff(BuffType.Silent, cUnit, 0, sDefSkillComponent.listValue[1].value[cUnit.unitLevel - 1]);

        _sBuffValue = null;
    }
    #endregion

    #region Godongi
    private void Skill_Godongi_201012_Defense()
    {
        StartCoroutine(nameof(Skill_Godongi_201012_Defense_Coroutine));
    }

    IEnumerator Skill_Godongi_201012_Defense_Coroutine()
    {
        cUnit.SetCastingSkill(true);

        yield return new WaitForSeconds(Time.deltaTime * 3f);

        cUnit.SetCastingSkill(false);

        cUnit.ChangeMP(0);

        yield return new WaitUntil(() => cUnit.hit);

        Debug.LogError("GodongiSkill");

        cUnit.PlayAnimation(UnitAniActionParam.DefSkill);

        cUnit.crtHitUnit.AddHP(cUnit, cUnit.crtHitUnit.CalcDamage(DamageType.AD, cUnit, (int)skillComponent_Def.listFinalValue[0]), DamageType.AD, true);
        cUnit.crtHitUnit.GetBuff(BuffType.Airborne, cUnit, 0, skillComponent_Def.listValue[1].value[cUnit.unitLevel - 1]);
    }
    #endregion

    #region Peullamon
    private void Skill_Peullamon_201013_Defense()
    {
        StopCoroutine(nameof(Skill_Peullamon_201013_Defense_Coroutine));
        StartCoroutine(nameof(Skill_Peullamon_201013_Defense_Coroutine));
    }

    IEnumerator Skill_Peullamon_201013_Defense_Coroutine()
    {
        yield return waitDefault;

        cUnit.SetCastingSkill(true);
        cUnit.ChangeMP(0);

        List<Unit> _listTarget = GetNearUnits((int)sDefSkillComponent.listValue[1].value[cUnit.unitLevel - 1]);

        cUnit.body.DOLookAt(new Vector3(_listTarget[0].body.position.x, cUnit.transform.position.y, _listTarget[0].body.position.z), 0.1f);

        yield return waitDefault;

        cUnit.PlayAnimation(UnitAniActionParam.DefSkill);

        yield return new WaitForSeconds(0.75f);

        List<TrackingProjectile> _listProjectiel = new List<TrackingProjectile>();

        float _fPrjSpeed = 10f;

        for (int i = 0; i < _listTarget.Count; i++)
        {
            _listProjectiel.Add(cUnit.SpawnDefSkillEffect(0, 0).GetComponent<TrackingProjectile>());
            _listProjectiel[i].transform.position = cUnit.body.position + Vector3.up * 0.5f;
            if (_listTarget[i] == null)
                _listProjectiel[i].GetComponent<PoolableObject>().ReturnObj();
            else
                _listProjectiel[i].SetTarget(_listTarget[i].body, _fPrjSpeed);
        }

        cUnit.SetCastingSkill(false);

        for (int i = 0; i < _listProjectiel.Count; i++)
            StartCoroutine(PeullamonShoot(_listTarget[i], _fPrjSpeed));
    }

    IEnumerator PeullamonShoot(Unit _cTarget, float _fPrfSpd)
    {
        yield return new WaitForSeconds(Vector3.Distance(cUnit.body.position, _cTarget.body.position) / _fPrfSpd);

        _cTarget.AddHP(cUnit, _cTarget.CalcDamage(DamageType.AP, cUnit, (int)sDefSkillComponent.listFinalValue[0]), DamageType.AP);
        _cTarget.GetBuff(BuffType.APDef, cUnit, _cTarget.unitStat.iCrtAPDef * -sDefSkillComponent.listValue[4].value[0] * 0.01f, sDefSkillComponent.listValue[3].value[0]);
    }

    #endregion

    #region Hunter
    private void Skill_Hunter_201014_Defense()
    {
        StopCoroutine(Skill_Hunter_201014_DefenseCoroutine());
        StartCoroutine(Skill_Hunter_201014_DefenseCoroutine());
    }

    IEnumerator Skill_Hunter_201014_DefenseCoroutine()
    {
        cUnit.SetCastingSkill(true);

        yield return waitDefault;

        cUnit.ChangeMP(0);

        cUnit.PlayAnimation(UnitAniActionParam.DefSkill);

        yield return new WaitForSeconds(0.5f);

        GameObject _goEffect = cUnit.SpawnDefSkillEffect(0, 0.5f);
        _goEffect.transform.position = cUnit.body.position + Vector3.up * 0.5f;
        _goEffect.transform.rotation = cUnit.body.rotation;

        for (int i = 0; i < _goEffect.transform.childCount; i++)
            _goEffect.transform.GetChild(i).gameObject.SetActive(false);

        _goEffect.transform.GetChild(cUnit.unitLevel - 1).gameObject.SetActive(true);

        listHitBox = _goEffect.transform.GetChild(cUnit.unitLevel - 1).GetComponentsInChildren<HitBox>(true).ToList();

        foreach (var item in listHitBox)
        {
            item.transform.localPosition = Vector3.forward * -1f;
            item.transform.DOLocalMoveZ(4f, 0.5f);

            item.hitBoxEnterChain -= HunterDamage;
            item.hitBoxEnterChain += HunterDamage;
        }
        
        yield return waitDefault;

        cUnit.SetCastingSkill(false);
    }

    private void HunterDamage(Unit _cTarget, out BuffValue? _sBuffValue)
    {
        if (_cTarget.attackUnit)
            _cTarget.AddHP(cUnit, _cTarget.CalcDamage(DamageType.AD, cUnit, (int)skillComponent_Def.listFinalValue[0]), DamageType.AD);

        _sBuffValue = null;
    }
    #endregion

    #region Dragoon
    private void Skill_Dragoon_201015_Defense()
    {
        StopCoroutine(Skill_Dragoon_201015_DefenseCoroutine());
        StartCoroutine(Skill_Dragoon_201015_DefenseCoroutine());
    }

    IEnumerator Skill_Dragoon_201015_DefenseCoroutine()
    {
        yield return waitDefault;

        cUnit.ChangeMP(0);
        cUnit.SetCastingSkill(true);

        float _flyingDuration = 0.75f;

        cUnit.body.transform.DOLocalMoveY(1.5f, _flyingDuration);

        yield return new WaitForSeconds(_flyingDuration);

        cUnit.SetCastingSkill(false);

        float _fDuration = sDefSkillComponent.listValue[0].value[cUnit.unitLevel - 1];

        cUnit.GetBuff(BuffType.Unspecified, cUnit, 0, _fDuration);
        cUnit.GetBuff(BuffType.SendDamage, cUnit, sDefSkillComponent.listFinalValue[0], _fDuration);
        cUnit.SetCanNotGetMp(true);

        yield return new WaitForSeconds(_fDuration);

        cUnit.SetCastingSkill(true);

        cUnit.body.DORotate(new Vector3(0, cUnit.body.eulerAngles.y, cUnit.body.eulerAngles.z), 0.1f);

        cUnit.SetCanNotGetMp(false);

        yield return waitDefault;

        cUnit.body.DOLocalMoveY(0, _flyingDuration);

        yield return new WaitForSeconds(_flyingDuration);

        cUnit.SetCastingSkill(false);
    }
    #endregion

    #region Bellial
    private void Skill_Bellial_201016_Defense()
    {
        StopCoroutine(Skill_Bellial_201016_Defense_Coroutine());
        StartCoroutine(Skill_Bellial_201016_Defense_Coroutine());
    }

    IEnumerator Skill_Bellial_201016_Defense_Coroutine()
    {
        yield return waitDefault;

        cUnit.body.DOLookAt(GetNearUnit(sDefSkillComponent.listValue[1].value[0]).bodyHolder.position, 0.1f);

        yield return waitDefault;

        cUnit.SetCastingSkill(true);

        cUnit.ChangeMP(0);
        cUnit.PlayAnimation(UnitAniActionParam.DefSkill);

        List<Unit> _listTargetUnit = new List<Unit>();

        yield return new WaitForSeconds(1.4f);

        while (_listTargetUnit.Count <= 0)
        {
            _listTargetUnit = GetNearUnits((int)sDefSkillComponent.listValue[2].value[cUnit.unitLevel - 1], sDefSkillComponent.listValue[1].value[0]);
            yield return null;
        }

        foreach (var item in _listTargetUnit)
        {
            Transform _tfEffect = cUnit.SpawnDefSkillEffect(cUnit.unitLevel - 1, sDefSkillComponent.listValue[3].value[cUnit.unitLevel - 1]).transform;
            _tfEffect.position = item.body.position;

            item.AddHP(cUnit, item.CalcDamage(DamageType.AP, cUnit, (int)sDefSkillComponent.listFinalValue[0]), DamageType.AP);
            item.GetBuff(BuffType.Stiff, cUnit, 0, sDefSkillComponent.listValue[3].value[cUnit.unitLevel - 1]);
        }

        yield return waitDefault;

        cUnit.SetCastingSkill(false);
    }
    #endregion

    #region Okin
    private void Skill_Okin_201017_Defense()
    {
        StopCoroutine(Skill_Okin_201017_Defense_Coroutine());
        StartCoroutine(Skill_Okin_201017_Defense_Coroutine());
    }

    IEnumerator Skill_Okin_201017_Defense_Coroutine()
    {
        string _strCode = "201017D";
        yield return null;
        cUnit.AddHP(cUnit, cUnit.CalcDamage(DamageType.True, cUnit, (int)sDefSkillComponent.listValue[0].value[cUnit.unitLevel - 1]), DamageType.True);
        if (!cUnit.buffList.Any(x => x.strBuffCode.Contains(_strCode)))
            cUnit.GetBuff(BuffType.AtkSpeed, cUnit, sDefSkillComponent.listValue[1].value[cUnit.unitLevel - 1], 0, true, true, _strCode);
        else
        {
            cUnit.buffList.Find(x => x.strBuffCode.Contains(_strCode)).AddValue(sDefSkillComponent.listValue[1].value[cUnit.unitLevel - 1]);
            cUnit.UpdateBuffValue();
        }
    }
    #endregion

    #region Guroguro
    private void Skill_Guroguro_201018_Defense()
   {
        StopCoroutine(Skill_Guroguro_201018_Defense_Coroutine());
        StartCoroutine(Skill_Guroguro_201018_Defense_Coroutine());
    }

    IEnumerator Skill_Guroguro_201018_Defense_Coroutine()
    {
        cUnit.SetCastingSkill(true);
        cUnit.SetCanNotGetMp(true);

        yield return waitDefault;

        cUnit.ChangeMP(0);

        BuffValue _cBuff = cUnit.GetBuff(BuffType.Shield, cUnit, sDefSkillComponent.listFinalValue[0], sDefSkillComponent.listValue[0].value[0]);

        yield return waitDefault;

        cUnit.SetCastingSkill(false);

        yield return new WaitUntil(() => !cUnit.buffList.Contains(_cBuff));

        Collider[] _cTargetHeros = Physics.OverlapSphere(cUnit.bodyHolder.position + Vector3.up * 0.5f, sDefSkillComponent.listValue[2].value[0] * 0.5f, cLayerMask.value);
        List<Hero> _listTargetHero = new List<Hero>();

        for (int i = 0; i < _cTargetHeros.Length; i++)
            _listTargetHero.Add(_cTargetHeros[i].GetComponent<Hero>());

        yield return waitDefault;

        foreach (var item in _listTargetHero)
        {
            if (item.attackUnit)
                item.AddHP(cUnit, item.CalcDamage(DamageType.AP, cUnit, (int)sDefSkillComponent.listFinalValue[0]), DamageType.AP);
        }

        cUnit.SetCanNotGetMp(false);
    }

    #endregion

    #region Predictor
    private void Skill_Predictor_201019_Defense()
    {
        StopCoroutine(Skill_Predictor_201019_Defense_Coroutine());
        StartCoroutine(Skill_Predictor_201019_Defense_Coroutine());
    }

    IEnumerator Skill_Predictor_201019_Defense_Coroutine()
    {
        Debug.LogError("Skill");

        cUnit.SetCastingSkill(true);

        yield return waitDefault;

        cUnit.ChangeMP(0);

        Unit _cTarget = GetNearUnit();

        cUnit.body.DOLookAt(new Vector3(_cTarget.body.position.x, cUnit.transform.position.y, _cTarget.body.position.z), 0.1f);

        yield return waitDefault;

        cUnit.PlayAnimation(UnitAniActionParam.DefSkill);

        yield return new WaitForSeconds(0.3f);

        TrackingProjectile _cProjectile = cUnit.SpawnDefSkillEffect(0, 0).GetComponent<TrackingProjectile>();
        _cProjectile.transform.position = cUnit.body.position + Vector3.up * 0.5f;
        float _fPrjSpeed = 5f;
        if (_cTarget == null)
            _cProjectile.GetComponent<PoolableObject>().ReturnObj();
        else
            _cProjectile.SetTarget(_cTarget.body, _fPrjSpeed);

        cUnit.SetCastingSkill(false);

        yield return new WaitForSeconds(Vector3.Distance(cUnit.body.position, _cTarget.body.position) / _fPrjSpeed);

        _cTarget.AddHP(cUnit, _cTarget.CalcDamage(DamageType.AP, cUnit, (int)sDefSkillComponent.listFinalValue[0]), DamageType.AP);

        StartCoroutine(Predictor_Cuse_Coroutine(_cTarget));
    }

    IEnumerator Predictor_Cuse_Coroutine(Unit _cTarget)
    {
        WaitForSeconds _delay = new WaitForSeconds(sDefSkillComponent.listValue[3].value[0]);

        GameObject _goEffect = cUnit.SpawnDefSkillEffect(1, sDefSkillComponent.listValue[1].value[cUnit.unitLevel - 1]);

        _goEffect.transform.SetParent(_cTarget.body);
        _goEffect.transform.localPosition = Vector3.up * 0.5f;

        for (int i = 0; i < sDefSkillComponent.listValue[1].value[cUnit.unitLevel - 1] / sDefSkillComponent.listValue[3].value[0]; i++)
        {
            Collider[] _cWeeklyHeros = Physics.OverlapSphere(_cTarget.bodyHolder.position + Vector3.up * 0.5f, sDefSkillComponent.listValue[2].value[0] * 0.5f, cLayerMask.value);

            foreach (var item in _cWeeklyHeros)
            {
                Hero _cHero = item.GetComponent<Hero>();
                if (_cHero.attackUnit)
                    _cHero.AddHP(cUnit, _cHero.CalcDamage(DamageType.AP, cUnit, (int)sDefSkillComponent.listFinalValue[1]), DamageType.AP);
            }

            yield return _delay;
        }
    }

    #endregion

    #region Babarian
    private void Skill_Babarian_201022_Defense()
    {
        StopCoroutine(Skill_Babarian_201022_Defense_Coroutine());
        StartCoroutine(Skill_Babarian_201022_Defense_Coroutine());
    }

    IEnumerator Skill_Babarian_201022_Defense_Coroutine()
    {
        yield return waitDefault;

        cUnit.SetCastingSkill(true);
        cUnit.ChangeMP(0);

        yield return new WaitForSeconds(0.2f);

        cUnit.PlayAnimation(UnitAniActionParam.DefSkill);

        yield return new WaitForSeconds(0.5f);

        yield return new WaitUntil(() => GetNearUnit(sDefSkillComponent.listValue[0].value[0]) != null);

        Unit _cTarget = GetNearUnit(sDefSkillComponent.listValue[0].value[0]);

        _cTarget.AddHP(cUnit, cUnit.CalcDamage(DamageType.AD, cUnit, (int)sDefSkillComponent.listFinalValue[cUnit.unitLevel - 1]), DamageType.AD);
        _cTarget.GetBuff(BuffType.ADDef, cUnit, _cTarget.unitStat.iCrtADDef * -sDefSkillComponent.listValue[4].value[0] * 0.01f, sDefSkillComponent.listValue[2].value[0]);

        yield return waitDefault;

        cUnit.SetCastingSkill(false);
    }
    #endregion

    #region Basilisk
    private void Skill_Basilisk_201020_Defense()
    {
        StopCoroutine(Skill_Basilisk_201020_Defense_Coroutine());
        StartCoroutine(Skill_Basilisk_201020_Defense_Coroutine());
    }

    IEnumerator Skill_Basilisk_201020_Defense_Coroutine()
    {
        yield return null;
    }

    private void Skill_Basilisk_202020_Attack()
    {
        StopCoroutine(Skill_Basilisk_202020_Attack_Coroutine());
        StartCoroutine(Skill_Basilisk_202020_Attack_Coroutine());
    }

    IEnumerator Skill_Basilisk_202020_Attack_Coroutine()
    {
        if (cUnit.buffList.Any(x => x.cCaster.unitID == 102006))
            yield break;

        cUnit.ChangeMP(0);
        cUnit.SetCanNotGetMp(true);

        cUnit.GetBuff(BuffType.Shield, cUnit, cUnit.unitStat.iMaxHp * sAtkSkillComponent.listFinalValue[0] * 0.01f, sAtkSkillComponent.listValue[0].value[0]);
        cUnit.GetBuff(BuffType.Taunt, cUnit, 0, sAtkSkillComponent.listValue[0].value[0]);

        yield return new WaitForSeconds(sAtkSkillComponent.listValue[0].value[0]);

        cUnit.SetCanNotGetMp(false);
    }
    #endregion

    #region Cablo
    private void Skill_Cablo_201021_Defense()
    {
        StopCoroutine(Skill_Cablo_201021_Defense_Coroutine());
        StartCoroutine(Skill_Cablo_201021_Defense_Coroutine());
    }

    public List<GameObject> cabloBlockList;
    public List<Unit> cabloBlockUnitList;

    IEnumerator Skill_Cablo_201021_Defense_Coroutine()
    {
        cUnit.SetCastingSkill(true);

        Unit _cTarget = GetNearUnit(cUnit.unitStat.fCrtAtkRange);

        yield return waitDefault;

        cUnit.body.DOLookAt(new Vector3(_cTarget.body.position.x, cUnit.transform.position.y, _cTarget.body.position.z), 0.1f);

        yield return waitDefault;

        cUnit.ChangeMP(0);

        cUnit.PlayAnimation(UnitAniActionParam.DefSkill, true);

        yield return new WaitForSeconds(1f);

        _cTarget.AddHP(cUnit, _cTarget.CalcDamage(DamageType.AP, cUnit, (int)sDefSkillComponent.listFinalValue[0]), DamageType.AD);
        _cTarget.GetBuff(BuffType.Stiff, cUnit, 0, sDefSkillComponent.listValue[1].value[cUnit.unitLevel - 1]);

        cabloBlockList = new List<GameObject>();
        cabloBlockUnitList = new List<Unit>();
        GameObject _effect = cUnit.SpawnDefSkillEffect(0, sDefSkillComponent.listValue[1].value[cUnit.unitLevel - 1]);
        _effect.GetComponent<CabloDefenceSkillComponent>().parentUnit = _cTarget;
        cabloBlockUnitList.Add(_cTarget);
        _effect.transform.position = _cTarget.body.position;

        yield return waitDefault;

        yield return new WaitUntil(() => !_effect.gameObject.activeSelf);

        for (int i = 0; i < cabloBlockList.Count; i++)
        {
            if (cabloBlockList[i] != _effect)
                Destroy(cabloBlockList[i]);
        }

        foreach (var item in cabloBlockUnitList)
            item.OnOffNavAgent(true);

        cUnit.PlayAnimation(UnitAniActionParam.StopSkill, true);

        yield return waitDefault ;

        cUnit.SetCastingSkill(false);
    }
    #endregion

    #region Icefairy
    private void Skill_Icefairy_201023_Defense()
    {
        StopCoroutine(Skill_Icefairy_201023_Defense_Coroutine());
        StartCoroutine(Skill_Icefairy_201023_Defense_Coroutine());
    }

    IEnumerator Skill_Icefairy_201023_Defense_Coroutine()
    {
        yield return waitDefault;

        cUnit.SetCastingSkill(true);

        cUnit.ChangeMP(0);

        yield return waitDefault;

        Unit _cTarget = GetNearUnit(3f);

        cUnit.body.DOLookAt(_cTarget.body.position, 0.1f);

        yield return new WaitForSeconds(0.6f);

        float _fTime = sDefSkillComponent.listValue[1].value[cUnit.unitLevel - 1];
        _cTarget.AddHP(cUnit, _cTarget.CalcDamage(DamageType.AP, cUnit, (int)sDefSkillComponent.listFinalValue[0]), DamageType.AP);
        _cTarget.GetBuff(BuffType.Stern, cUnit, 0, _fTime);
        _cTarget.ChangeMP(0);
        GameObject _goEffect = cUnit.SpawnDefSkillEffect(0, _fTime);
        _goEffect.transform.position = _cTarget.bodyHolder.position;

        listHitBox.Add(_goEffect.GetComponentInChildren<HitBox>());

        listHitBox[0].gameObject.SetActive(true);

        listHitBox[0].hitBoxEnterChain -= IcefairySlow_Enter;
        listHitBox[0].hitBoxEnterChain += IcefairySlow_Enter;
        listHitBox[0].hitBoxExitChain -= IcefairySlow_Exit;
        listHitBox[0].hitBoxExitChain += IcefairySlow_Exit;

        yield return waitDefault;

        cUnit.SetCastingSkill(false);

        yield return new WaitForSeconds(_fTime - 0.1f);

        listHitBox[0].gameObject.SetActive(false);
        listHitBox.Clear();
    }

    private void IcefairySlow_Enter(Unit _cTarget, out BuffValue _sBuffValue)
    {
        if (_cTarget.attackUnit)
            _sBuffValue = _cTarget.GetBuff(BuffType.MoveSpeed, cUnit, -sDefSkillComponent.listValue[2].value[0], 0, true, false, "201023");
        else
            _sBuffValue = null;
    }

    private void IcefairySlow_Exit(Unit _cTarget, BuffValue _sBuffValue)
    {
        if (_cTarget.attackUnit)
            _cTarget.ExitBuff(_sBuffValue.strBuffCode.Substring(_sBuffValue.strBuffCode.Length - 6));
    }
    #endregion

    #region Hogs
    private void Skill_Hogs_201026_Defense()
    {
        StopCoroutine(Skill_Hogs_201026_DefenseCoroutine());
        StartCoroutine(Skill_Hogs_201026_DefenseCoroutine());
    }

    IEnumerator Skill_Hogs_201026_DefenseCoroutine()
    {
        yield return waitDefault;

        cUnit.SetCastingSkill(true);
        cUnit.ChangeMP(0);

        yield return new WaitForSeconds(0.3f);

        cUnit.PlayAnimation(UnitAniActionParam.DefSkill);

        yield return waitDefault;

        listHitBox[0].gameObject.SetActive(true);

        listHitBox[0].hitBoxEnterChain -= HogsDamage;
        listHitBox[0].hitBoxEnterChain += HogsDamage;
        listHitBox[0].hitBoxEnterChain -= HogsStern;
        listHitBox[0].hitBoxEnterChain += HogsStern;

        yield return new WaitForFixedUpdate();

        listHitBox[0].gameObject.SetActive(false);

        yield return waitDefault;

        cUnit.SetCastingSkill(false);
    }

    private void HogsDamage(Unit _cTarget, out BuffValue? _sBuffValue)
    {
        if (_cTarget.attackUnit)
            _cTarget.AddHP(cUnit, _cTarget.CalcDamage(DamageType.AP, cUnit, (int)sDefSkillComponent.listFinalValue[0]), DamageType.AP);

        _sBuffValue = null;
    }

    private void HogsStern(Unit _cTarget, out BuffValue? _sBuffValue)
    {
        _sBuffValue = _cTarget.GetBuff(BuffType.Stern, cUnit, 0, sDefSkillComponent.listValue[1].value[cUnit.unitLevel - 1]);
    }

    #endregion

    #region Syaonil
    private void Skill_Syaonil_201028_Defense()
    {
        StopCoroutine(Skill_Syaonil_201028_Defense_Coroutine());
        StartCoroutine(Skill_Syaonil_201028_Defense_Coroutine());
    }

    IEnumerator Skill_Syaonil_201028_Defense_Coroutine()
    {
        cUnit.SetCastingSkill(true);

        yield return waitDefault;

        cUnit.ChangeMP(0);

        cUnit.SetCastingSkill(false);
        cUnit.SetCanNotGetMp(true);

        cUnit.SetAttackRange(sDefSkillComponent.listValue[1].value[0]);
        cUnit.GetBuff(BuffType.AtkSpeed, cUnit, sDefSkillComponent.listValue[2].value[0], sDefSkillComponent.listValue[0].value[0]);

        cUnit.skillChain_AtkEnd -= SyaonilHit;
        cUnit.skillChain_AtkEnd += SyaonilHit;

        yield return new WaitForSeconds(sDefSkillComponent.listValue[0].value[0]);

        cUnit.SetAttackRange(cUnit.unitStat.fOrgAtkRange);
        cUnit.SetCanNotGetMp(false);

        InGameManager.instance.cHeroInfoPanel.UpdateStatus();

        cUnit.skillChain_AtkEnd -= SyaonilHit;
    }

    private void SyaonilHit()
    {
        Transform _skillEffect = cUnit.SpawnDefSkillEffect(0, 1f).transform;
        _skillEffect.position = cUnit.crtHitUnit.body.position;

        _skillEffect.GetChild(0).localScale = Vector3.one * (0.4f * sDefSkillComponent.listValue[3].value[0]);

        Collider[] _cTargetHeros = Physics.OverlapSphere(cUnit.targetUnit.bodyHolder.position + Vector3.up * 0.25f, sDefSkillComponent.listValue[3].value[0] * 0.5f, cLayerMask.value);
        List<Hero> _listTargetHero = new List<Hero>();

        for (int i = 0; i < _cTargetHeros.Length; i++)
            _listTargetHero.Add(_cTargetHeros[i].GetComponent<Hero>());

        foreach (var item in _listTargetHero)
        {
            if (item.attackUnit)
                item.AddHP(cUnit, item.CalcDamage(DamageType.AP, cUnit, (int)sDefSkillComponent.listFinalValue[0]), DamageType.AP);
        }
    }

    #endregion

    #region Scollwarrior
    private void Skill_Scollwarrior_201030_Defense()
    {
        StopCoroutine(Skill_Scollwarrior_201030_Defense_Coroutine());
        StartCoroutine(Skill_Scollwarrior_201030_Defense_Coroutine());
    }

    IEnumerator Skill_Scollwarrior_201030_Defense_Coroutine()
    {
        cUnit.SetCastingSkill(true);

        yield return waitDefault;

        yield return new WaitUntil(() => GetNearUnit() != null);

        Unit _cTarget = GetNearUnit(2f);

        cUnit.ChangeMP(0);

        Vector3 _v3TargetPos = GetNearUnit().transform.position;

        cUnit.body.DOLookAt(_v3TargetPos, 0.1f);
        cUnit.PlayAnimation(UnitAniActionParam.DefSkill);

        yield return new WaitForSeconds(0.8f);

        float _duration = sDefSkillComponent.listValue[1].value[0];

        StartCoroutine(ScollwarriorCC(_cTarget, _duration));

        yield return new WaitForSeconds(_duration);

        cUnit.PlayAnimation(UnitAniActionParam.DefSkill);

        yield return new WaitForSeconds(1f);

        cUnit.body.DOLookAt(_v3TargetPos, 0.1f);

        yield return waitDefault;

        Collider[] _cHitColiders = Physics.OverlapSphere(_cTarget.bodyHolder.position, sDefSkillComponent.listValue[2].value[0] * 0.5f, LayerMask.GetMask("Unit"));
        List<Unit> _cHitUnits = new List<Unit>();

        foreach (var item in _cHitColiders)
        {
            if (item.GetComponent<Unit>() != null)
            {
                Unit _cTargetUnit = item.GetComponent<Unit>();
                if (_cTargetUnit != _cTarget && _cTargetUnit.attackUnit)
                    _cHitUnits.Add(_cTargetUnit);
            }
        }

        foreach (var item in _cHitUnits)
        {
            item.AddHP(cUnit, item.CalcDamage(DamageType.AP, cUnit, (int)sDefSkillComponent.listFinalValue[0]), DamageType.AP);
            item.GetBuff(BuffType.Airborne, cUnit, 0, sDefSkillComponent.listValue[3].value[cUnit.unitLevel - 1]);
        }

        _cTarget.transform.position = _v3TargetPos;
        _cTarget.body.localPosition = Vector3.zero;

        cUnit.SetCastingSkill(false);
    }

    IEnumerator ScollwarriorCC(Unit _cTarget, float _duration)
    {
        Transform _tfTail = cUnit.body.GetComponentInChildren<ScollwarriorTail>().transform;

        _cTarget.GetBuff(BuffType.Stern, cUnit, 0, _duration);

        float _crtTime = 0; 

        while (_crtTime < _duration + 1f)
        {
            _cTarget.body.position = _tfTail.position;

            yield return null;

            _crtTime += Time.deltaTime;
        }
    }
    #endregion

    #region Destroyer
    private void Skill_Destroyer_201032_Defense()
    {
        StopCoroutine(Skill_Destroyer_201032_Defense_Coroutine());
        StartCoroutine(Skill_Destroyer_201032_Defense_Coroutine());
    }

    IEnumerator Skill_Destroyer_201032_Defense_Coroutine()
    {
        yield return waitDefault;

        cUnit.ChangeMP(0);
        cUnit.SetCastingSkill(true);

        cUnit.PlayAnimation(UnitAniActionParam.DefSkill);

        yield return new WaitForSeconds(0.35f);

        Collider[] _colliders = Physics.OverlapSphere(cUnit.bodyHolder.position + Vector3.up * 0.25f, sDefSkillComponent.listValue[0].value[0] * 0.5f, cLayerMask);
        List<Unit> _listUnit = new List<Unit>();

        foreach (var item in _colliders)
        {
            Unit _cTempUnit = item.GetComponent<Unit>();
            if (_cTempUnit.attackUnit)
                _listUnit.Add(_cTempUnit);
        }

        foreach (var item in _listUnit)
        {
            item.AddHP(cUnit, item.CalcDamage(DamageType.AD, item, (int)sDefSkillComponent.listFinalValue[0]), DamageType.AD);
            item.GetBuff(BuffType.HealAmount, cUnit, sDefSkillComponent.listValue[5].value[0], sDefSkillComponent.listValue[2].value[0]);
            StartCoroutine(Destroyer_Damage(item));
        }

        cUnit.SetCastingSkill(false);
    }

    IEnumerator Destroyer_Damage(Unit _cUnit)
    {
        for (int i = 0; i < (int)(sDefSkillComponent.listValue[2].value[0] / sDefSkillComponent.listValue[3].value[0]); i++)
        {
            _cUnit.AddHP(cUnit, _cUnit.CalcDamage(DamageType.AP, cUnit, (int)sDefSkillComponent.listFinalValue[1]), DamageType.AP, true);

            yield return new WaitForSeconds(sDefSkillComponent.listValue[3].value[0]);
        }
    }
    #endregion

    #region Baykey
    private void Skill_Baykey_201035_Defense()
    {
        StopCoroutine(Skill_Baykey_201035_Defense_Coroutine());
        StartCoroutine(Skill_Baykey_201035_Defense_Coroutine());
    }

    IEnumerator Skill_Baykey_201035_Defense_Coroutine()
    {
        yield return waitDefault;

        cUnit.SetCastingSkill(true);
        cUnit.ChangeMP(0);

        WaitUntil _newUntil = new WaitUntil(() => GetNearUnit(cUnit.unitStat.fCrtAtkRange));
        WaitForSeconds _newWait = new WaitForSeconds(1f / cUnit.unitStat.fCrtAtkSpeed);

        for (int i = 0; i < sDefSkillComponent.listValue[0].value[cUnit.unitLevel - 1]; i++)
        {
            yield return _newWait;
            yield return _newUntil;
            Unit _cTarget = GetNearUnit();
            cUnit.body.DOLookAt(_cTarget.bodyHolder.position, 0.05f);
            StartCoroutine(Baykey_Damage(_cTarget));
            cUnit.PlayAnimation(UnitAniActionParam.DefSkill);
        }

        yield return waitDefault;

        cUnit.SetCastingSkill(false);
    }

    IEnumerator Baykey_Damage(Unit _cTarget)
    {
        yield return new WaitForSeconds(0.25f);

        _cTarget.AddHP(cUnit, _cTarget.CalcDamage(DamageType.AD, cUnit, (int)sDefSkillComponent.listFinalValue[0]), DamageType.AD);
    }
    #endregion

    #region Paladin
    private void Skill_Paladin_201036_Defense()
    {
        StopCoroutine(Skill_Paladin_201036_Defense_Coroutine());
        StartCoroutine(Skill_Paladin_201036_Defense_Coroutine());
    }

    IEnumerator Skill_Paladin_201036_Defense_Coroutine()
    {
        cUnit.SetCastingSkill(true);

        yield return waitDefault;

        cUnit.ChangeMP(0);

        Tile _cTargetTile = cUnit.currentTile.GetCrossInLine(2);
        int _iTargetTileIdx = _cTargetTile.GetTileIndexOfFinal();

        List<Tile> _cTargetTiles = new List<Tile>();
        int _targetCount = (int)sDefSkillComponent.listValue[0].value[cUnit.unitLevel - 1];

        for (int i = _targetCount - 1; i >= 0; i--)
        {
            int _iRelativePos = i - _iTargetTileIdx;
            _cTargetTiles.Add(_cTargetTile.GetNextTile(_iRelativePos));
        }

        cUnit.body.DOLookAt(new Vector3(_cTargetTile.tilePos.x, cUnit.body.position.y, _cTargetTile.tilePos.z), 0.1f);

        cUnit.PlayAnimation(UnitAniActionParam.DefSkill, true);

        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < _cTargetTiles.Count; i++)
        {
            GameObject _goEffect = cUnit.SpawnDefSkillEffect(0, 3f);

            _goEffect.transform.position = _cTargetTiles[i].tilePos + Vector3.up * 2.5f;

            float _fFallTime = 0.3f;

            _goEffect.transform.DOMoveY(1f, _fFallTime);
            StartCoroutine(PaladinDamageCoroutine(_fFallTime, _cTargetTiles[i].tilePos));

            yield return new WaitForSeconds(0.2f);
        }

        cUnit.PlayAnimation(UnitAniActionParam.StopSkill, true);

        yield return new WaitForSeconds(0.5f);

        cUnit.SetCastingSkill(false);
    }

    IEnumerator PaladinDamageCoroutine(float _fFallTime, Vector3 _v3TargetPos)
    {
        yield return new WaitForSeconds(_fFallTime);

        Collider[] _cTargetUnits = Physics.OverlapSphere(_v3TargetPos, sDefSkillComponent.listValue[1].value[0] * 0.5f, cLayerMask.value);
        List<Unit> _listTargetUnits = new List<Unit>();

        for (int i = 0; i < _cTargetUnits.Length; i++)
            _listTargetUnits.Add(_cTargetUnits[i].GetComponent<Hero>());

        foreach (var item in _listTargetUnits)
        {
            if (item.attackUnit)
                item.AddHP(cUnit, item.CalcDamage(DamageType.AD, cUnit, (int)sDefSkillComponent.listFinalValue[0]), DamageType.AD);
        }

        Collider[] _cCenterTargetHeros = Physics.OverlapSphere(_v3TargetPos, 0.1f, cLayerMask.value);

        List<Unit> _listCenterTargetHero = new List<Unit>();

        for (int i = 0; i < _cCenterTargetHeros.Length; i++)
            _listCenterTargetHero.Add(_cCenterTargetHeros[i].GetComponent<Unit>());

        foreach (var item in _listCenterTargetHero)
        {
            if (item.attackUnit)
            {
                item.AddHP(cUnit, item.CalcDamage(DamageType.AP, cUnit, (int)sDefSkillComponent.listFinalValue[1]), DamageType.AP);
                item.GetBuff(BuffType.Stern, cUnit, 0, sDefSkillComponent.listValue[4].value[0]);
            }
        }
    }
    #endregion

    #region Preeka
    private void Skill_Preeka_201039_Defense()
    {
        StopCoroutine(Skill_Preeka_201039_Defense_Coroutine());
        StartCoroutine(Skill_Preeka_201039_Defense_Coroutine());
    }

    IEnumerator Skill_Preeka_201039_Defense_Coroutine()
    {
        cUnit.SetCastingSkill(true);

        yield return waitDefault;

        cUnit.ChangeMP(0);

        Collider[] _cTargets = Physics.OverlapSphere(cUnit.friendUnit.bodyHolder.transform.position + Vector3.up * 0.5f, sDefSkillComponent.listValue[1].value[0] * 0.5f, cLayerMask.value);

        foreach (var item in _cTargets)
        {
            Unit _targets = item.GetComponent<Unit>();
            _targets.AddHP(cUnit, _targets.CalcDamage(DamageType.AP, cUnit, (int)sDefSkillComponent.listFinalValue[0]), DamageType.AP);
        }

        yield return new WaitForSeconds(0.5f);

        Quaternion _tempRotation = cUnit.body.rotation;
        cUnit.currentTile.SwitchUnit(cUnit.friendUnit.currentTile, false, false, false);

        yield return null;

        cUnit.body.rotation = cUnit.friendUnit.body.rotation;
        cUnit.friendUnit.body.rotation = _tempRotation;

        cUnit.SetCastingSkill(false);

        yield return new WaitForSeconds(0.3f);
    }
    #endregion;

    #region Slater
    private void Skill_Slater_201041_Defense()
    {
        StopCoroutine(Skill_Slater_201041_Defense_Coroutine());
        StartCoroutine(Skill_Slater_201041_Defense_Coroutine());
    }

    IEnumerator Skill_Slater_201041_Defense_Coroutine()
    {
        if (!Tools.GetRandomForPercentResult(sDefSkillComponent.listFinalValue[0] * 4f))
            yield break;

        Unit _cTarget = cUnit.damagedMineUnit;

        yield return waitDefault;

        _cTarget.AddHP(cUnit, _cTarget.CalcDamage(DamageType.AP, cUnit, (int)sDefSkillComponent.listFinalValue[1]), DamageType.AP);
        _cTarget.GetBuff(BuffType.Stern, cUnit, 0, sDefSkillComponent.listValue[2].value[cUnit.unitLevel - 1]);
    }
    #endregion

    #region Cawin
    private void Skill_Cawin_201043_Defense()
    {
        StopCoroutine(Skill_Cawin_201043_Defense_Coroutine());
        StartCoroutine(Skill_Cawin_201043_Defense_Coroutine());
    }

    IEnumerator Skill_Cawin_201043_Defense_Coroutine()
    {
        cUnit.SetCastingSkill(true);

        Unit _cTarget = GetNearUnit(cUnit.unitStat.fCrtAtkRange);

        yield return waitDefault;

        cUnit.body.DOLookAt(new Vector3(_cTarget.body.position.x, cUnit.transform.position.y, _cTarget.body.position.z), 0.1f);

        yield return waitDefault;

        cUnit.ChangeMP(0);

        cUnit.PlayAnimation(UnitAniActionParam.DefSkill);

        yield return new WaitForSeconds(1.3f);

        _cTarget.AddHP(cUnit, _cTarget.CalcDamage(DamageType.AP, cUnit, (int)sDefSkillComponent.listFinalValue[0]), DamageType.AP);
        _cTarget.GetBuff(BuffType.Stern, cUnit, 0, sDefSkillComponent.listValue[1].value[cUnit.unitLevel - 1]);

        int _iTargetTileIndex = InGameManager.instance.cAStar.finalTileList.IndexOf(_cTarget.GetStayTile()) - (int)sDefSkillComponent.listValue[2].value[cUnit.unitLevel - 1];
        _iTargetTileIndex = Mathf.Clamp(_iTargetTileIndex, 0, int.MaxValue);

        float _fJumpTime = sDefSkillComponent.listValue[1].value[cUnit.unitLevel - 1] * 0.5f;

        _cTarget.transform.DOJump(InGameManager.instance.cAStar.finalTileList[_iTargetTileIndex].tilePos, 1.5f, 1, _fJumpTime);

        yield return waitDefault;

        cUnit.SetCastingSkill(false);

        yield return new WaitForSeconds(_fJumpTime - 0.1f);

        Collider[] _cHitColiders = Physics.OverlapSphere(_cTarget.bodyHolder.position, sDefSkillComponent.listValue[5].value[0] * 0.5f, LayerMask.GetMask("Unit"));
        List<Unit> _cHitUnits = new List<Unit>();

        foreach (var item in _cHitColiders)
        {
            if (item.GetComponent<Unit>() != null)
            {
                Unit _cTargetUnit = item.GetComponent<Unit>();
                if (_cTargetUnit != _cTarget && _cTargetUnit.attackUnit)
                    _cHitUnits.Add(_cTargetUnit);
            }
        }

        foreach (var item in _cHitUnits)
        {
            item.AddHP(cUnit, item.CalcDamage(DamageType.AP, cUnit, (int)(item.unitStat.iMaxHp * sDefSkillComponent.listValue[3].value[0])), DamageType.AP);
            item.GetBuff(BuffType.Airborne, cUnit, 0, sDefSkillComponent.listValue[4].value[0]);
        }
    }
    #endregion

    #region Balckeria
    private void Skill_Balckeria_201045_Defense()
    {
        StopCoroutine(Skill_Balckeria_201045_Defense_Coroutine());
        StartCoroutine(Skill_Balckeria_201045_Defense_Coroutine());
    }

    IEnumerator Skill_Balckeria_201045_Defense_Coroutine()
    {
        cUnit.SetCastingSkill(true);

        WaitForSeconds _cNewWait = new WaitForSeconds(sDefSkillComponent.listValue[2].value[0]);

        yield return waitDefault;

        cUnit.ChangeMP(0);

        cUnit.body.transform.DOLookAt(GetNearUnit().bodyHolder.position, 0.1f);

        yield return waitDefault;

        cUnit.PlayAnimation(UnitAniActionParam.DefSkill);

        yield return new WaitForSeconds(0.5f);

        float _fEffectScale = cUnit.unitLevel <= 2 ? 0.4f : 0.5f;

        GameObject _goEffect = cUnit.SpawnDefSkillEffect(0, sDefSkillComponent.listValue[1].value[0]);
        _goEffect.transform.position = cUnit.currentTile.GetCrossInLine(3).tilePos;
        _goEffect.transform.GetChild(0).localScale = _fEffectScale * Vector3.one;

        listHitBox.Clear();
        listHitBox.Add(_goEffect.GetComponentInChildren<HitBox>(true));

        listHitBox[0].hitBoxEnterChain -= BalckeriaDamage;
        listHitBox[0].hitBoxEnterChain += BalckeriaDamage;

        for (int i = 0; i < sDefSkillComponent.listValue[1].value[0] / sDefSkillComponent.listValue[2].value[0]; i++)
        {
            listHitBox[0].gameObject.SetActive(true);

            yield return _cNewWait;

            listHitBox[0].gameObject.SetActive(false);
        }

        cUnit.SetCastingSkill(false);
    }

    private void BalckeriaDamage(Unit _cTarget, out BuffValue _sBuffValue)
    {
        if (_cTarget.attackUnit)
            _cTarget.AddHP(cUnit, _cTarget.CalcDamage(DamageType.AP, cUnit, (int)sDefSkillComponent.listFinalValue[0]), DamageType.AP, true, false, false, int.Parse($"201045{cUnit.transform.GetSiblingIndex()}"));

        _sBuffValue = null;
    }
    #endregion

    #region Cramis
    private void Skill_Cramis_201046_Defense()
    {
        StopCoroutine(Skill_Cramis_201046_Defense_Coroutine());
        StartCoroutine(Skill_Cramis_201046_Defense_Coroutine());
    }

    IEnumerator Skill_Cramis_201046_Defense_Coroutine()
    {
        yield return waitDefault;

        cUnit.ChangeMP(0);
        cUnit.SetCastingSkill(true);

        cUnit.PlayAnimation(UnitAniActionParam.DefSkill, true);

        yield return new WaitForSeconds(3f);

        Vector3 _v3Origin = cUnit.body.position + Vector3.up * 0.25f;
        Vector3 v3LDirection = cUnit.body.forward;
        float _time = sDefSkillComponent.listValue[0].value[cUnit.unitLevel - 1];
        float _fDistance = sDefSkillComponent.listValue[1].value[0];
        float _crtTime = 0;
        float _fCastTime = 0;
        float _fRepeatTime = sDefSkillComponent.listValue[2].value[0];

        List<LineRenderer> lineRenderers = new List<LineRenderer>();

        for (int i = 0; i < 5; i++)
        {
            LineRenderer lineRenderer = new GameObject("LineRenderer").AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
            lineRenderer.material.color = Color.red;
            lineRenderer.widthMultiplier = 0.1f;
            lineRenderer.positionCount = 2;

            lineRenderers.Add(lineRenderer);
        }

        while (_crtTime <= _time)
        {
            for (int i = 0; i < 5; i++)
            {
                LineRenderer lineRenderer = lineRenderers[i];

                float angle = Mathf.Deg2Rad * (-90 + i * 36);

                Vector3 newDirection = new Vector3(
                    v3LDirection.x * Mathf.Cos(angle) - v3LDirection.z * Mathf.Sin(angle),
                    v3LDirection.y,
                    v3LDirection.x * Mathf.Sin(angle) + v3LDirection.z * Mathf.Cos(angle)
                );

                RaycastHit _hit;

                if (Physics.Raycast(_v3Origin, newDirection, out _hit, 3f, LayerMask.GetMask("Unit")))
                {
                    lineRenderer.SetPosition(0, _v3Origin);
                    lineRenderer.SetPosition(1, _hit.point + newDirection.normalized * 0.25f);

                    if (_fCastTime >= _fRepeatTime)
                    {
                        Unit _cTargetUnit = _hit.transform.GetComponentInParent<Unit>();
                        float _fCenter = i == 2 ? 1f + sDefSkillComponent.listValue[4].value[0] * 0.01f : 1f;

                        if (_cTargetUnit.attackUnit)
                            _cTargetUnit.AddHP(cUnit, _cTargetUnit.CalcDamage(DamageType.AP, cUnit, (int)(sDefSkillComponent.listFinalValue[0] * _fCenter)), DamageType.AP, true, false, true, 201046 + cUnit.transform.GetSiblingIndex());
                    }
                }
                else
                {
                    lineRenderer.SetPosition(0, _v3Origin);
                    lineRenderer.SetPosition(1, _v3Origin + newDirection * _fDistance);
                }
            }

            yield return null;

            if (_fCastTime >= _fRepeatTime)
                _fCastTime = 0;

            _fCastTime += Time.deltaTime;
            _crtTime += Time.deltaTime;

            if (!cUnit.alive || !cUnit.isPlaced)
            {
                foreach (var lineRenderer in lineRenderers)
                    Destroy(lineRenderer.gameObject);

                yield break;
            }
        }

        foreach (var lineRenderer in lineRenderers)
            Destroy(lineRenderer.gameObject);

        cUnit.PlayAnimation(UnitAniActionParam.StopSkill, true);

        yield return waitDefault;

        cUnit.SetCastingSkill(false);
    }
    #endregion

    #region Raino
    private void Skill_Raino_201047_Defense()
    {
        StopCoroutine(Skill_Raino_201047_Defense_Coroutine());
        StartCoroutine(Skill_Raino_201047_Defense_Coroutine());
    }

    IEnumerator Skill_Raino_201047_Defense_Coroutine()
    {
        yield return waitDefault;

        cUnit.ChangeMP(0);
        cUnit.SetCastingSkill(true);

        cUnit.body.DOLookAt(GetNearUnit().body.position, 0.1f);

        yield return waitDefault;

        cUnit.PlayAnimation(UnitAniActionParam.DefSkill);

        WaitForSeconds _cDelay = new WaitForSeconds(0.9f);

        for (int i = 0; i < (int)sDefSkillComponent.listValue[2].value[0]; i++)
        {
            yield return _cDelay;

            Collider[] _cTargets = Physics.OverlapSphere(cUnit.body.position + Vector3.up * 0.5f, sDefSkillComponent.listValue[0].value[0] * 0.5f, cLayerMask.value);

            foreach (var item in _cTargets)
            {
                Hero _cHero = item.GetComponent<Hero>();
                if (_cHero.attackUnit)
                    _cHero.AddHP(cUnit, _cHero.CalcDamage(DamageType.AD, cUnit, (int)sDefSkillComponent.listFinalValue[0]), DamageType.AD);
            }
        }

        yield return waitDefault;

        List<Tile> _targetFriendlyTiles = cUnit.currentTile.GetNearbyTiles((int)sDefSkillComponent.listValue[3].value[cUnit.unitLevel - 1]);

        float _fBuffTime = sDefSkillComponent.listValue[4].value[0];

        foreach (var item in _targetFriendlyTiles)
        {
            if (item.isUnit)
            {
                if (item.placedUnit.isOwn)
                {
                    item.placedUnit.GetBuff(BuffType.Attack, cUnit, (int)sDefSkillComponent.listFinalValue[1], _fBuffTime);
                }
            }
        }

        GameObject _goFieldEffect = cUnit.SpawnDefSkillEffect(0, _fBuffTime);
        _goFieldEffect.transform.position = cUnit.body.position;
        float _fFEScale = cUnit.unitLevel <= 2 ? 0.9f : 1.25f;
        _goFieldEffect.transform.GetChild(0).localScale = new Vector3(_fFEScale, 1f, _fFEScale);

        yield return waitDefault;

        cUnit.SetCastingSkill(false);
    }
    #endregion

    #region DragonHunter
    private void Skill_DragonHunter_201048_Defense()
    {
        StopCoroutine(Skill_DragonHunter_201048_Defense_Coroutine());
        StartCoroutine(Skill_DragonHunter_201048_Defense_Coroutine());
    }

    IEnumerator Skill_DragonHunter_201048_Defense_Coroutine()
    {
        yield return waitDefault;

        if (cUnit.targetUnit != null)
            cUnit.body.DOLookAt(new Vector3(cUnit.targetUnit.body.position.x, cUnit.transform.position.y, cUnit.targetUnit.body.position.z), 0.1f);

        yield return waitDefault;

        cUnit.SetCastingSkill(true);
        cUnit.ChangeMP(0);

        cUnit.PlayAnimation(UnitAniActionParam.DefSkill);
        yield return new WaitForSeconds(0.8f);

        Vector3 _v3TargetPos = cUnit.body.transform.position + cUnit.body.transform.forward * 30f + Vector3.up * 0.5f;

        GameObject _goEffect = cUnit.SpawnDefSkillEffect(0, 3f);
        _goEffect.transform.position = cUnit.body.position + Vector3.up * 0.5f;
        _goEffect.transform.rotation = cUnit.body.rotation;
        _goEffect.transform.DOMove(_v3TargetPos, 2.5f);

        listHitBox.Clear();
        listHitBox.Add(_goEffect.GetComponentInChildren<HitBox>());

        listHitBox[0].hitBoxEnterChain -= DragonHunterDamage;
        listHitBox[0].hitBoxEnterChain += DragonHunterDamage;

        yield return waitDefault;
        
        cUnit.SetCastingSkill(false);
    }

    private void DragonHunterDamage(Unit _cTarget, out BuffValue? _sBuffValue)
    {
        int _iDragonDmg = 1;

        if (_cTarget is Hero)
            _iDragonDmg = ((_cTarget as Hero).heroStat.eTribe == HeroTribe.Dragon || (_cTarget as Hero).heroStat.eTribe == HeroTribe.Dragonoid) ? 2 : 1;

        if (_cTarget.attackUnit)
            _cTarget.AddHP(cUnit, _cTarget.CalcDamage(DamageType.True, cUnit, (int)skillComponent_Def.listFinalValue[0]) * _iDragonDmg, DamageType.True);

        _sBuffValue = null;
    }
    #endregion

    #region Maist
    private void Skill_Maist_201053_Defense()
    {
        StopCoroutine(Skill_Maist_201053_Defense_Coroutine());
        StartCoroutine(Skill_Maist_201053_Defense_Coroutine());
    }

    IEnumerator Skill_Maist_201053_Defense_Coroutine()
    {
        yield return waitDefault;

        cUnit.SetCastingSkill(true);
        cUnit.ChangeMP(0);

        yield return new WaitForSeconds(1.5f);

        GameObject _goEffect = cUnit.SpawnDefSkillEffect(0, 5f);
        listHitBox.Clear();
        listHitBox.Add(_goEffect.GetComponentInChildren<HitBox>());

        listHitBox[0].gameObject.SetActive(true);

        listHitBox[0].hitBoxEnterChain -= MaistWave_Enter;
        listHitBox[0].hitBoxEnterChain += MaistWave_Enter;

        listHitBox[0].transform.localPosition = Vector3.zero;
        listHitBox[0].transform.DOLocalMoveZ(10f, 3.5f);

        _goEffect.transform.position = new Vector3(cUnit.currentTile.transform.position.x, 0.55f, -5.5f);

        yield return new WaitForSeconds(1f);

        cUnit.SetCastingSkill(false);
    }

    private void MaistWave_Enter(Unit _cTarget, out BuffValue _sBuffValue)
    {
        if (_cTarget.attackUnit)
        {
            _cTarget.AddHP(cUnit, _cTarget.CalcDamage(DamageType.AP, cUnit, (int)sDefSkillComponent.listFinalValue[0]), DamageType.AP);
            _sBuffValue = _cTarget.GetBuff(BuffType.Airborne, cUnit, 0, sDefSkillComponent.listValue[1].value[0]);
            _sBuffValue = _cTarget.GetBuff(BuffType.Silent, cUnit, 0, sDefSkillComponent.listValue[2].value[cUnit.unitLevel - 1]);
        }
        else
            _sBuffValue = null;
    }
    #endregion

    #region Senland
    private void Skill_Senland_201050_Defense()
    {
        StopCoroutine(Skill_Senland_201050_Defense_Coroutine());
        StartCoroutine(Skill_Senland_201050_Defense_Coroutine());
    }

    IEnumerator Skill_Senland_201050_Defense_Coroutine()
    {
        while (GetNearUnit(cUnit.unitStat.fCrtAtkRange * 1.4f) == null)
            yield return null;

        Unit _cTarget = GetNearUnit(cUnit.unitStat.fCrtAtkRange * 1.4f);

        yield return waitDefault;

        cUnit.body.DOLookAt(_cTarget.bodyHolder.position, 0.1f);

        yield return waitDefault;

        cUnit.SetCastingSkill(true);

        cUnit.ChangeMP(0);
        cUnit.PlayAnimation(UnitAniActionParam.DefSkill);

        yield return new WaitForSeconds(1f);

        _cTarget.GetBuff(BuffType.Stiff, cUnit, 0, sDefSkillComponent.listFinalValue[0], false, false, "201050");

        yield return waitDefault;

        cUnit.SetCastingSkill(false);

        yield return new WaitForSeconds(sDefSkillComponent.listFinalValue[0] - 0.1f);

        _cTarget.body.DORotate(Vector3.zero, 0.1f);
    }
    #endregion

    #region Saint
    private void Skill_Saint_201055_Defense()
    {
        StopCoroutine(Skill_Saint_201055_Defense_Coroutine());
        StartCoroutine(Skill_Saint_201055_Defense_Coroutine());
    }

    IEnumerator Skill_Saint_201055_Defense_Coroutine()
    {
        yield return new WaitForSeconds(0.25f);

        cUnit.SetCastingSkill(true);
        cUnit.ChangeMP(0);

        cUnit.PlayAnimation(UnitAniActionParam.DefSkill);

        yield return new WaitForSeconds(1.5f);

        foreach (var item in InGameManager.instance.cPlayerController.ownUnits.Where(x => x.battleUnit))
        {
            string _strCode = "201055D";
            item.ExitBuff(_strCode);
            float _fTime = sDefSkillComponent.listValue[0].value[cUnit.unitLevel - 1];
            item.GetBuff(BuffType.SendDamage, cUnit, sDefSkillComponent.listFinalValue[0], _fTime, false, false, $"{_strCode}A");
            item.GetBuff(BuffType.ReciveDamage, cUnit, sDefSkillComponent.listFinalValue[0], _fTime, false, false, $"{_strCode}B");
            item.GetBuff(BuffType.PlusMp, cUnit, sDefSkillComponent.listValue[2].value[cUnit.unitLevel - 1], _fTime, false, false, $"{_strCode}C");
            GameObject _goEffect = cUnit.SpawnDefSkillEffect(0, _fTime);
            _goEffect.transform.SetParent(item.bodyHolder.transform);
            _goEffect.transform.localPosition = Vector3.up * 0.15f;
            _goEffect.transform.localRotation = Quaternion.identity;
        }

        cUnit.SetCastingSkill(false);
    }
    #endregion

    #region Peurika
    private void Skill_Peurika_201057_Defense()
    {
        StopCoroutine(Skill_Peurika_201057_Defense_Coroutine());
        StartCoroutine(Skill_Peurika_201057_Defense_Coroutine());
    }

    IEnumerator Skill_Peurika_201057_Defense_Coroutine()
    {
        yield return waitDefault;

        cUnit.PlayAnimation(UnitAniActionParam.DefSkill, true);

        cUnit.SetCastingSkill(true);
        cUnit.ChangeMP(0);

        Tile _tileA = null;
        Tile _tileB = null;

        List<Tile> _listTiles = InGameManager.instance.cAStar.GetFInalTileRoutListWithoutCamps().ToList();
        Tools.ShuffleList(_listTiles);

        _tileA = _listTiles[0];
        _tileB = _listTiles[1];

        GameObject _goEffectA = cUnit.SpawnDefSkillEffect(0, sDefSkillComponent.listFinalValue[0]);
        _goEffectA.transform.position = _tileA.transform.position + Vector3.up * 0.5f;
        GameObject _goEffectB = cUnit.SpawnDefSkillEffect(0, sDefSkillComponent.listFinalValue[0]);
        _goEffectB.transform.position = _tileB.transform.position + Vector3.up * 0.5f;

        GameObject _goInHoleEffect = InGameManager.instance.cAStar.GetRangeBetweenTwoTiles(_tileA, InGameManager.instance.cAStar.finalTileList[0]) > InGameManager.instance.cAStar.GetRangeBetweenTwoTiles(_tileB, InGameManager.instance.cAStar.finalTileList[0]) ? _goEffectA : _goEffectB;
        GameObject _goOutHoleEffect = _goInHoleEffect == _goEffectA ? _goEffectB : _goEffectA;

        _goInHoleEffect.name = "InHole";
        _goOutHoleEffect.name = "OutHole";

        listHitBox.Clear();
        listHitBox.Add(_goInHoleEffect.GetComponentInChildren<HitBox>(true));
        listHitBox.Add(_goOutHoleEffect.GetComponentInChildren<HitBox>(true));

        listHitBox[0].hitBoxEnterChain -= PeurikaTrigger_Enter;
        listHitBox[0].hitBoxEnterChain += PeurikaTrigger_Enter;

        yield return new WaitForSeconds(sDefSkillComponent.listFinalValue[0]);

        cUnit.PlayAnimation(UnitAniActionParam.StopSkill, true);
        cUnit.SetCastingSkill(false);
    }

    private void PeurikaTrigger_Enter(Unit _cTarget, out BuffValue? _sBuffValue)
    {
        if (!_cTarget.GetCustomMark("201057"))
            StartCoroutine(PeurikaTrigger_Enter_Coroutine(_cTarget));

        _sBuffValue = null;
    }

    IEnumerator PeurikaTrigger_Enter_Coroutine(Unit _cTarget)
    {
        GameObject _goEffect = cUnit.SpawnDefSkillEffect(1, 1f);
        _goEffect.transform.position = _cTarget.body.position + Vector3.up * 0.5f;

        if (_cTarget.attackUnit)
        {
            _cTarget.AddCustomMark("201057", true);

            yield return new WaitForSeconds(0.6f);

            _cTarget.GetBuff(BuffType.Stern, cUnit, 0, 1f);
            _cTarget.OnOffNavAgent(false);
            _cTarget.bodyHolder.DOLocalMoveY(-1.5f, 0.5f);

            yield return new WaitForSeconds(0.5f);

            _cTarget.bodyHolder.DOLocalMoveY(0, 0.5f);
            _cTarget.transform.position = new Vector3(listHitBox[1].transform.position.x, 0.5f, listHitBox[1].transform.position.z);
            _cTarget.OnOffNavAgent(true);
        }
    }

    #endregion

    public HeroSkillComponent skillComponent_Def { get => sDefSkillComponent; set => sDefSkillComponent = value; }
    public HeroSkillComponent skillComponent_Atk { get => sAtkSkillComponent; set => sAtkSkillComponent = value; }
}