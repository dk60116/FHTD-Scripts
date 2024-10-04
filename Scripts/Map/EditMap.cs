using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EditMap : MonoBehaviour
{
    [SerializeField]
    private Unit cBlock;

    private bool bIsSelect, bIsCatch, bVoid, bSell;

    [SerializeField, ReadOnlyInspector]
    private Tile cStayTile, cTempTile, cSelectedTile;
    [SerializeField, ReadOnlyInspector]
    private Unit cCurrentSelectedUnit;

    private Ray rayMouseHit_Tile;
    private RaycastHit rayHit_Tile;
    private int iLayerMask_Tile;

    private List<Tile> listAllTiles;
    [SerializeField]
    private Transform tfSlotTransform;

    private Vector2 v2ClickPoint;

    [SerializeField]
    private RectTransform tfSellPanel;

    [SerializeField]
    private Transform tfUnitsButtonHolder;

    [SerializeField, ReadOnlyInspector]
    private Transform tfStageUnitsParent;

    [SerializeField]
    private GraphicRaycaster cGr;

    void Awake()
    {
        iLayerMask_Tile = 1 << LayerMask.NameToLayer("Tile");
        listAllTiles = FindObjectsOfType<Tile>().ToList();
        bVoid = true;
        tfSellPanel.gameObject.SetActive(false);
    }

    void Start()
    {
        InGameManager.instance.cAStar.PathEvent();

        for (int i = 0; i < tfUnitsButtonHolder.childCount; i++)
        {
            int _iIndex = i;
            tfUnitsButtonHolder.GetChild(i).GetComponent<Button>().onClick.AddListener(() => CreateChampionRandom(_iIndex));
            tfUnitsButtonHolder.GetChild(i).GetComponentInChildren<Text>().text = GameManager.instance.heroData[i].unitName;

            switch (GameManager.instance.heroData[i].unitStat.eRank)
            {
                case UnitRank.None:
                    break;
                case UnitRank.Common:
                    tfUnitsButtonHolder.GetChild(i).GetComponent<Image>().color = new Color(0.8f, 0.8f, 0.9f);
                    break;
                case UnitRank.UnCommon:
                    tfUnitsButtonHolder.GetChild(i).GetComponent<Image>().color = new Color(0.6f, 0.8f, 0.65f);
                    break;
                case UnitRank.Rare:
                    tfUnitsButtonHolder.GetChild(i).GetComponent<Image>().color = new Color(0.6f, 0.65f, 0.8f);
                    break;
                case UnitRank.Epic:
                    tfUnitsButtonHolder.GetChild(i).GetComponent<Image>().color = new Color(0.7f, 0.6f, 0.8f);
                    break;
                case UnitRank.Legendary:
                    tfUnitsButtonHolder.GetChild(i).GetComponent<Image>().color = new Color(0.8f, 0.8f, 0.2f);
                    break;
                default:
                    break;
            }
        }
    }

    void Update()
    {
        CatchUnit();
        MoveUnit();
    }

    [ContextMenu("Init")]
    private void Init()
    {
        tfStageUnitsParent = GameObject.Find("Attack Units").transform;
    }

    public void CreateBlockObstacle()
    {
        int _iCount = 0;

        while (true)
        {
            _iCount++;

            if (_iCount > 1000)
                return;

            Tile _cTile = InGameManager.instance.cAStar.GetTile((int)Random.Range(0, InGameManager.instance.cAStar.sizeX), (int)Random.Range(0, InGameManager.instance.cAStar.sizeY));

            if (_cTile.isUnit)
                continue;
            else
            {
                Unit _cBlock = Instantiate(cBlock);
                _cTile.SetNewUnit(_cBlock);
                InGameManager.instance.cAStar.PathEvent();
                break;
            }
        }
    }

    public Unit CreateChampionWithId(int _iId)
    {
        Unit _cUnit = Instantiate(GameManager.instance.GetHeroWithId(_iId), tfStageUnitsParent).GetComponent<Unit>();

        return _cUnit;
    }

    public void CreateChampionRandom(int _iIndex)
    {
        int _iCount = 0;

        while (true)
        {
            _iCount++;

            if (_iCount > 1000)
                return;

            Tile _cTile = InGameManager.instance.cAStar.GetTile(Random.Range(0, InGameManager.instance.cAStar.sizeX), Random.Range(0, InGameManager.instance.cAStar.sizeY));

            if (_cTile.isUnit)
                continue;
            else
            {
                Unit _cUnit = Instantiate(GameManager.instance.heroData[_iIndex]).GetComponent<Unit>();
                if (InGameManager.instance.cPlayerController.placedUnits.Count >= InGameManager.instance.tileList.Count - 1)
                {
                    if (_cUnit.GetComponent<SpawnPartnerUnit>() != null)
                    {
                        GameManager.instance.OpenNoticePanel("No tile space.");
                        Destroy(_cUnit.gameObject);
                    }
                    else
                        _cTile.SetNewUnit(_cUnit);
                }
                else
                    _cTile.SetNewUnit(_cUnit);
                InGameManager.instance.cAStar.PathEvent();
                break;
            }
        }

        GameManager.instance.cSynergySystem.UpdatePlayerInfo();
        InGameManager.instance.cPlayerController.UpdateCampInfo();
    }

    public void ClearHero()
    {
        foreach (var item in listAllTiles)
        {
            if (item.isUnit)
            {
                if (!item.placedUnit.canNotSell)
                    item.SellUnit(false);
            }
        }

        foreach (var item in GameObject.FindGameObjectsWithTag("HeroWeapon"))
        {
            item.GetComponent<PoolableObject>().ReturnObj();
        }

        InGameManager.instance.cAStar.PathEvent();
    }

    public void CatchUnit()
    {
        if (Physics.Raycast(rayMouseHit_Tile, out rayHit_Tile, 100f, iLayerMask_Tile))
        {
            cStayTile = rayHit_Tile.transform.GetComponent<Tile>();
        }

        var _ped = new PointerEventData(null);
        _ped.position = Input.mousePosition;
        List<RaycastResult> _results = new List<RaycastResult>();
        cGr.Raycast(_ped, _results);

        bool _bOverUI = _results.Count > 0;

        rayMouseHit_Tile = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (!_bOverUI && Physics.Raycast(rayMouseHit_Tile, out rayHit_Tile, 100f, iLayerMask_Tile))
        {
            bVoid = rayHit_Tile.transform.CompareTag("Void");

            if (Input.GetMouseButtonDown(0))
            {
                InGameManager.instance.cEnemyCampInfoPanel.gameObject.SetActive(false);

                if (bVoid)
                {
                    InGameManager.instance.cDealParserPanel.OnOffPanel(false);
                    if (cSelectedTile != null && cSelectedTile.isUnit)
                    {
                        if (cSelectedTile.placedUnit is Hero)
                            InGameManager.instance.cHeroInfoPanel.OnOffPanel(false);
                    }
                }
            }

            Tile _cTile = null;

            if (rayHit_Tile.transform.CompareTag("Tile"))
            {
                _cTile = rayHit_Tile.transform.GetComponent<Tile>();

                if (Input.GetMouseButtonDown(0))
                {
                    cSelectedTile = _cTile;

                    if (cSelectedTile.isUnit && cSelectedTile.placedUnit is Hero)
                    {
                        InGameManager.instance.cHeroInfoPanel.UpdateInfo(cSelectedTile.placedUnit as Hero);
                        InGameManager.instance.cHeroInfoPanel.OnOffPanel(true);
                    }
                    else
                        InGameManager.instance.cHeroInfoPanel.OnOffPanel(false);

                    if (_cTile.isUnit && !_cTile.placedUnit.canNotMove)
                    {
                        v2ClickPoint = Input.mousePosition;
                        bIsSelect = true;
                        cTempTile = _cTile;
                        cCurrentSelectedUnit = _cTile.placedUnit;
                        SwitchAllUnitsOutline(false);
                        cCurrentSelectedUnit.SwitchOutLine(true);
                        if (cCurrentSelectedUnit.friendUnit != null)
                        {
                            cCurrentSelectedUnit.friendUnit.ChangeOutlineColor(Color.green);
                            cCurrentSelectedUnit.friendUnit.SwitchOutLine(true);
                        }

                        tfSlotTransform.gameObject.SetActive(!cCurrentSelectedUnit.canNotKeepSlot);

                        if (cCurrentSelectedUnit is Hero)
                            (cCurrentSelectedUnit as Hero).OnOffRangeCircle(true);
                    }
                }
            }

            SpotCurrentTile(_cTile);
        }
    }

    private void MoveUnit()
    {
        if (cCurrentSelectedUnit != null)
        {
            if (bVoid)
                cCurrentSelectedUnit.ChangeOutlineColor(Color.red);
            else if (InGameManager.instance.crtOverTile != null && InGameManager.instance.crtOverTile.isUnit)
            {
                if (cCurrentSelectedUnit.currentTile.bSlot && InGameManager.instance.crtOverTile.placedUnit.canNotMove)
                    cCurrentSelectedUnit.ChangeOutlineColor(Color.red);
                else if (cCurrentSelectedUnit.canNotKeepSlot && InGameManager.instance.crtOverTile.bSlot)
                    cCurrentSelectedUnit.ChangeOutlineColor(Color.red);
                else if (cCurrentSelectedUnit.currentTile.bSlot && InGameManager.instance.crtOverTile.placedUnit.canNotKeepSlot)
                    cCurrentSelectedUnit.ChangeOutlineColor(Color.red);
                else
                    cCurrentSelectedUnit.ChangeOutlineColor(Color.green);
            }
            else
                cCurrentSelectedUnit.ChangeOutlineColor(Color.green);
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (rayHit_Tile.transform != null)
            {
                if (rayHit_Tile.transform.CompareTag("Tile"))
                {
                    if (cCurrentSelectedUnit != null)
                        cCurrentSelectedUnit.ChangeOutlineColor(Color.green);
                    else
                        InGameManager.instance.cDealParserPanel.OnOffPanel(false);

                    if (!rayHit_Tile.transform.GetComponent<Tile>().isUnit)
                    {
                        SwitchAllUnitsOutline(false);
                        SwitchAllUnitsOutlineWait(false);
                    }
                    else if (cSelectedTile != null && cSelectedTile.placedUnit is EnemyCamp)
                    {
                        InGameManager.instance.cEnemyCampInfoPanel.OnOffPanel(true);
                    }
                }

                if (rayHit_Tile.transform.CompareTag("Void"))
                    SwitchAllUnitsOutline(false);
            }
        }

        if (bIsSelect)
        {
            if (Vector2.Distance(v2ClickPoint, Input.mousePosition) > (Screen.width + Screen.height) * 0.02f)
                bIsCatch = true;

            if (bIsCatch)
            {
                cCurrentSelectedUnit.transform.position = new Vector3(rayHit_Tile.point.x, 1f, rayHit_Tile.point.z);
                InGameManager.instance.cAStar.OnOffPathLine(!(cCurrentSelectedUnit is EnemyCamp || cCurrentSelectedUnit is PlayerCamp));
                InGameManager.instance.cDealParserPanel.OnOffPanel(false);
                tfSellPanel.gameObject.SetActive(true);
                if (cSelectedTile.isUnit && cSelectedTile.placedUnit is Hero)
                    InGameManager.instance.cHeroInfoPanel.OnOffPanel(false);
            }

            SwitchBlockLine(bIsCatch);

            if (Input.GetMouseButtonUp(0))
            {
                tfSellPanel.gameObject.SetActive(false);
                tfSellPanel.GetChild(3).gameObject.SetActive(false);

                if (bIsCatch)
                {
                    if (rayHit_Tile.transform.CompareTag("Tile"))
                    {
                        Tile _cTile = rayHit_Tile.transform.GetComponent<Tile>();

                        if (!_cTile.isUnit)
                        {
                            if (InGameManager.instance.cPlayerController.placedUnits.Count >= InGameManager.instance.tileList.Count - 1 && !cStayTile.bSlot)
                            {
                                if (cTempTile.placedUnit.friendUnit != null)
                                {
                                    cTempTile.placedUnit.transform.position = cTempTile.tilePos;
                                    cTempTile.SetUnit(cTempTile.placedUnit);
                                    GameManager.instance.OpenNoticePanel("No tile space.");
                                }
                            }
                            else
                                _cTile.MoveUnit(cTempTile);
                        }
                        else
                        {
                            if (!(_cTile.placedUnit.canNotKeepSlot && cTempTile.bSlot))
                            {
                                _cTile.SwitchUnit(cTempTile);
                                _cTile.placedUnit.SwitchOutLine(false);
                            }
                            else
                            {
                                cTempTile.MoveUnit(cTempTile, false);
                            }

                            if (_cTile.placedUnit is Hero)
                                (_cTile.placedUnit as Hero).OnOffRangeCircle(false);
                            cCurrentSelectedUnit.OnOffRangeCircle(false);
                            cCurrentSelectedUnit.SwitchOutLine(false);
                            if (cCurrentSelectedUnit.friendUnit != null)
                                cCurrentSelectedUnit.friendUnit.SwitchOutLine(false);
                        }
                    }
                    else
                    {
                        cTempTile.MoveUnit(cTempTile, false);
                        cTempTile.placedUnit.SwitchOutLine(!bVoid);
                    }

                    if (bSell)
                        SellUnit();
                }

                bIsSelect = false;
                bIsCatch = false;

                cCurrentSelectedUnit = null;
                SwitchBlockLine(false);
                InGameManager.instance.cAStar.PathEvent();
                tfSlotTransform.gameObject.SetActive(true);
                InGameManager.instance.cAStar.OnOffPathLine(true);
            }
        }
    }

    private void SwitchAllUnitsOutline(bool _bValue)
    {
        var tilesWithUnits = listAllTiles.Where(item => item.isUnit);
        foreach (var item in tilesWithUnits)
        {
            item.placedUnit.SwitchOutLine(_bValue);
            if (item.placedUnit is Hero)
                (item.placedUnit as Hero).OnOffRangeCircle(_bValue);
        }
    }

    private void SwitchAllUnitsOutlineWait(bool _bValue)
    {
        StartCoroutine(SwitchAllUnitsOutlineWaitCoroutine(_bValue));
    }

    IEnumerator SwitchAllUnitsOutlineWaitCoroutine(bool _bValue)
    {
        yield return null;

        foreach (var item in listAllTiles)
        {
            if (item.isUnit)
            {
                item.placedUnit.SwitchOutLine(_bValue);
                if (item.placedUnit is Hero)
                    (item.placedUnit as Hero).OnOffRangeCircle(_bValue);
            }
        }
    }

    private void SwitchBlockLine(bool _bIsOn)
    {
        foreach (var item in listAllTiles)
            item.SwitchLine(_bIsOn);
    }

    private void SpotCurrentTile(Tile _cTile)
    {
        foreach (var item in listAllTiles)
        {
            item.SpotLine(false);

            if (item == cSelectedTile)
                item.SpotLine(true, true);
        }

        if (cTempTile == null)
            return;

        if (_cTile != null && _cTile.isUnit)
        {
            if (cTempTile.bSlot && _cTile.placedUnit.canNotKeepSlot)
                return;
        }
        
        if (_cTile != null)
            _cTile.SpotLine(true);

        if (bVoid)
            AllOffSpot();
    }

    private void AllOffSpot()
    {
        foreach (var item in listAllTiles)
        {
            item.SpotLine(false);

            if (item == cSelectedTile)
                item.SpotLine(true, true);
        }
    }

    public void SetSellHeroBoolean(bool _bOn)
    {
        PointerUp(_bOn && !cCurrentSelectedUnit.canNotSell);
        tfSellPanel.GetChild(0).gameObject.SetActive(_bOn && !cCurrentSelectedUnit.canNotSell);

        if (cCurrentSelectedUnit is Hero)
        {
            tfSellPanel.GetChild(1).gameObject.SetActive(!_bOn);
            tfSellPanel.GetChild(2).gameObject.SetActive(_bOn);
            tfSellPanel.GetChild(3).gameObject.SetActive(_bOn);
            tfSellPanel.GetChild(3).GetComponent<TextMeshProUGUI>().text = $"+{(cTempTile.placedUnit as Hero).heroCost}";

            (cCurrentSelectedUnit as Hero).AppearBody(!_bOn);
            (cTempTile.placedUnit as Hero).block.gameObject.SetActive(!_bOn);
            (cTempTile.placedUnit as Hero).hud.SwitchHud(!_bOn);
            if (cCurrentSelectedUnit is Hero)
                (cCurrentSelectedUnit as Hero).OnOffRangeCircle(!_bOn);
        }
    }

    public void SellUnit()
    {
        cTempTile.SellUnit();
        bSell = false;
        tfSellPanel.GetChild(0).gameObject.SetActive(false);
        tfSellPanel.GetChild(1).gameObject.SetActive(true);
        tfSellPanel.GetChild(2).gameObject.SetActive(false);
        tfSellPanel.GetChild(3).gameObject.SetActive(false);
    }

    public void PointerUp(bool _bOn)
    {
        StartCoroutine(PointerUpCoroutine(_bOn));
    }

    WaitForEndOfFrame cWaitFrame = new WaitForEndOfFrame();

    IEnumerator PointerUpCoroutine(bool _bOn)
    {
        yield return cWaitFrame;

        bSell = _bOn;
    }

    public bool isCatch { get => bIsSelect; }
    public Tile stayTile { get => cStayTile; }
    public Tile tempTile { get => cTempTile; }
    public Unit selectedUnit { get => cCurrentSelectedUnit; }
}
