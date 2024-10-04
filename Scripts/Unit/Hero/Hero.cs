using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum HeroTribe { None, Human, Beast, Dragon, Dragonoid, Elemental, Fairy, Undead, Plant, Cavalry, Guardian, Berserker }
public enum HeroClass { None, Magician, Flying, Armoric, TwinSwords, Insect, Fighter, Natual, Swordsman, BigHorns, Ranger, Priest }

public class Hero : Unit
{
    [SerializeField, ReadOnlyInspector]
    protected int iHeroNumber;

    [SerializeField]
    protected HeroStatus sHeroStat;

    [SerializeField]
    protected Transform tfFootHold;

    [SerializeField, ReadOnlyInspector]
    protected DealParserElemnt cDealParser;

    [SerializeField]
    private List<Equipment> listEquipment;
    [SerializeField, ReadOnlyInspector]
    private bool[] bIsEquips;

    [ContextMenu("Init Pool")]
    void InitPool()
    {
        cObjPool = GetComponentInChildren<PoolingManager>();
        cObjPool.Reset();
    }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        sOriginHeroStatus = sHeroStat;
        iHeroCost = (int)sUnitStat.eRank;
        listEquipment = new List<Equipment>();
    }

    protected override void OnEnable()
    {
        bIsEquips = new bool[3];
    }

    public override void SetupStat()
    {
        base.SetupStat();

        int _iEquipHP = 0;
        int _iEquipMP = 0;
        int _iEquipAD = 0;
        int _iEquipAP = 0;
        int _iEquipADDef = 0;
        int _iEquipAPDef = 0;
        float _fEquipAtkSpd = 0;

        for (int i = 0; i < listEquipment.Count; i++)
        {
            _iEquipHP += listEquipment[i].card.stat.iAddHP;
            _iEquipMP += listEquipment[i].card.stat.iAddMP;
            _iEquipAD += listEquipment[i].card.stat.iAddAD;
            _iEquipAP += listEquipment[i].card.stat.iAddAP;
            _iEquipADDef += listEquipment[i].card.stat.iAddDef;
            _iEquipAPDef += listEquipment[i].card.stat.iAddAPDef;
            _fEquipAtkSpd += listEquipment[i].card.stat.fAddAtkSpd;

            sUnitStat.iAddAttack += sHeroStat.iBeastAttack + _iEquipAD;
            sUnitStat.iAddPower += sHeroStat.iMagicianPower + _iEquipAP;
            sUnitStat.iAddADDef += _iEquipADDef;
            sUnitStat.iAddAPDef += _iEquipAPDef;
            SetAttackSpeed(sUnitStat.fOrgAtkSpeed * (1f + _fEquipAtkSpd * 0.01f));
        }

        cSkill.UpdateFinalValue();
    }

    public void SetDealParser(DealParserElemnt _cTarget)
    {
        cDealParser = _cTarget;
    }

    protected override void TakeHit()
    {
        base.TakeHit();
    }

    public override void AddHP(Unit _cCaster, int _iValue, DamageType _eDmgType, bool _bNoneTarget = false, bool _bIsCriticalPossible = false, bool _bIsEvationPossible = false, int _iDamageCode = 0)
    {
        base.AddHP(_cCaster, _iValue, _eDmgType, _bNoneTarget, _bIsCriticalPossible, _bIsEvationPossible, _iDamageCode);

        if (_cCaster != null && _cCaster is Hero && _iValue < 0 && bAttackUnit)
            (_cCaster as Hero).cDealParser.AddDamage(Mathf.Abs(_iValue), _eDmgType);
    }

    public void EquipmentMounting(int _iCardId)
    {
        Equipment _cEquip = new Equipment(_iCardId);

        listEquipment.Add(_cEquip);

        cHUDUI.OnOffEquipSlot(true);
        cHUDUI.equpSlot.SetItem(_iCardId);

        bIsEquips[(int)_cEquip.equipType] = true;

        Card _cCard = CardManager.instance.GetCardWithID(_iCardId);

        if (_cCard.stat.iAddHP != 0)
            GetBuff(BuffType.MaxHp, this, _cCard.stat.iAddHP, 0, true);
        if (_cCard.stat.iSubMaxMP != 0)
            GetBuff(BuffType.MaxMp, this, _cCard.stat.iSubMaxMP, 0, true);
        if (_cCard.stat.iAddAD != 0)
            GetBuff(BuffType.Attack, this, _cCard.stat.iAddAD, 0, true);
        if (_cCard.stat.iAddAP != 0)
            GetBuff(BuffType.Power, this, _cCard.stat.iAddAP, 0, true);
        if (_cCard.stat.fAddMoveSpd != 0)
            GetBuff(BuffType.MoveSpeed, this, _cCard.stat.fAddMoveSpd, 0, true);
        if (_cCard.stat.fAddAtkSpd != 0)
            GetBuff(BuffType.AtkSpeed, this, _cCard.stat.fAddAtkSpd, 0, true);
        if (_cCard.stat.iAddDef != 0)
            GetBuff(BuffType.ADDef, this, _cCard.stat.iAddDef, 0, true);
        if (_cCard.stat.iAddAPDef != 0)
            GetBuff(BuffType.Power, this, _cCard.stat.iAddAPDef, 0, true);

        SetupStat();
    }

    public int heroNumber { get => iHeroNumber; set => iHeroNumber = value; }
    public Transform footHold { get => tfFootHold; }
    public HeroStatus heroStat { get => sHeroStat; set => sHeroStat = value; }
    public bool[] equipBool { get => bIsEquips; }
}
