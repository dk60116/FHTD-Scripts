using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameCardSysyem : MonoBehaviour
{
    public List<Card> listDeck;

    void Start()
    {
        listDeck = new List<Card>();
        //listDeck = CardManager.instance.deckCard;
        for (int i = 0; i < 10; i++)
        {
            listDeck.Add(new Card());
            listDeck[i].stat = CardManager.instance.cardList[i].stat;
            listDeck[i].own = true;
        }
    }
}
