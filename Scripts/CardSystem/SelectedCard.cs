using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Net.NetworkInformation;
using System.Linq;

public class SelectedCard : MonoBehaviour
{
    [SerializeField]
    private Card cCard;
    [SerializeField]
    private Image imgFrame, imgIllust, imgTypeIcon, imgClassIcon;
    [SerializeField]
    private TextMeshProUGUI txtCardName, txtLevel;
    [SerializeField]
    private Image selectImage;
    private Button btn;
    private CanvasGroup cg;

    void Awake()
    {
        btn = GetComponent<Button>();
        cg = GetComponent<CanvasGroup>();
    }

    void Start()
    {
        imgIllust.color = new Color(1f, 1f, 1f, 0);
        txtCardName.text = string.Empty;
        txtLevel.text = string.Empty;

        btn.onClick.AddListener(() => MainSceneManager.instance.cardDeckPanel.FindCard(card.stat.iCardID));
    }

    public void SetCard(Card _cCard)
    {
        if (btn == null)
            return;

        cCard = _cCard;
        int _count = CardManager.instance.deckCard.Where(x => x.stat.iCardID == _cCard.stat.iCardID).Count();
        btn.image.color = _cCard.own && _count <= _cCard.count ? Color.white : Color.red;
        imgTypeIcon.sprite = _cCard.stat.eCardType == CardType.Equip ? CardManager.instance.equipIcon : CardManager.instance.spellIcon;
        imgIllust.color = Color.white;
        imgIllust.sprite = _cCard.stat.imgCardIcon;
        txtCardName.text = _cCard.stat.strCardName;
        txtLevel.text = _cCard.stat.iLevel.ToString();
        imgClassIcon.sprite = CardManager.instance.cardClassIcons[(int)_cCard.stat.eCardClass];
    }

    public void ResetCard()
    {
        try
        {
            int _count = CardManager.instance.deckCard.Where(x => x.stat.iCardID == cCard.stat.iCardID).Count();
            btn.image.color = cCard.own && _count <= cCard.count ? Color.white : Color.red;
        }
        catch {}
    }

    public void OnOffSelect(bool _on)
    {
        selectImage.gameObject.SetActive(_on);
    }

    public Card card { get => cCard; }
}
