using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class SplineController : MonoBehaviour
{
    [SerializeField]
    private GameObject goParticle;
    [SerializeField, ReadOnlyInspector]
    private PoolingManager cPool;

    [SerializeField, ReadOnlyInspector]
    private List<GameObject> listParticles;
    private List<Tile> listTemp;

    [SerializeField]
    private float _fMoveSpeed;

    void Awake()
    {
        cPool = GetComponent<PoolingManager>();
        listParticles = new List<GameObject>();
        listTemp = new List<Tile>();
    }

    void Start()
    {
        PoolingParticles();
    }

    private void PoolingParticles()
    {
        cPool.AddTargetObj(goParticle);
        cPool.AddListCount(50);
        cPool.Init();
    }

    public void DrawLine()
    {
        CancelInvoke();
        InvokeRepeating(nameof(SpawnParticles), 0, 1f / _fMoveSpeed);
    }

    private void SpawnParticles()
    {
        for (int i = 0; i < listParticles.Count; i++)
            cPool.ReturnObj(listParticles[i], 0);

        foreach (var item in listParticles)
            item.transform.DOKill();

        listParticles.Clear();

        for (int i = 0; i < InGameManager.instance.cAStar.finalTileList.Count - 1; i++)
        {
            listParticles.Add(cPool.GetObj(0, transform.GetChild(0)));
            listParticles[i].transform.position = InGameManager.instance.cAStar.GetFinalTargetListPos(i) + Vector3.up * 0.01f;
            if (i < InGameManager.instance.cAStar.finalTileList.Count - 1)
            {
                listParticles[i].transform.LookAt(InGameManager.instance.cAStar.GetFinalTargetListPos(i + 1) + Vector3.up * 0.01f);
                listParticles[i].transform.DOMove(InGameManager.instance.cAStar.GetFinalTargetListPos(i + 1) + Vector3.up * 0.01f, 1f / _fMoveSpeed).SetEase(Ease.Linear);
            }
        }

        listTemp.Clear();
        listTemp = InGameManager.instance.cAStar.finalTileList.ToList();
    }

    public bool CheckSame()
    {
        return listTemp.SequenceEqual(InGameManager.instance.cAStar.finalTileList);
    }

    public void OnOffSPline(bool _bValue)
    {
        transform.GetChild(0).gameObject.SetActive(_bValue);
    }
}
