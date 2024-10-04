using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public abstract class CardDisplay : MonoBehaviour
{
    [SerializeField]
    protected Card cCard;
    [SerializeField]
    protected Image costFrame;
    [SerializeField]
    protected TextMeshProUGUI txtCardName, txtLevel, txtCost;
    [SerializeField]
    protected Image imgIllust, imgMark, imgTearGem, imgClassIcon, imgFront, imgBack, imgSelect;

    [SerializeField]
    protected CardAbility cAbility;

    protected RectTransform tfRect;
    protected CanvasGroup cCanvasGroup;

    [ReadOnlyInspector]
    protected bool bSelect;

    protected Vector2 v2ClickPos;
    protected Vector2 v2OrgPos;
    protected Vector3 v3OrgRot;

    protected Ray rayMouseHit_Tile;
    protected RaycastHit rayHit_Tile;
    protected int iLayerMask_Tile;

    [SerializeField, ReadOnlyInspector]
    protected CardInfoDisplay cardInfo;

    [ContextMenu("Init")]
    private void Init()
    {
        cAbility = GetComponent<CardAbility>();
        cardInfo = GetComponent<CardInfoDisplay>();
    }

    public virtual void UpdateCardInfo(Card _cCard)
    {
        cCard = _cCard;

        tfRect = GetComponent<RectTransform>();

        CardFrame _frame = CardManager.instance.GetCardFrameWithID("300400");

        if (SocialManager.instance != null)
            _frame = CardManager.instance.GetCardFrameWithID(SocialManager.instance.playerInfo.equipCardFrame);

        imgFront.sprite = _frame.frontSprite;
        imgBack.sprite = _frame.backSprite;
        txtCardName.color = _frame.nameColor;

        if (this is CardDP_Main)
            imgBack.gameObject.SetActive(!cCard.own);
        txtCardName.text = cCard.stat.strCardName;
        imgIllust.sprite = cCard.stat.imgCardIcon;
        imgMark.sprite = CardManager.instance.cardTypeIcons[(int)cCard.stat.eCardType];
        imgTearGem.sprite = CardManager.instance.cardGems[(int)cCard.stat.eCardTear];
        imgClassIcon.sprite = CardManager.instance.cardClassIcons[(int)cCard.stat.eCardClass];
        txtLevel.text = cCard.stat.iLevel.ToString();
        costFrame.sprite = CardManager.instance.costSpriteList[(int)cCard.stat.eCostType];
        txtCost.text = cCard.stat.iCost.ToString();
        if (MainSceneManager.instance != null)
            gameObject.SetActive(_cCard.own || MainSceneManager.instance.cardDeckPanel.createOn);
    }

    public void OnOffGlow(bool _bOn)
    {
        imgSelect.gameObject.SetActive(_bOn);
    }

    public Card card { get => cCard; }
    public CanvasGroup cv { get => cCanvasGroup; }
    public CardInfoDisplay cardInfoDP { get => cardInfo; }
}
