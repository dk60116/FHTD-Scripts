using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum PartnerSpawnTiming { Use, UseHeroSkill, useCard, WhenAHero, WhenPlaceHero, WhenASynergy}

public class SpawnPartnerUnit : MonoBehaviour
{
    [SerializeField, ReadOnlyInspector]
    private Unit cOriginUnit;
    [SerializeField]
    private GameObject goPartnerPrefab;
    [SerializeField, ReadOnlyInspector]
    private Unit cPartnerUnit;
    [SerializeField]
    private PartnerSpawnTiming eTiming;

    [SerializeField]
    private int iSkillId, iCardId;

    [SerializeField]
    private bool bSyncRank, bSynkLevel, bSyncStatus;

    void Awake()
    {
        cOriginUnit = GetComponent<Unit>();
    }

    public void SpawnUnit()
    {
        if (goPartnerPrefab != null)
        {
            cPartnerUnit = Instantiate(goPartnerPrefab, InGameManager.instance.tfTempUnitHolder).GetComponent<Unit>();
            cPartnerUnit.isOwn = true;
            cOriginUnit.SetFreindUnit(cPartnerUnit);
            cPartnerUnit.SetFreindUnit(cOriginUnit);
            OnSpawn();
        }
    }

    public void PlaceUnit()
    {
        if (cPartnerUnit.isPlaced)
            return;

        cPartnerUnit.transform.SetParent(null);
        cOriginUnit.currentTile.GetNearestAvailableTile().SetNewUnit(cPartnerUnit);
        cPartnerUnit.isPlaced = true;
        OnSpawn();
    }

    public void DisableUnit()
    {
        if (!cPartnerUnit.isPlaced)
            return;

        cPartnerUnit.transform.SetParent(InGameManager.instance.tfTempUnitHolder);
        cPartnerUnit.transform.localPosition = Vector3.zero;
        cPartnerUnit.isPlaced = false;
        cPartnerUnit.currentTile.UnLinkPlaceUnit();
    }

    public void DestroyUnit()
    {
        cOriginUnit.SetFreindUnit(null);
        Destroy(cPartnerUnit.gameObject);
    }

    public void OnSpawn()
    {
        if (bSyncRank)
            cPartnerUnit.SetRank(cOriginUnit.unitStat.eRank);
        if (bSynkLevel)
        {
            cPartnerUnit.SetLevel(cOriginUnit.unitLevel, cOriginUnit.unitID);
        }
    }

    public PartnerSpawnTiming timing { get => eTiming; }
}
