using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyCamp : Unit
{
    public override void SetTile(Tile _cTile)
    {
        FindObjectOfType<InGameManager>().cAStar.SetStartNode(_cTile);

        base.SetTile(_cTile);
    }

    protected override void Start()
    {
        base.Start();
        InGameManager.instance.cPlayerController.placedUnits.Add(this);
    }
}
