using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UnitController : MonoBehaviour
{
    [SerializeField]
    private Transform tfUnitSlot;
    [SerializeField, ReadOnlyInspector]
    private List<Unit> listAttackUnits;

    [SerializeField]
    private float fSpawnDelay;

    private bool bIsWaveStart;

    public void ResetStageUnits()
    {
        listAttackUnits = new List<Unit>();

        for (int i = 0; i < tfUnitSlot.childCount; i++)
        {
            if (tfUnitSlot.GetChild(i).gameObject.activeSelf)
            {
                listAttackUnits.Add(tfUnitSlot.GetChild(i).GetComponent<Hero>());
                tfUnitSlot.GetChild(i).gameObject.SetActive(false);
            }
        }

        InGameManager.instance.SetStatus(StageStatus.Waiting);
    }

    public void StopAttack()
    {
        StopAllCoroutines();
        InGameManager.instance.bIsRoundStart = false;
        InGameManager.instance.cAStar.SwitchPathLine(true);

        for (int i = 0; i < listAttackUnits.Count; i++)
        {
            listAttackUnits[i].transform.position = InGameManager.instance.cAStar.startTile.tilePos;
            listAttackUnits[i].gameObject.SetActive(false);
        }

        InGameManager.instance.SetStatus(StageStatus.Waiting);
    }

    public void TryStartRound()
    {
        InGameManager.instance.cAStar.PathEvent.Invoke();

        if (true || InGameManager.instance.cPlayerController.CanRoundStart())
        {
            InGameManager.instance.cPlayerController.ResetPlacedUnitsStatus();
            InGameManager.instance.cPlayerController.ResetEnemyUnitsStatus();
            StartMoveToTarget();
        }
        else
            GameManager.instance.OpenConfirm("There are too many units!", null, false);
    }

    public void StartMoveToTarget()
    {
        StopAllCoroutines();

        InGameManager.instance.SetStatus(StageStatus.Running);

        InGameManager.instance.bIsRoundStart = true;
        InGameManager.instance.cAStar.SwitchPathLine(false);
        InGameManager.instance.cDealParserPanel.UpdateHeroList();

        foreach (var item in FindObjectsOfType<Tile>())
        {
            if (item.isUnit && item.placedUnit is Hero)
            {
                (item.placedUnit as Hero).hud.OnOffEmblem(false);
            }
        }

        foreach (var item in FindObjectsOfType<Tile>())
        {
            if (!item.bSlot)
                item.block.gameObject.SetActive(true);
        }

        foreach (var item in InGameManager.instance.cAStar.finalTileList)
        {
            item.block.gameObject.SetActive(false);
        }

        StopAllCoroutines();
        StartCoroutine(nameof(StartMoveToTargetCoroutine));
    }

    IEnumerator StartMoveToTargetCoroutine()
    {
        bIsWaveStart = true;

        foreach (var item in listAttackUnits)
        {
            item.gameObject.SetActive(false);
            item.transform.position = InGameManager.instance.cAStar.startTile.tilePos;
        }

        yield return null;

        for (int i = 0; i < listAttackUnits.Count; i++)
        {
            listAttackUnits[i].gameObject.SetActive(true);
            listAttackUnits[i].transform.rotation = Quaternion.identity;
            listAttackUnits[i].body.localRotation = Quaternion.identity;
            listAttackUnits[i].SetupStat();
            listAttackUnits[i].StartMoveToTarget();

            yield return new WaitForSeconds(fSpawnDelay);
        }
    }

    public List<Unit> attackUnitList { get => listAttackUnits; }
}
