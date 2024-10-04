using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Card
{
    [SerializeField, ReadOnlyInspector]
    private bool bOwn;
    [SerializeField, ReadOnlyInspector]
    public int count;
    [SerializeField]
    private CardStatus sStat;
    [ReadOnlyInspector]
    public string instanceId;
    [ReadOnlyInspector]
    public CardDisplay display;

    public void OwnCard(string _instanceId, int _count)
    {
        bOwn = true;
        count = _count;
        instanceId = _instanceId;
    }

    public void UnOwnCard()
    {
        bOwn = false;
        count = 0;
        instanceId = string.Empty;
    }

    public bool own { get => bOwn; set => bOwn = value; }
    public CardStatus stat { get => sStat; set => sStat = value; }
}
