using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Tooltip("Camp")]
    [SerializeField]
    private CampStatus sCampStat;

    [Tooltip("Unit")]
    [SerializeField]
    private List<Unit> listOwnUnit;
    [SerializeField]
    private List<Unit> listPlacedUnit;
    [SerializeField]
    private List<Hero> listOwnHero, listWaitingHero, listPlacedHero;

    [SerializeField, ReadOnlyInspector]
    private List<int> listCampNeedExp;
    [SerializeField, ReadOnlyInspector]
    private TextMeshPro txtBatchDisplay;
    private Color cOrgBatchTextColor;

    void Awake()
    {
        listOwnUnit = new List<Unit>();
        listOwnHero = new List<Hero>();
        listPlacedUnit = new List<Unit>();
        listWaitingHero = new List<Hero>();
        listPlacedHero = new List<Hero>();
        cOrgBatchTextColor = txtBatchDisplay.color;
    }

    void Start()
    {
        InitPlayer();

        listOwnUnit.Add(FindObjectOfType<PlayerCamp>());
    }

    [ContextMenu("Init")]
    private void Init()
    {
        listCampNeedExp = new List<int>();
        txtBatchDisplay = FindObjectOfType<TextMeshPro>(true);

        for (int i = 1; i < 21; i++)
            listCampNeedExp.Add(2 * i * i);
    }

    private void InitPlayer()
    {
        sCampStat.iCampMaxHp = 100;
        sCampStat.iCampCrtHp = sCampStat.iCampMaxHp;
        sCampStat.iGold = 1;
        sCampStat.iCampLevel = 1;
        sCampStat.iCampNeedExp = listCampNeedExp[0];
        sCampStat.iExpClickCount = 0;
        sCampStat.iCampCrtExp = 0;
        sCampStat.iExpCost = 4;
        sCampStat.iMaxUnitBatch = 1;
        sCampStat.iExpIncreasePoint = 4;

        UpdateCampInfo();
        InGameManager.instance.cPlayerInfoUI.UpdateHPBowl(sCampStat);
    }

    public void GetCampDamage(int _iDamage)
    {
        sCampStat.iCampCrtHp -= _iDamage;
        sCampStat.iCampCrtHp = Mathf.Clamp(sCampStat.iCampCrtHp, 0, sCampStat.iCampMaxHp);

        InGameManager.instance.cPlayerInfoUI.UpdateHPBowl(sCampStat);
        InGameManager.instance.stageController.isWin = false;
    }

    public void GetCampHeal(int _iHeal)
    {
        sCampStat.iCampCrtHp += _iHeal;
        sCampStat.iCampCrtHp = Mathf.Clamp(sCampStat.iCampCrtHp, 0, sCampStat.iCampMaxHp);

        InGameManager.instance.cPlayerInfoUI.UpdateHPBowl(sCampStat);
    }

    public void UpdateCampInfo()
    {
        InGameManager.instance.cPlayerInfoUI.UpdatePlayerInfo(sCampStat);
        txtBatchDisplay.text = listPlacedHero.Count + " / " + sCampStat.iMaxUnitBatch;

        if (listPlacedHero.Count > sCampStat.iMaxUnitBatch)
            txtBatchDisplay.color = Color.red;
        else if (listPlacedHero.Count < sCampStat.iMaxUnitBatch)
            txtBatchDisplay.color = Color.gray;
        else
            txtBatchDisplay.color = cOrgBatchTextColor;

        txtBatchDisplay.alpha = 0.5f;

        InGameManager.instance.cPlayerInfoUI.UpdatePlayerInfo(sCampStat);
        
        for (int i = 0; i < (int)UnitRank.Legendary; i++)
            InGameManager.instance.cPlayerInfoUI.UpdatePercentage(i, GameManager.instance.marketPercentage[i].list[sCampStat.iCampLevel - 1]);
    }

    public bool CanRoundStart()
    {
        return listPlacedHero.Count <= sCampStat.iMaxUnitBatch;
    }

    public void AddGold(int _iValue)
    {
        sCampStat.iGold += _iValue;
        sCampStat.iGold = Mathf.Clamp(sCampStat.iGold, 0, 999);

        UpdateCampInfo();
    }

    public bool TryUseGold(int _iValue, bool _bPass = true)
    {
        if (sCampStat.iGold < _iValue)
        {
            GameManager.instance.OpenNoticePanel("Not enough gold");
            return false;
        }

        if (_bPass)
            sCampStat.iGold -= _iValue;

        UpdateCampInfo();

        return true;
    }

    public bool CheckCampLevel(int _iValue)
    {
        if (sCampStat.iCampLevel < _iValue)
        {
            GameManager.instance.OpenNoticePanel("User's camp level is low.");
            InGameManager.instance.cCardController.AllOffGlow();
            return false;
        }

        UpdateCampInfo();

        return true;
    }

    public void IncreaseBasicExp()
    {
        if (TryUseGold(sCampStat.iExpCost))
        {
            sCampStat.iExpClickCount += 1;
            sCampStat.iCampCrtExp += sCampStat.iExpIncreasePoint;

            if (sCampStat.iCampCrtExp >= sCampStat.iCampNeedExp)
                CampLevelUp(sCampStat.iCampCrtExp - sCampStat.iCampNeedExp);

            UpdateCampInfo();

            InGameManager.instance.GetPlayerCamp().AbleExpUp(sCampStat.iExpIncreasePoint);
        }
    }

    public void IncreaseCustomExp(int _iValue)
    {
        sCampStat.iExpClickCount += 1;
        sCampStat.iCampCrtExp += _iValue;

        if (sCampStat.iCampCrtExp >= sCampStat.iCampNeedExp)
        {
            CampLevelUp(sCampStat.iCampCrtExp - sCampStat.iCampNeedExp);
        }

        UpdateCampInfo();

        InGameManager.instance.GetPlayerCamp().AbleExpUp(sCampStat.iExpIncreasePoint);
    }

    public void CampLevelUp(int _iRemainderExp)
    {
        sCampStat.iCampLevel += 1;
        sCampStat.iCampCrtExp = _iRemainderExp;
        sCampStat.iExpClickCount = 0;
        sCampStat.iCampNeedExp = listCampNeedExp[sCampStat.iCampLevel - 1];
        sCampStat.iMaxUnitBatch += 1;
    }

    public void ResetPlacedUnitsStatus()
    {
        foreach (var item in listOwnHero)
        {
            item.SetupStat();
            item.hud.updateHUD(false);
        }
    }

    public void ResetEnemyUnitsStatus()
    {
        foreach (var item in InGameManager.instance.stageController.stageHerolist)
        {
            item.SetupStat();
            item.hud.updateHUD(false);
        }
    }

    public List<Unit> ownUnits { get => listOwnUnit; }
    public List<Hero> ownHeros { get => listOwnHero; }
    public List<Hero> waitingHeros { get => listWaitingHero; }
    public List<Unit> placedUnits { get => listPlacedUnit; }
    public List<Hero> placedHeros { get => listPlacedHero; }
    public CampStatus campStat { get => sCampStat; set => sCampStat = value; }
}
