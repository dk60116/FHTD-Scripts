using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum TileBuff { Attack, AbilPower, AtkSpd, Count}

public class Tile : MonoBehaviour
{
    [ReadOnlyInspector]
    public List<bool> listTileBuff;

    public Tile(bool _isWall, int _x, int _y)
    {
        isWall = _isWall;
        x = _x;
        y = _y;
    }

    public bool bSlot, isWall;
    [HideInInspector]
    public Tile cParentTile;

    [HideInInspector]
    public int x, y, G, H;
    [HideInInspector]
    public int F { get { return G + H; } }

    [SerializeField]
    private bool bIsUnit;
    private Transform tfBlock;
    [SerializeField]
    private Unit cPlacedUnit;

    [SerializeField]
    private LineRenderer cLine;

    [SerializeField]
    private Transform tfDustEffect;
    [SerializeField]
    private Transform tfGroundEffectParent;
    [SerializeField, ReadOnlyInspector]
    private List<ParticleSystem> listGroundEffects;

    public Vector3 tilePos { get { return new Vector3(x - InGameManager.instance.cAStar.fOffset, 0.5f, y - InGameManager.instance.cAStar.fOffset); } }

    void Awake()
    {
        if (!bSlot)
        {
            tfBlock = transform.GetComponentInChildren<NavMeshObstacle>().transform;
            tfBlock.gameObject.SetActive(false);
        }

        cLine.numCornerVertices = 5;

        if (!bSlot)
        {
            listGroundEffects = new List<ParticleSystem>();

            for (int i = 0; i < tfGroundEffectParent.childCount; i++)
            {
                listGroundEffects.Add(tfGroundEffectParent.GetChild(i).GetComponent<ParticleSystem>());
                listGroundEffects[i].gameObject.SetActive(false);
            }
        }

        listTileBuff = new List<bool>();

        for (int i = 0; i < (int)TileBuff.Count; i++)
            listTileBuff.Add(false);
    }

    void Start()
    {
        InGameManager.instance.SetTile(x, y, this);
    }

    public void SetUnit(Unit _cUnit, bool _bAction = true, bool _bEffect = true, bool _bRotate = true)
    {
        Vector3 _v3SpwanLocate = transform.position + Vector3.up * 0.5f;

        _cUnit.SetTile(this);
        cPlacedUnit = _cUnit;
        _cUnit.gameObject.transform.position = _v3SpwanLocate;
        isWall = !_cUnit.canNotBlock;
        bIsUnit = true;
        _cUnit.gameObject.name = $"{_cUnit.unitName} ({x} - {y})";

        if (_bRotate)
            _cUnit.body.rotation = Quaternion.Euler(0, 180f, 0);
        _cUnit.body.localPosition = Vector3.up * 0.1f;

        if (_cUnit is Hero)
        {
            if (!bSlot && _bAction && InGameManager.instance.cEditMap.tempTile != this)
                (_cUnit as Hero).PlayAnimation(UnitAniActionParam.Attack_Arrangement);

            (_cUnit as Hero).hud.OnOffUpperHUD(!bSlot);
            (_cUnit as Hero).hud.OnOffEmblem(false);

            if ((_cUnit as Hero).heroStat.eTribe == HeroTribe.Human)
                Invoke(nameof(UpdateTileBuff), Time.deltaTime);

            GameManager.instance.cSynergySystem.UpdatePlayerInfo();

            (cPlacedUnit as Hero).isOwn = true;
            (cPlacedUnit as Hero).isPlaced = !bSlot;
        }

        if (_bEffect)
            tfDustEffect.gameObject.SetActive(true);
    }

    private void UpdateTileBuff()
    {
        InGameManager.instance.stageController.SpawnHumanBuff(false);
    }

    public void SetUnitImmideiate(Unit _cUnit, bool _bAction = true)
    {
        Vector3 _v3SpwanLocate = transform.position + Vector3.up * 0.5f;

        _cUnit.SetTile(this);
        cPlacedUnit = _cUnit;
        _cUnit.gameObject.transform.position = _v3SpwanLocate;
        isWall = !_cUnit.canNotBlock;
        bIsUnit = true;
        _cUnit.gameObject.name = $"{_cUnit.unitName} ({x} - {y})";

        if (_cUnit is Hero)
        {
            _cUnit.body.rotation = Quaternion.Euler(0, 180f, 0);
            _cUnit.body.localPosition = Vector3.up * 0.1f;

            if (!bSlot && _bAction && FindObjectOfType<InGameManager>().cEditMap.tempTile != this)
                (_cUnit as Hero).PlayAnimation(UnitAniActionParam.Attack_Arrangement);

            (_cUnit as Hero).hud.OnOffUpperHUD(!bSlot);
            (_cUnit as Hero).hud.OnOffEmblem(false);

            FindObjectOfType<GameManager>().cSynergySystem.UpdatePlayerInfo();

            (cPlacedUnit as Hero).isOwn = true;
            (cPlacedUnit as Hero).isPlaced = !bSlot;
        }

        tfDustEffect.gameObject.SetActive(true);
    }

    public Unit SetNewUnit(Unit _cUnit)
    {
        if (_cUnit == null)
            return null;

        SetUnit(_cUnit);
        
        _cUnit.OnSpawn();
        if (bSlot)
            _cUnit.OnSlot();
        else
            _cUnit.OnPlace();

        InGameManager.instance.cPlayerController.placedUnits.Add(_cUnit);
        InGameManager.instance.cPlayerController.ownUnits.Add(_cUnit);

        if (_cUnit is Hero)
        {
            _cUnit.PlayDissolve(1, 1f, 0, 0.5f);

            InGameManager.instance.cPlayerController.ownHeros.Add(_cUnit as Hero);

            if (bSlot)
                InGameManager.instance.cPlayerController.waitingHeros.Add(_cUnit as Hero);
            else
                InGameManager.instance.cPlayerController.placedHeros.Add(_cUnit as Hero);

            StartCoroutine(CheckLevelUpCoroutine(_cUnit));
        }

        return _cUnit;
    }

    IEnumerator CheckLevelUpCoroutine(Unit _cUnit)
    {
        List<Unit> _listTempUnit = new List<Unit>();
        Unit _cMainHero_One = null;
        Unit _cMainUnit_Two = null;

        if (CheckCanLvUp(_cUnit, out _cMainHero_One, out _listTempUnit) && _cUnit.unitLevel == 1)
        {
            _cMainHero_One.LevelUp(_listTempUnit);

            yield return null;

            yield return new WaitUntil(() => !_cMainHero_One.leveling);

            if (CheckCanLvUp(_cMainHero_One, out _cMainUnit_Two, out _listTempUnit))
                _cMainUnit_Two.LevelUp(_listTempUnit);
        }
    }

    private bool CheckCanLvUp(Unit _cUnit, out Unit _cMainUnit, out List<Unit> _listUnit)
    {
        List<Unit> _listOwnUnit = InGameManager.instance.cPlayerController.ownUnits;
        List<Unit> _listTempUnit = new List<Unit>();

        _cMainUnit = null;
        _listUnit = new List<Unit>();

        for (int i = 0; i < _listOwnUnit.Count; i++)
        {
            if (_listOwnUnit[i].unitID == _cUnit.unitID &&_listOwnUnit[i].unitLevel == _cUnit.unitLevel)
            {
                if (!_listOwnUnit[i].leveling)
                {
                    _listTempUnit.Add(_listOwnUnit[i]);
                    if (_listTempUnit.Count >= 3)
                        break;
                }
            }
        }

        if (_listTempUnit.Count >= 3)
        {
            for (int i = 0; i < _listTempUnit.Count; i++)
            {
                if (!_listTempUnit[i].isSlot)
                {
                    _cMainUnit = _listTempUnit[i];
                    break;
                }

                _cMainUnit = _listTempUnit[i];
            }

            _listTempUnit.Remove(_cMainUnit);
            _listUnit = _listTempUnit;

            return true;
        }

        return false;
    }

    public void SetNewUnitImmeidate(Unit _cUnit)
    {
        if (_cUnit == null)
            return;

        SetUnitImmideiate(_cUnit);

        FindObjectOfType<InGameManager>().cPlayerController.ownUnits.Add(_cUnit);
    }

    public void MoveUnit(Tile _cOrgTile, bool _bAction = true)
    {
        Unit _cTempUnit = _cOrgTile.placedUnit;

        SetUnit(_cOrgTile.cPlacedUnit, _bAction);

        if (bSlot)
            _cTempUnit.OnSlot();
        else
            _cTempUnit.OnPlace();

        if (_cOrgTile != this)
        {
            _cOrgTile.RemoveUnit();

            if (placedUnit is Hero)
            {
                (placedUnit as Hero).hud.OnOffUpperHUD(!bSlot);

                if (_cOrgTile.bSlot != bSlot)
                {
                    if (bSlot)
                    {
                        InGameManager.instance.cPlayerController.placedUnits.Remove(_cTempUnit);
                        InGameManager.instance.cPlayerController.placedHeros.Remove(_cTempUnit as Hero);
                        InGameManager.instance.cPlayerController.waitingHeros.Add(placedUnit as Hero);
                    }
                    else
                    {
                        InGameManager.instance.cPlayerController.placedUnits.Add(_cTempUnit);
                        InGameManager.instance.cPlayerController.waitingHeros.Remove(_cTempUnit as Hero);
                        InGameManager.instance.cPlayerController.placedHeros.Add(placedUnit as Hero);
                    }
                }
            }
        }

        InGameManager.instance.cPlayerController.UpdateCampInfo();
    }

    public void SwitchUnit(Tile _cOrgTile, bool _bAction = true, bool _bEffect = true, bool _bRotate = true)
    {
        Unit _cTempUnit = cPlacedUnit;

        if (_cOrgTile.bSlot != bSlot)
        {
            if (_cOrgTile.bSlot)
            {
                InGameManager.instance.cPlayerController.waitingHeros.Remove(_cOrgTile.placedUnit as Hero);
                InGameManager.instance.cPlayerController.placedHeros.Add(_cOrgTile.placedUnit as Hero);
                InGameManager.instance.cPlayerController.placedHeros.Remove(cPlacedUnit as Hero);
                InGameManager.instance.cPlayerController.waitingHeros.Add(cPlacedUnit as Hero);
            }
            else
            {
                InGameManager.instance.cPlayerController.placedHeros.Remove(_cOrgTile.placedUnit as Hero);
                InGameManager.instance.cPlayerController.waitingHeros.Add(_cOrgTile.placedUnit as Hero);
                InGameManager.instance.cPlayerController.waitingHeros.Remove(cPlacedUnit as Hero);
                InGameManager.instance.cPlayerController.placedHeros.Add(cPlacedUnit as Hero);
            }
        }

        SetUnit(_cOrgTile.cPlacedUnit, _bAction, _bEffect, _bRotate);

        _cOrgTile.SetUnit(_cTempUnit, _bAction, _bEffect, _bRotate);
    }

    public void RemoveUnit()
    {
        isWall = false;
        bIsUnit = false;
        cPlacedUnit = null;
    }

    public void SellUnit(bool _bSell = true)
    {
        InGameManager.instance.cPlayerController.ownUnits.Remove(cPlacedUnit);

        if (cPlacedUnit is Hero)
        {
            InGameManager.instance.cPlayerController.ownHeros.Remove(cPlacedUnit as Hero);

            if (bSlot)
                InGameManager.instance.cPlayerController.waitingHeros.Remove(cPlacedUnit as Hero);
            else
                InGameManager.instance.cPlayerController.placedHeros.Remove(cPlacedUnit as Hero);

            if (_bSell)
                InGameManager.instance.cPlayerController.AddGold((cPlacedUnit as Hero).heroCost);
            InGameManager.instance.cMarketSystem.objPool.ReturnObj(cPlacedUnit.gameObject, (cPlacedUnit as Hero).heroNumber);
        }
        else if (cPlacedUnit != null)
            cPlacedUnit.gameObject.SetActive(false);

        cPlacedUnit.OnRemove();
        RemoveUnit();

        GameManager.instance.cSynergySystem.UpdatePlayerInfo();
        InGameManager.instance.cPlayerController.UpdateCampInfo();
    }

    public void DrawLine(float _fHeight)
    {
        if (cLine == null)
            return;

        float _fLength = 0.4f;

        cLine.startWidth = 0.04f;
        cLine.positionCount = 4;

        cLine.SetPosition(0, new Vector3(transform.position.x + _fLength, _fHeight, transform.position.z + _fLength));
        cLine.SetPosition(1, new Vector3(transform.position.x - _fLength, _fHeight, transform.position.z + _fLength));
        cLine.SetPosition(2, new Vector3(transform.position.x - _fLength, _fHeight, transform.position.z - _fLength));
        cLine.SetPosition(3, new Vector3(transform.position.x + _fLength, _fHeight, transform.position.z - _fLength));

        cLine.enabled = false;
    }

    public void SwitchLine(bool _bOn)
    {
        if (cLine != null)
            cLine.enabled = _bOn;
    }

    public void SpotLine(bool _isSelect, bool _isOrigin = false)
    {
        if (cLine == null)
            return;

        cLine.material.SetInt("bIsSelect", _isSelect ? 1 : 0);
        cLine.material.SetInt("bIsOriginTile", _isOrigin ? 1 : 0);
        cLine.startWidth = _isSelect ? 0.16f : 0.04f;

        if (_isOrigin)
            cLine.startWidth = 0.06f;
    }

    public void OpenFieldBuffEffect(bool _bOpen, TileBuff _eBuff)
    {
        OffAllBuffEffect();
        listTileBuff[(int)_eBuff] = true;
        listGroundEffects[(int)_eBuff].gameObject.SetActive(_bOpen);
    }

    public void OffAllBuffEffect()
    {
        for (int i = 0; i < listTileBuff.Count; i++)
            listTileBuff[i] = false;

        foreach (var item in listGroundEffects)
            item.gameObject.SetActive(false);
    }

    public void UnLinkPlaceUnit()
    {
        InGameManager.instance.cPlayerController.placedUnits.Remove(cPlacedUnit);
        cPlacedUnit = null;
        bIsUnit = false;
    }

    public Tile GetNearestAvailableTile()
    {
        Queue<Tile> _queue = new Queue<Tile>();
        HashSet<Tile> _visited = new HashSet<Tile>();

        Tile _cStartTile = this;
        _visited.Add(_cStartTile);
        _queue.Enqueue(_cStartTile);

        while (_queue.Count > 0)
        {
            Tile _cCurrentTile = _queue.Dequeue();

            if (!_cCurrentTile.isUnit)
            {
                return _cCurrentTile;
            }

            List<Tile> _listNeighbors = _cCurrentTile.GetNeighborTiles(); // 변경된 부분
            foreach (Tile cNeighbor in _listNeighbors)
            {
                if (!_visited.Contains(cNeighbor))
                {
                    _queue.Enqueue(cNeighbor);
                    _visited.Add(cNeighbor);
                }
            }
        }

        return null;
    }

    private List<Tile> GetNeighborTiles()
    {
        List<Tile> _cNeighbors = new List<Tile>();

        // Horizontal and vertical neighbors
        if (x + 1 < InGameManager.instance.cAStar.sizeX)
            _cNeighbors.Add(InGameManager.instance.tiles[x + 1, y]); // Right
        if (x - 1 >= 0)
            _cNeighbors.Add(InGameManager.instance.tiles[x - 1, y]); // Left
        if (y + 1 < InGameManager.instance.cAStar.sizeY)
            _cNeighbors.Add(InGameManager.instance.tiles[x, y + 1]); // Top
        if (y - 1 >= 0)
            _cNeighbors.Add(InGameManager.instance.tiles[x, y - 1]); // Bottom

        // Diagonal neighbors
        if (x - 1 >= 0 && y - 1 >= 0)
            _cNeighbors.Add(InGameManager.instance.tiles[x - 1, y - 1]); // Bottom-left
        if (x + 1 < InGameManager.instance.cAStar.sizeX && y - 1 >= 0)
            _cNeighbors.Add(InGameManager.instance.tiles[x + 1, y - 1]); // Bottom-right
        if (x - 1 >= 0 && y + 1 < InGameManager.instance.cAStar.sizeY)
            _cNeighbors.Add(InGameManager.instance.tiles[x - 1, y + 1]); // Top-left
        if (x + 1 < InGameManager.instance.cAStar.sizeX && y + 1 < InGameManager.instance.cAStar.sizeY)
            _cNeighbors.Add(InGameManager.instance.tiles[x + 1, y + 1]); // Top-right

        return _cNeighbors;
    }

    public List<Tile> GetCrossesTiles(int _n)
    {
        List<Tile> _cNeighbors = new List<Tile>();

        if (_n == 0)
        {
            _cNeighbors.Add(InGameManager.instance.tiles[x, y]);
            return _cNeighbors;
        }

        // Horizontal and vertical neighbors
        for (int i = 1; i <= _n; i++)
        {
            if (x + i < InGameManager.instance.cAStar.sizeX)
                _cNeighbors.Add(InGameManager.instance.tiles[x + i, y]); // Right
            if (x - i >= 0)
                _cNeighbors.Add(InGameManager.instance.tiles[x - i, y]); // Left
            if (y + i < InGameManager.instance.cAStar.sizeY)
                _cNeighbors.Add(InGameManager.instance.tiles[x, y + i]); // Top
            if (y - i >= 0)
                _cNeighbors.Add(InGameManager.instance.tiles[x, y - i]); // Bottom
        }

        return _cNeighbors;
    }


    public Tile GetCrossInLine(int _n)
    {
        List<Tile> _tiles = GetCrossesTiles(_n);

        for (int i = 0; i < _tiles.Count; i++)
        {
            if (InGameManager.instance.cAStar.finalTileList.Contains(_tiles[i]))
                return _tiles[i];
        }

        return null;
    }

    public List<Tile> GetNearbyTiles(int _distance)
    {
        List<Tile> _listNearbyTiles = new List<Tile>();

        int _width = InGameManager.instance.cAStar.sizeX;
        int _height = InGameManager.instance.cAStar.sizeY;

        int _minX = Mathf.Max(0, x - _distance);
        int _maxX = Mathf.Min(_width - 1, x + _distance);
        int _minY = Mathf.Max(0, y - _distance);
        int _maxY = Mathf.Min(_height - 1, y + _distance);

        for (int i = _minX; i <= _maxX; i++)
        {
            for (int j = _minY; j <= _maxY; j++)
                _listNearbyTiles.Add(InGameManager.instance.GetTile(i, j));
        }

        return _listNearbyTiles;
    }

    public int GetTileIndexOfFinal()
    {
        return InGameManager.instance.cAStar.finalRoadList.IndexOf(this);
    }

    public Tile GetNextTile(int _iCount)
    {
        int _index = InGameManager.instance.cAStar.finalRoadList.IndexOf(this) + _iCount;

        if (_index < 0 || _index >= InGameManager.instance.cAStar.finalRoadList.Count - 1)
            return null;
        else
            return InGameManager.instance.cAStar.finalRoadList[_index];
    }

    public bool isUnit { get => bIsUnit; }
    public Transform dustEffect { get => tfDustEffect; }
    public Transform block { get => tfBlock; }
    public Unit placedUnit { get => cPlacedUnit; }
}
