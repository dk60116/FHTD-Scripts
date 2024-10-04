using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;

public enum StageStatus { Waiting, Running}

public class InGameManager : MonoBehaviour
{
    static public InGameManager instance;

    private Tile[,] cTiles;
    [ReadOnlyInspector]
    public AStar cAStar;
    [ReadOnlyInspector]
    public MapBuilder cMapBuilder;
    [ReadOnlyInspector]
    public EditMap cEditMap;
    [ReadOnlyInspector]
    public UnitController cUnitController;
    [ReadOnlyInspector]
    public Canvas uiMainCanvas, uiHudCanvas;
    [ReadOnlyInspector]
    public CanvasScaler uiHudCanvasScaler;
    [SerializeField, ReadOnlyInspector]
    private StageController cStageController;
    [ReadOnlyInspector]
    public PlayerController cPlayerController;
    [ReadOnlyInspector]
    public HeroMarketSystem cMarketSystem;
    [ReadOnlyInspector]
    public PlayerInfoUI cPlayerInfoUI;
    [ReadOnlyInspector]
    public InGameCardSysyem cCardSystem;
    [ReadOnlyInspector]
    public CardController cCardController;
    [ReadOnlyInspector]
    public HeroInfoPanel cHeroInfoPanel;
    [ReadOnlyInspector]
    public EnemyCampInfoPanel cEnemyCampInfoPanel;
    [ReadOnlyInspector]
    public DealParserPanel cDealParserPanel;

    [ReadOnlyInspector]
    public bool bIsRoundStart;

    [SerializeField, ReadOnlyInspector]
    private List<Tile> listFieldTile;

    [SerializeField]
    private Transform tfSlotTileHolder;
    [SerializeField, ReadOnlyInspector]
    private List<Tile> listSlotTile;

    [SerializeField, ReadOnlyInspector]
    private bool bCanLevelUp;
    [SerializeField, ReadOnlyInspector]
    private Sprite[] imgEmblemList;

    [SerializeField]
    private UnitHUD cHud_Def_Prefab, cHud_Atk_Prfeab;

    [SerializeField, ReadOnlyInspector]
    private StageStatus eStatus;

    [SerializeField]
    public RectTransform tfSynergyCountSlot_P, tfSynergyCountSlot_E;


    [SerializeField]
    public Transform tfTempUnitHolder;

    [SerializeField]
    private 

    void Reset()
    {
        Init();
    }

    void Awake()
    {
        cTiles = new Tile[cMapBuilder.v2MapSize.x, cMapBuilder.v2MapSize.y];

        instance = this;

        listFieldTile = new List<Tile>();

        for (int i = 0; i < cMapBuilder.tileHolder.childCount; i++)
            listFieldTile.Add(cMapBuilder.tileHolder.GetChild(i).GetComponent<Tile>());

        listSlotTile = new List<Tile>();

        for (int i = 0; i < tfSlotTileHolder.childCount; i++)
            listSlotTile.Add(tfSlotTileHolder.GetChild(i).GetComponent<Tile>());

        bCanLevelUp = true;

        cEnemyCampInfoPanel.OnOffPanel(false);

        SetStatus(StageStatus.Waiting);
    }

    void Start()
    {
        GameManager.instance.cSynergySystem.InGameInit();
        cDealParserPanel.OnOffPanel(false);
    }

    [ContextMenu("Init")]
    private void Init()
    {
        cAStar = FindObjectOfType<AStar>(true);
        cMapBuilder = FindObjectOfType<MapBuilder>(true);
        cEditMap = FindObjectOfType<EditMap>(true);
        cUnitController = FindObjectOfType<UnitController>(true);
        cStageController = FindObjectOfType<StageController>(true);
        cPlayerController = FindObjectOfType<PlayerController>(true);
        uiMainCanvas = GameObject.Find("Main Canvas").GetComponent<Canvas>();
        uiHudCanvas = GameObject.Find("HUD Canvas").GetComponent<Canvas>();
        cPlayerInfoUI = FindObjectOfType<PlayerInfoUI>(true);
        cDealParserPanel = FindObjectOfType<DealParserPanel>(true);
        uiHudCanvasScaler = uiHudCanvas.GetComponent<CanvasScaler>();
        tfSlotTileHolder = GameObject.Find("Slot Holder").transform;
        cMarketSystem = FindObjectOfType<HeroMarketSystem>(true);
        cCardSystem = FindObjectOfType<InGameCardSysyem>();
        cCardController = FindObjectOfType<CardController>(true);
        cHeroInfoPanel = FindObjectOfType<HeroInfoPanel>(true);
        cEnemyCampInfoPanel = FindObjectOfType<EnemyCampInfoPanel>(true);
        imgEmblemList = Resources.LoadAll<Sprite>($"GUI/HUD/TribeEmblem");
    }

    public void SetTile(int _x, int _y, Tile _cTile)
    {
        tiles[_x, _y] = _cTile;
    }

    public Tile GetTile(int _x, int _y)
    {
        return tiles[_x, _y];
    }

    public void SetCanLevelUp(bool _bValue)
    {
        bCanLevelUp = _bValue;
    }

    public Sprite GetEmblemSprite(HeroTribe? _eTribe, HeroClass? _cClass)
    {
        if (_eTribe != null)
            return imgEmblemList[(int)_eTribe - 1];
        else
            return imgEmblemList[(int)_cClass + 10];
    }

    public PlayerCamp GetPlayerCamp()
    {
        return FindObjectOfType<PlayerCamp>();
    }

    public EnemyCamp GetEnemyCamp()
    {
        return FindObjectOfType<EnemyCamp>();
    }

    public int GetSlotUnitCount()
    {
        int _iSlotUnitCount = 0;

        for (int i = 0; i < listSlotTile.Count; i++)
        {
            if (listSlotTile[i].isUnit)
                _iSlotUnitCount++;
        }

        return _iSlotUnitCount;
    }

    public bool CheckNotSlotFull()
    {
        bool _bResult = GetSlotUnitCount() < listSlotTile.Count;

        if (!_bResult)
            GameManager.instance.OpenNoticePanel("Hero slots are full");

        return _bResult;
    }

    public Tile GetEmptyTile()
    {
        foreach (var item in listFieldTile)
        {
            if (!item.isUnit)
                return item;
        }

        return null;
    }

    public Tile GetEmptySlotTile()
    {
        foreach (var item in listSlotTile)
        {
            if (!item.isUnit)
                return item;
        }

        return null;
    }


    public void SetStatus(StageStatus _eStatus)
    {
        eStatus = _eStatus;
    }

    public Tile[,] tiles { get => cTiles; }
    public List<Tile> tileList { get => listFieldTile; }
    public List<Tile> slotTileList { get => listSlotTile; }
    public Tile crtOverTile { get => cEditMap.stayTile;}
    public bool canLevelUp { get => bCanLevelUp; }
    public UnitHUD hud_Def { get => cHud_Def_Prefab; }
    public UnitHUD hud_Atk { get => cHud_Atk_Prfeab; }
    public StageController stageController { get => cStageController; }
    public StageStatus status { get => eStatus; }
}
