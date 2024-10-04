using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardType { All, Equip, Spell, Count }
public enum CardClass { All, Hit, Buff, Support, Weapon, Armor, Accessories, Count }
public enum CardCostType { All, Gold, Blood, Count }
public enum CardTear { None, Basic, Common, UnCommon, Rare, Epic, Legendary, Count }

public class CardManager : SingletonMono<CardManager>
{
    [SerializeField]
    private List<Card> listCard, listDeckCard;

    [SerializeField, ReadOnlyInspector]
    private Sprite[] imgTypeIcons, imgGems, imgClassIcons;
    public List<Sprite> costSpriteList;

    public Sprite equipIcon, spellIcon;
    public List<CardFrame> cardFrameList;

    protected override void Awake()
    {
        base.Awake();

        listDeckCard = new List<Card>();

        for (int i = 0; i < 10; i++)
            listDeckCard.Add(new Card());
    }

    [ContextMenu("Init")]
    private void Init()
    {
        imgTypeIcons = new Sprite[(int)CardType.Count];
        imgTypeIcons[(int)CardType.All] = Resources.Load<Sprite>("GUI/Card/CardType_All");
        imgTypeIcons[(int)CardType.Equip] = Resources.Load<Sprite>("GUI/Card/CardType_Equip");
        imgTypeIcons[(int)CardType.Spell] = Resources.Load<Sprite>("GUI/Card/CardType_Spell");

        imgGems = new Sprite[(int)CardTear.Count];
        imgClassIcons = new Sprite[(int)CardClass.Count];

        for (int i = 0; i < imgGems.Length; i++)
            imgGems[i] = Resources.Load<Sprite>($"GUI/Card/CardGem_{(CardTear)i}");
        for (int i = 0; i < imgGems.Length; i++)
            imgClassIcons[i] = Resources.Load<Sprite>($"GUI/Card/CardClass_{(CardClass)i}");
    }

    public Card GetCardWithID(int _iCardID)
    {
        for (int i = 0; i < listCard.Count; i++)
        {
            if (listCard[i].stat.iCardID == _iCardID)
                return listCard[i];
        }

        return null;
    }

    public void SetDeckCard(List<int> _cardList)
    {
        listDeckCard = new List<Card>();

        foreach (var item in _cardList)
            listDeckCard.Add(GetCardWithID(item));

        Debug.LogError("SetCradDeck");
    }

    public CardFrame GetCardFrameWithID(string _id)
    {
        foreach (var item in cardFrameList)
        {
            if (item.itemId == _id)
                return item;
        }

        return new CardFrame();
    }

    public List<Card> cardList { get => listCard; set => listCard = value; }
    public List<Card> deckCard { get => listDeckCard; }
    public Sprite[] cardTypeIcons { get => imgTypeIcons; }
    public Sprite[] cardGems { get => imgGems; }
    public Sprite[] cardClassIcons { get => imgClassIcons; }
}
