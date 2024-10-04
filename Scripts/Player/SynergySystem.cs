using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class SynergySystem : MonoBehaviour
{
    [SerializeField, ReadOnlyInspector]
    private RectTransform tfSynergyCountPanel_Player, tfSynergyCountPanel_Enemy;
    [SerializeField, ReadOnlyInspector]
    private List<SynergyCountSlot> listCountSlot_Player, listCountSlot_Enemy;

    [SerializeField]
    private List<SynergyStatus> listSynergyStat;
    [SerializeField, ReadOnlyInspector]
    private SynergyActivate cActivate;
    [SerializeField, ReadOnlyInspector]
    private SynergyDescriptionPanel cDescriptionPanel;

    void Awake()
    {
        if (cDescriptionPanel != null)
            cDescriptionPanel.gameObject.SetActive(false);
    }

    void Start()
    {
        UpdatePlayerInfo();
    }

    [ContextMenu("Init")]
    private void Init()
    {
        cDescriptionPanel = FindObjectOfType<SynergyDescriptionPanel>(true);
    }

    public void InGameInit()
    {
        tfSynergyCountPanel_Player = InGameManager.instance.tfSynergyCountSlot_P;
        tfSynergyCountPanel_Enemy = InGameManager.instance.tfSynergyCountSlot_E;

        for (int i = 0; i < tfSynergyCountPanel_Player.childCount; i++)
        {
            tfSynergyCountPanel_Player.GetChild(i).gameObject.SetActive(false);
            tfSynergyCountPanel_Enemy.GetChild(i).gameObject.SetActive(false);
        }

        listCountSlot_Player = new List<SynergyCountSlot>();

        for (int i = 0; i < tfSynergyCountPanel_Player.childCount; i++)
        {
            listCountSlot_Player.Add(tfSynergyCountPanel_Player.GetChild(i).GetComponent<SynergyCountSlot>());
            listCountSlot_Enemy.Add(tfSynergyCountPanel_Enemy.GetChild(i).GetComponent<SynergyCountSlot>());
        }
    }

    public HeroTribe GetTribeWithNumber(int _iNumber)
    {
        HeroTribe _eResult = HeroTribe.None;

        foreach (var item in synergyStatusList)
        {
            if (item.iNumber == _iNumber)
                Enum.TryParse(item.strSynergy, out _eResult);
        }

        return _eResult;
    }

    public HeroClass GetClassWithNumber(int _iNumber)
    {
        HeroClass _eResult = HeroClass.None;

        foreach (var item in synergyStatusList)
        {
            if (item.iNumber == _iNumber)
                Enum.TryParse(item.strSynergy, out _eResult);
        }

        return _eResult;
    }

    public int GetSynergyCount(HeroTribe _eTribe, bool _bIsEnemy)
    {
        List<Hero> _listPlacedHero = _bIsEnemy ? InGameManager.instance.stageController.stageHerolist : InGameManager.instance.cPlayerController.placedHeros;

        List<Hero> _listTribe = _listPlacedHero.Where(x => x.heroStat.eTribe == _eTribe).ToList();
        List<int> _listTribes = new List<int>();

        for (int i = 0; i < _listTribe.Count; i++)
            _listTribes.Add(_listTribe[i].unitID);

        int _iTribeCount = _listTribes.Distinct().ToList().Count;

        return _iTribeCount;
    }
    
    public int GetNumberWithTribe(HeroTribe _eTribe)
    {
        return (int)_eTribe - 1;
    }

    public int GetNumberWithClass(HeroClass _eClass)
    {
        return (int)_eClass + 10;
    }

    public int GetSynergyCount(int _iSynergy, bool _bIsEnemy)
    {
        if (_iSynergy < 11)
            return GetSynergyCount(GetTribeWithNumber(_iSynergy), _bIsEnemy);
        else
            return GetSynergyCount(GetTribeWithNumber(_iSynergy), _bIsEnemy);
    }

    public int GetSynergyCount(HeroClass _eClass, bool _bIsEnemy)
    {
        List<Hero> _listPlacedHero = _bIsEnemy ? InGameManager.instance.stageController.stageHerolist : InGameManager.instance.cPlayerController.placedHeros;

        List<Hero> _listClass_1 = _listPlacedHero.Where(x => x.heroStat.eClass_1 == _eClass).ToList();
        List<Hero> _listClass_2 = _listPlacedHero.Where(x => x.heroStat.eClass_2 == _eClass).ToList();
        List<int> _listClasses = new List<int>();

        for (int i = 0; i < _listClass_1.Count; i++)
            _listClasses.Add(_listClass_1[i].unitID);

        for (int i = 0; i < _listClass_2.Count; i++)
            _listClasses.Add(_listClass_2[i].unitID);

        int _iClassCount = _listClasses.Distinct().ToList().Count;

        return _iClassCount;
    }

    public bool IsSynergyActive(int _iSynergy, bool _bIsEnemy)
    {
        return GetSynergyCount(_iSynergy, false) >= listSynergyStat[_iSynergy].listCount[0];
    }

    public int GetOpenSynergyCount(int _iSynergy, bool _bIsEnemy)
    {
        if (listSynergyStat[_iSynergy].listCount.Count == 1)
        {
            if (_iSynergy < 11)
            {
                if (GetSynergyCount(GetTribeWithNumber(_iSynergy), _bIsEnemy) >= 1)
                    return 1;
            }
            else
            {
                if (GetSynergyCount(GetClassWithNumber(_iSynergy), _bIsEnemy) >= 1)
                    return 1;
            }
        }

        for (int i = 0; i < listSynergyStat[_iSynergy].listCount.Count; i++)
        {
            if (_iSynergy < 11)
            {
                if (GetSynergyCount(GetTribeWithNumber(_iSynergy), _bIsEnemy) >= listSynergyStat[_iSynergy].listCount[i] && GetSynergyCount(GetTribeWithNumber(_iSynergy), false) < listSynergyStat[_iSynergy].listCount[i + 1])
                    return i + 1;
            }
            else
            {
                if (GetSynergyCount(GetClassWithNumber(_iSynergy), _bIsEnemy) >= listSynergyStat[_iSynergy].listCount[i] && GetSynergyCount(GetClassWithNumber(_iSynergy), false) < listSynergyStat[_iSynergy].listCount[i + 1])
                    return i + 1;
            }
        }

        return 0;
    }

    public void UpdatePlayerInfo()
    {
        if (FindObjectOfType<InGameManager>(true) == null)
            return;

        StartCoroutine(UpdatePlayerInfoCoroutine());
    }

    public void UpdateEnemyInfo()
    {
        StartCoroutine(UpdateEnemyInfoCoroutine());
    }

    WaitForEndOfFrame cWaitFrame = new WaitForEndOfFrame();

    IEnumerator UpdatePlayerInfoCoroutine()
    {
        yield return cWaitFrame;

        UpdatePlayerCount();
    }

    IEnumerator UpdateEnemyInfoCoroutine()
    {
        yield return cWaitFrame;

        UpdateEnemyCount();
    }

    private void UpdatePlayerCount()
    {
        listCountSlot_Player[0].SetInfo(HeroTribe.Human, synergyStatusList[0].listCount, 21);
        listCountSlot_Player[1].SetInfo(HeroTribe.Beast, synergyStatusList[1].listCount, 20);
        listCountSlot_Player[2].SetInfo(HeroTribe.Dragon, synergyStatusList[2].listCount, 19);
        listCountSlot_Player[3].SetInfo(HeroTribe.Dragonoid, synergyStatusList[3].listCount, 18);
        listCountSlot_Player[4].SetInfo(HeroTribe.Elemental, synergyStatusList[4].listCount, 17);
        listCountSlot_Player[5].SetInfo(HeroTribe.Fairy, synergyStatusList[5].listCount, 16);
        listCountSlot_Player[6].SetInfo(HeroTribe.Undead, synergyStatusList[6].listCount, 15);
        listCountSlot_Player[7].SetInfo(HeroTribe.Plant, synergyStatusList[7].listCount, 14);
        listCountSlot_Player[8].SetInfo(HeroTribe.Cavalry, synergyStatusList[8].listCount, 13);
        listCountSlot_Player[9].SetInfo(HeroTribe.Guardian, synergyStatusList[9].listCount, 12);
        listCountSlot_Player[10].SetInfo(HeroTribe.Berserker, synergyStatusList[10].listCount, 11);
        listCountSlot_Player[11].SetInfo(HeroClass.Magician, synergyStatusList[11].listCount, 10);
        listCountSlot_Player[12].SetInfo(HeroClass.Flying, synergyStatusList[12].listCount, 9);
        listCountSlot_Player[13].SetInfo(HeroClass.Armoric, synergyStatusList[13].listCount, 8);
        listCountSlot_Player[14].SetInfo(HeroClass.TwinSwords, synergyStatusList[14].listCount, 7);
        listCountSlot_Player[15].SetInfo(HeroClass.Insect, synergyStatusList[15].listCount, 6);
        listCountSlot_Player[16].SetInfo(HeroClass.Fighter, synergyStatusList[16].listCount, 5);
        listCountSlot_Player[17].SetInfo(HeroClass.Natual, synergyStatusList[17].listCount, 4);
        listCountSlot_Player[18].SetInfo(HeroClass.Swordsman, synergyStatusList[18].listCount, 3);
        listCountSlot_Player[19].SetInfo(HeroClass.BigHorns, synergyStatusList[19].listCount, 2);
        listCountSlot_Player[20].SetInfo(HeroClass.Ranger, synergyStatusList[20].listCount, 1);
        listCountSlot_Player[21].SetInfo(HeroClass.Priest, synergyStatusList[21].listCount, 0);

        cActivate.CheckPlayerSynergy(HeroTribe.Beast, GetSynergyCount(HeroTribe.Beast, false));
        cActivate.CheckPlayerSynergy(HeroClass.Magician, GetSynergyCount(HeroClass.Magician, false));

        StartCoroutine(SortPlayerSlotsCoroutine());
    }

    private void UpdateEnemyCount()
    {
        listCountSlot_Enemy[0].SetInfo(HeroTribe.Human, synergyStatusList[0].listCount, 21);
        listCountSlot_Enemy[1].SetInfo(HeroTribe.Beast, synergyStatusList[1].listCount, 20);
        listCountSlot_Enemy[2].SetInfo(HeroTribe.Dragon, synergyStatusList[2].listCount, 19);
        listCountSlot_Enemy[3].SetInfo(HeroTribe.Dragonoid, synergyStatusList[3].listCount, 18);
        listCountSlot_Enemy[4].SetInfo(HeroTribe.Elemental, synergyStatusList[4].listCount, 17);
        listCountSlot_Enemy[5].SetInfo(HeroTribe.Fairy, synergyStatusList[5].listCount, 16);
        listCountSlot_Enemy[6].SetInfo(HeroTribe.Undead, synergyStatusList[6].listCount, 15);
        listCountSlot_Enemy[7].SetInfo(HeroTribe.Plant, synergyStatusList[7].listCount, 14);
        listCountSlot_Enemy[8].SetInfo(HeroTribe.Cavalry, synergyStatusList[8].listCount, 13);
        listCountSlot_Enemy[9].SetInfo(HeroTribe.Guardian, synergyStatusList[9].listCount, 12);
        listCountSlot_Enemy[10].SetInfo(HeroTribe.Berserker, synergyStatusList[10].listCount, 11);
        listCountSlot_Enemy[11].SetInfo(HeroClass.Magician, synergyStatusList[11].listCount, 10);
        listCountSlot_Enemy[12].SetInfo(HeroClass.Flying, synergyStatusList[12].listCount, 9);
        listCountSlot_Enemy[13].SetInfo(HeroClass.Armoric, synergyStatusList[13].listCount, 8);
        listCountSlot_Enemy[14].SetInfo(HeroClass.TwinSwords, synergyStatusList[14].listCount, 7);
        listCountSlot_Enemy[15].SetInfo(HeroClass.Insect, synergyStatusList[15].listCount, 6);
        listCountSlot_Enemy[16].SetInfo(HeroClass.Fighter, synergyStatusList[16].listCount, 5);
        listCountSlot_Enemy[17].SetInfo(HeroClass.Natual, synergyStatusList[17].listCount, 4);
        listCountSlot_Enemy[18].SetInfo(HeroClass.Swordsman, synergyStatusList[18].listCount, 3);
        listCountSlot_Enemy[19].SetInfo(HeroClass.BigHorns, synergyStatusList[19].listCount, 2);
        listCountSlot_Enemy[20].SetInfo(HeroClass.Ranger, synergyStatusList[20].listCount, 1);
        listCountSlot_Enemy[21].SetInfo(HeroClass.Priest, synergyStatusList[21].listCount, 0);

        StartCoroutine(SortEnemySlotsCoroutine());
    }

    private WaitForEndOfFrame cNewWaitFrame;

    IEnumerator SortPlayerSlotsCoroutine()
    {
        yield return cNewWaitFrame;

        listCountSlot_Player = listCountSlot_Player.OrderBy(x => x.tearMax).ThenBy(x => x.synergyMax).ThenBy(x => x.count).ThenBy(x => x.number).ToList();

        for (int i = 0; i < listCountSlot_Player.Count; i++)
            listCountSlot_Player[i].transform.SetSiblingIndex(i);
    }

    IEnumerator SortEnemySlotsCoroutine()
    {
        yield return cNewWaitFrame;

        listCountSlot_Enemy = listCountSlot_Enemy.OrderBy(x => x.tearMax).ThenBy(x => x.synergyMax).ThenBy(x => x.count).ThenBy(x => x.number).ToList();

        for (int i = 0; i < listCountSlot_Player.Count; i++)
            listCountSlot_Enemy[i].transform.SetSiblingIndex(i);
    }

    public List<SynergyStatus> synergyStatusList { get => listSynergyStat; set => listSynergyStat = value; }
    public SynergyDescriptionPanel synergyDescriptionPanel { get => cDescriptionPanel; }
}
