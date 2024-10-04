using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[Serializable]
public class TileList
{
    public Tile[] cTileArray;
}

public class AStar : MonoBehaviour
{
    [ReadOnlyInspector]
    public Vector2Int bottomLeft, topRight, startPos, targetPos;
    [SerializeField, ReadOnlyInspector]
    private List<Tile> listFinalNodeList;

    public int sizeX, sizeY;
    public TileList[] sTileList;
    [SerializeField]
    private Tile cStartTile, cTargetTile;
    private Tile StartNode, TargetNode, CurNode;
    List<Tile> OpenList, ClosedList;

    [SerializeField]
    private LineRenderer cLineRender;

    [ReadOnlyInspector]
    public float fOffset;

    public delegate void PathFindingEvents();

    public PathFindingEvents PathEvent;

    [SerializeField]
    private SplineController cSpline;

    void Awake()
    {
        cSpline = FindObjectOfType<SplineController>();
        PathEvent += PathFinding;
        PathEvent += DrawPathLine;
        PathEvent += DrawSpline;
    }

    public Tile GetTile(int _x, int _y)
    {
        return sTileList[_x].cTileArray[_y];
    }

    public Tile startTile { get => cStartTile; }

    public Tile targetTile { get => cTargetTile; }

    public void SetStartNode(Tile _cTile)
    {
        cStartTile = _cTile;
    }

    public void SetTargetNode(Tile _cTile)
    {
        cTargetTile = _cTile;
    }

    private void PathFinding()
    {
        StartNode = cStartTile;
        TargetNode = GetTile(targetTile.x - bottomLeft.x, targetTile.y - bottomLeft.y);

        OpenList = new List<Tile>() { StartNode };
        ClosedList = new List<Tile>();
        listFinalNodeList = new List<Tile>();

        while (OpenList.Count > 0)
        {
            CurNode = OpenList[0];

            for (int i = 1; i < OpenList.Count; i++)
            {
                if (OpenList[i].F <= CurNode.F && OpenList[i].H < CurNode.H)
                    CurNode = OpenList[i];
            }

            OpenList.Remove(CurNode);
            ClosedList.Add(CurNode);

            if (CurNode == TargetNode)
            {
                Tile TargetCurNode = TargetNode;

                while (TargetCurNode != StartNode)
                {
                    listFinalNodeList.Add(TargetCurNode);
                    TargetCurNode = TargetCurNode.cParentTile;
                }

                listFinalNodeList.Add(StartNode);
                listFinalNodeList.Reverse();

                return;
            }

            OpenListAdd(CurNode.x, CurNode.y + 1);
            OpenListAdd(CurNode.x + 1, CurNode.y);
            OpenListAdd(CurNode.x, CurNode.y - 1);
            OpenListAdd(CurNode.x - 1, CurNode.y);
        }
    }

    void OpenListAdd(int checkX, int checkY)
    {
        // 상하좌우 범위를 벗어나지 않고, 벽이 아니면서, 닫힌리스트에 없다면
        if (checkX >= bottomLeft.x && checkX < topRight.x + 1 && checkY >= bottomLeft.y && checkY < topRight.y + 1 && !GetTile(checkX - bottomLeft.x, checkY - bottomLeft.y).isWall && !ClosedList.Contains(GetTile(checkX - bottomLeft.x, checkY - bottomLeft.y)))
        {
            // 이웃노드에 넣고, 직선은 10, 대각선은 14비용
            Tile NeighborNode = GetTile(checkX - bottomLeft.x, checkY - bottomLeft.y);
            int MoveCost = CurNode.G + (CurNode.x - checkX == 0 || CurNode.y - checkY == 0 ? 10 : 14);


            // 이동비용이 이웃노드G보다 작거나 또는 열린리스트에 이웃노드가 없다면 G, H, ParentNode를 설정 후 열린리스트에 추가
            if (MoveCost < NeighborNode.G || !OpenList.Contains(NeighborNode))
            {
                NeighborNode.G = MoveCost;
                NeighborNode.H = (Mathf.Abs(NeighborNode.x - TargetNode.x) + Mathf.Abs(NeighborNode.y - TargetNode.y)) * 10;
                NeighborNode.cParentTile = CurNode;

                OpenList.Add(NeighborNode);
            }
        }
    }

    public void DrawSpline()
    {
        if (!cSpline.CheckSame())
            cSpline.DrawLine();
    }

    public void DrawPathLine()
    {
        return;

        cLineRender.positionCount = listFinalNodeList.Count;

        for (int i = 0; i < cLineRender.positionCount; i++)
            cLineRender.SetPosition(i, listFinalNodeList[i].tilePos + Vector3.up * 0.05f);
    }

    public void OnOffPathLine(bool _bOn)
    {
        cLineRender.enabled = _bOn;
    }

    public void SwitchPathLine(bool _bOn)
    {
        cLineRender.enabled = _bOn;
        cSpline.OnOffSPline(_bOn);
    }

    public Vector3 GetFinalTargetListPos(int _iIndex)
    {
        return new Vector3(listFinalNodeList[_iIndex].x - fOffset, 0.5f, listFinalNodeList[_iIndex].y - fOffset);
    }

    public List<Tile> GetFInalTileRoutListWithoutCamps()
    {
        List<Tile> _list = listFinalNodeList.ToList();
        _list.RemoveAt(0);
        _list.RemoveAt(_list.Count - 1);
        return _list;
    }

    public int GetRangeBetweenTwoTiles(Tile cTileA, Tile cTileB)
    {
        return Mathf.Abs(listFinalNodeList.IndexOf(cTileA) - listFinalNodeList.IndexOf(cTileB));
    }

    void OnDrawGizmos()
    {
#if UNITY_EDITOR
        Gizmos.color = Color.red;

        if (finalTileList.Count != 0)
        {
            for (int i = 0; i < finalTileList.Count - 1; i++)
                Gizmos.DrawLine(new Vector3(finalTileList[i].x - fOffset, 2f, finalTileList[i].y - fOffset), new Vector3(finalTileList[i + 1].x - fOffset, 2f, finalTileList[i + 1].y - fOffset));
        }

        GUIStyle _newStyle = new GUIStyle();

        _newStyle.normal.textColor = Color.green;

        if (sTileList.Length > 0)
        {
            for (int x = 0; x < sTileList.Length; x++)
            {
                for (int y = 0; y < sTileList[x].cTileArray.Length; y++)
                    Handles.Label((sTileList[x].cTileArray[y].transform.position) + new Vector3(-0.2f, 0, 0.2f), $"[{x}-{y}]", _newStyle);
            }
        }
#endif
    }

    public int mapSize { get => sizeX * sizeY; }
    public List<Tile> finalTileList { get => listFinalNodeList; }
    public List<Tile> finalRoadList { get => listFinalNodeList.GetRange(1, listFinalNodeList.Count - 2); }
}
