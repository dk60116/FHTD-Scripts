using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MapBuilder : MonoBehaviour
{
    [SerializeField]
    private Transform tfTileHolder, tfSlotHolder, tfFloor;

    public Tile TilePrefab;
    public Vector2Int v2MapSize;
    [Range(0, 1f)]
    public float fOutlinePercent;

    [SerializeField]
    private AStar cAStar;

    [SerializeField]
    private Unit cEnemyCampUnit, cPlayerCampUnit;
    [SerializeField]
    private Transform walls, tfMapDeco;
    [SerializeField]
    private Material matTile;
    [SerializeField]
    private TextMeshPro countDisplayText; 

    public void GenerateMap()
    {
        cAStar.sizeX = v2MapSize.x;
        cAStar.sizeY = v2MapSize.y;
        cAStar.sTileList = new TileList[v2MapSize.x];
        cAStar.topRight = new Vector2Int(v2MapSize.x - 1, v2MapSize.y - 1);

        for (int i = 0; i < v2MapSize.x; i++)
        {
            cAStar.sTileList[i] = new TileList();
            cAStar.sTileList[i].cTileArray = new Tile[v2MapSize.y];
        }

        foreach (var item in FindObjectsOfType<Tile>())
        {
            if (!item.bSlot)
                DestroyImmediate(item.gameObject);
        }

        tfFloor.localScale = new Vector3(v2MapSize.x, 1f, v2MapSize.y);
        tfFloor.position = Vector3.zero;

        for (int x = 0; x < v2MapSize.x; x++)
        {
            for (int y = 0; y < v2MapSize.y; y++)
            {
                Vector3 _v3TilePositon = new Vector3(-v2MapSize.x / 2 + x + 0.5f, 0, -v2MapSize.y / 2 + y + 0.5f);
                Transform _tfNewTile = Instantiate(TilePrefab.transform, _v3TilePositon, Quaternion.identity);
                _tfNewTile.name = $"Tile ({x} - {y})";
                cAStar.sTileList[x].cTileArray[y] = _tfNewTile.GetComponent<Tile>();
                cAStar.sTileList[x].cTileArray[y].x = x;
                cAStar.sTileList[x].cTileArray[y].y = y;
                _tfNewTile.SetParent(tfTileHolder);
                _tfNewTile.transform.localScale = Vector3.one * (1f - fOutlinePercent);
                _tfNewTile.GetComponent<Tile>().DrawLine(0.6f);
            }
        }

        for (int i = 0; i < tfSlotHolder.childCount; i++)
            tfSlotHolder.GetChild(i).GetComponent<Tile>().DrawLine(0.7f);

        if (FindObjectOfType<EnemyCamp>() != null)
            DestroyImmediate(FindObjectOfType<EnemyCamp>().gameObject);
        if (FindObjectOfType<PlayerCamp>() != null)
            DestroyImmediate(FindObjectOfType<PlayerCamp>().gameObject);
        cAStar.SetStartNode(cAStar.GetTile(0, v2MapSize.y - 1));
        cAStar.SetTargetNode(cAStar.GetTile(v2MapSize.x - 1, 0));
        Unit _cEnemyCampUnit = Instantiate(cEnemyCampUnit);
        cAStar.startTile.SetNewUnitImmeidate(_cEnemyCampUnit);
        PlayerCamp _cPlayerCampUnit = Instantiate(cPlayerCampUnit as PlayerCamp);
        _cPlayerCampUnit.SetTextUI();
        cAStar.targetTile.SetNewUnitImmeidate(_cPlayerCampUnit);

        tfMapDeco.localScale = new Vector3(v2MapSize.x, v2MapSize.x, v2MapSize.y) * 0.1f;
        float _multyply = 1.25f;
        walls.localScale = new Vector3(tfMapDeco.localScale.x * _multyply, 1.1f, tfMapDeco.localScale.z * _multyply);
        matTile.mainTextureScale = new Vector2(v2MapSize.x * 0.25f, v2MapSize.y * 0.25f);
        FindObjectOfType<AStar>().fOffset = v2MapSize.x * 0.5f - 0.5f;
        countDisplayText.fontSize = v2MapSize.x;
    }

    public Transform tileHolder { get => tfTileHolder; }
}
