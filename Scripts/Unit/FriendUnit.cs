using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendUnit : Unit
{
    [SerializeField]
    private UnitStatus sEditableStatus;
    [SerializeField]
    private LineRenderer cLinkRender;

    protected override void OnEnable()
    {
        sUnitStat = sEditableStatus;
        OnOffLinkLine(false);
    }

    void Update()
    {
        if (cFreindUnit != null) 
        {
            cLinkRender.SetPosition(0, tfBodyHolder.position + Vector3.up * 0.25f);
            cLinkRender.SetPosition(1, cFreindUnit.bodyHolder.position + Vector3.up * 0.25f);
        }
    }

    public void SetPartner(Hero _cPartnerHero)
    {
        cFreindUnit = _cPartnerHero;
    }

    public void OnOffLinkLine(bool _bValue)
    {
        cLinkRender.enabled = _bValue;
    }
}
