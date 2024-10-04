using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageController : MonoBehaviour
{
    [SerializeField, ReadOnlyInspector]
    private EnemyCampInfoPanel cEnemyInfoPanel;

    [SerializeField]
    private ChapterStatus sStageStat;

    [SerializeField, ReadOnlyInspector]
    private List<Hero> listStageHero;

    [SerializeField, ReadOnlyInspector]
    private Transform tfStageUnitsHolder;

    [SerializeField, ReadOnlyInspector]
    private bool bIsWinningStreak, bIsWin;
    [SerializeField, ReadOnlyInspector]
    private int iWinningStreak, iLosingStreak;
    [SerializeField, ReadOnlyInspector]
    private int iInterstGold;

    [SerializeField, ReadOnlyInspector]
    private List<Tile> listHumanlBuffTile;
    [SerializeField, ReadOnlyInspector]
    private List<int> listHumanBuffType;

    void Awake()
    {
        listHumanlBuffTile = new List<Tile>();
        listHumanBuffType = new List<int>();
    }

    void Start()
    {
        sStageStat = GameManager.instance.chapterList[GameManager.instance.selectedChapter];
        sStageStat.iCrtStage = 1;

        //Invoke(nameof(UpdateStageUnits), 0.1f);

        bIsWin = true;
        bIsWinningStreak = true;

        SpawnHumanBuff(true);
    }

    [ContextMenu("Init")]
    private void Init()
    {
        cEnemyInfoPanel = FindObjectOfType<EnemyCampInfoPanel>(true);
        tfStageUnitsHolder = GameObject.Find("Attack Units").transform;
    }

    public void NextStage()
    {
        bIsWin = true;
        //StartCoroutine(NextStageCoroutine());
    }

    IEnumerator NextStageCoroutine()
    {
        yield return null;

        sStageStat.iCrtStage++;

        InGameManager.instance.cAStar.SwitchPathLine(true);

        JudgementWinOrLose();
        UpdateStageUnits();

        if (!InGameManager.instance.cMarketSystem.IsRerollLock())
            InGameManager.instance.cMarketSystem.Reroll();

        Debug.LogError("Next");

        IncreaseStageResources(sStageStat.iCrtStage);

        InGameManager.instance.cPlayerController.UpdateCampInfo();
        
        SpawnHumanBuff(true);
    }

    public void SpawnHumanBuff(bool _bRandom)
    {
        foreach (var item in InGameManager.instance.tileList)
            item.OffAllBuffEffect();

        if (_bRandom)
            ChangeHumanBuffList();

        int _iSynergyCount = GameManager.instance.cSynergySystem.GetOpenSynergyCount(GameManager.instance.cSynergySystem.GetNumberWithTribe(HeroTribe.Human), false);

        if (_iSynergyCount > 0)
            _iSynergyCount = (int)GameManager.instance.cSynergySystem.synergyStatusList[GameManager.instance.cSynergySystem.GetNumberWithTribe(HeroTribe.Human)].listValue[0].value[_iSynergyCount - 1];

        for (int i = 0; i < _iSynergyCount; i++)
            listHumanlBuffTile[i].OpenFieldBuffEffect(true, (TileBuff)listHumanBuffType[i]);
    }

    public void ChangeHumanBuffList()
    {
        listHumanlBuffTile.Clear();
        listHumanBuffType.Clear();

        List<Tile> _list = Tools.ShuffleList(InGameManager.instance.tileList);

        for (int i = 0; i < GameManager.instance.cSynergySystem.synergyStatusList[GameManager.instance.cSynergySystem.GetNumberWithTribe(HeroTribe.Human)].listValue[0].value[2]; i++)
        {
            listHumanlBuffTile.Add(_list[i]);
            listHumanBuffType.Add(Random.Range(0, 3));
        }
    }

    private void IncreaseStageResources(int _iStage)
    {
        CampStatus _sCampStat = InGameManager.instance.cPlayerController.campStat;

        int _iStreakGold = 0;

        if (bIsWinningStreak)
        {
            switch (iWinningStreak)
            {
                case 0:
                case 1:
                    _iStreakGold = 0;
                    break;
                case 2:
                case 3:
                    _iStreakGold = 1;
                    break;
                case 4:
                    _iStreakGold = 2;
                    break;
                default:
                    _iStreakGold = 3;
                    break;
            }
        }

        if (!bIsWinningStreak)
        {
            switch (iLosingStreak)
            {
                case 0:
                case 1:
                    _iStreakGold = 0;
                    break;
                case 2:
                case 3:
                    _iStreakGold = 1;
                    break;
                case 4:
                    _iStreakGold = 2;
                    break;
                default:
                    _iStreakGold = 3;
                    break;
            }
        }

        _sCampStat.iGold += _iStage + + GetInterstGold() + _iStreakGold + 1;
        InGameManager.instance.cPlayerController.campStat = _sCampStat;
        InGameManager.instance.cPlayerController.IncreaseCustomExp(4);
    }

    public void UpdateStageUnits()
    {
        cEnemyInfoPanel.UpdateEnemyCampInfo(GameManager.instance.selectedChapter, sStageStat.iCrtStage);
        listStageHero = new List<Hero>();

        //for (int i = 0; i < tfStageUnitsHolder.childCount; i++)
        //{
        //    listStageHero.Add(tfStageUnitsHolder.GetChild(i).GetComponent<Hero>());
        //    listStageHero[i].gameObject.SetActive(true);
        //    listStageHero[i].attackUnit = true;
        //    listStageHero[i].CreateHUD();
        //    GameManager.instance
        //    listStageHero[i].gameObject.SetActive(false);
        //}

        for (int i = 0; i < tfStageUnitsHolder.childCount; i++)
            Destroy(tfStageUnitsHolder.GetChild(i).gameObject);

        for (int i = 0; i < GameManager.instance.chapterList[GameManager.instance.selectedChapter].listStageUnits[sStageStat.iCrtStage - 1].listId.Count; i++)
        {
            Unit _newUnit = InGameManager.instance.cEditMap.CreateChampionWithId(GameManager.instance.chapterList[GameManager.instance.selectedChapter].listStageUnits[sStageStat.iCrtStage - 1].listId[i]);
            _newUnit.attackUnit = true;
            _newUnit.body.transform.position += Vector3.down * 0.1f;
            (_newUnit as Hero).SetLevel(GameManager.instance.chapterList[GameManager.instance.selectedChapter].listStageUnits[sStageStat.iCrtStage - 1].listLevel[i]);
            (_newUnit as Hero).CreateHUD();
            listStageHero.Add(_newUnit as Hero);
        }

        InGameManager.instance.cUnitController.ResetStageUnits();
        GameManager.instance.cSynergySystem.UpdateEnemyInfo();

        for (int i = 0; i < listStageHero.Count; i++)
            cEnemyInfoPanel.unitIconList[i].SetUnit(listStageHero[i]);
    }

    private void JudgementWinOrLose()
    {
        if (bIsWinningStreak)
        {
            if (bIsWin)
                iWinningStreak++;
            else
            {
                bIsWinningStreak = false;
                iWinningStreak = 0;
                iLosingStreak = 1;
            }    
        }
        else
        {
            if (!bIsWin)
                iLosingStreak++;
            else
            {
                bIsWinningStreak = true;
                iWinningStreak = 1;
                iLosingStreak = 0;
            }
        }

        InGameManager.instance.cPlayerInfoUI.UpdateWinningStreakText(bIsWinningStreak, bIsWinningStreak ? iWinningStreak : iLosingStreak);
    }

    public int ActivateUnitsCount()
    {
        int _iCount = 0;

        for (int i = 0; i < listStageHero.Count; i++)
        {
            if (listStageHero[i].gameObject.activeSelf)
                _iCount++;
        }

        return _iCount;
    }

    public void CheckComplete()
    {
        if (ActivateUnitsCount() <= 0)
        {
            NextStage();
        }
    }

    public int GetInterstGold()
    {
        iInterstGold = (int)(InGameManager.instance.cPlayerController.campStat.iGold * 0.1f);
        iInterstGold = Mathf.Clamp(iInterstGold, 0, 5);
        return iInterstGold;
    }

    public ChapterStatus stat { get => sStageStat; }
    public List<Hero> stageHerolist { get => listStageHero; }
    public bool isWin { get => bIsWin; set => bIsWin = false; }
}
