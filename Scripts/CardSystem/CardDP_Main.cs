using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardDP_Main : CardDisplay, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private RectTransform countFrame;
    [SerializeField]
    private TextMeshProUGUI cardCountText;
    [SerializeField]
    private Button createBtn, destroyBtn;
    [SerializeField]
    private CanvasGroup canGroup;

    [SerializeField, ReadOnlyInspector]
    private float clickTime;
    [SerializeField, ReadOnlyInspector]
    private bool isEnter, isMoving;

    void Update()
    {
        if (isEnter)
        {
            if (card.own)
            {
                if (Input.GetMouseButton(0))
                    clickTime += Time.deltaTime;
                else
                    clickTime = 0;
            }
        }
        else
            clickTime = 0;

        if (clickTime >= 0.5f)
        {
            cardInfo.openMovingCard = true;
            clickTime = 6f;
            isMoving = true;
            MainSceneManager.instance.cardDeckPanel.SetMovingCard(true, card);
        }

        if (isMoving)
        {
            if (Input.GetMouseButtonUp(0))
            {
                clickTime = 0;
                isMoving = false;
                cardInfo.openMovingCard = false;
                MainSceneManager.instance.cardDeckPanel.AllOffGlow();
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isEnter = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isEnter = false;
    }

    public override void UpdateCardInfo(Card _cCard)
    {
        base.UpdateCardInfo(_cCard);
        countFrame.gameObject.SetActive(_cCard.own);
        cardCountText.text = _cCard.count > 99 ? "99 +" : $"x {_cCard.count}";
        imgBack.gameObject.SetActive(!_cCard.own && !MainSceneManager.instance.cardDeckPanel.createOn);
        canGroup.alpha = (!_cCard.own && MainSceneManager.instance.cardDeckPanel.createOn) ? 0.3f : 1f;
        createBtn.gameObject.SetActive(_cCard.stat.eCardTear != CardTear.Basic && MainSceneManager.instance.cardDeckPanel.createOn);
        destroyBtn.gameObject.SetActive(_cCard.own && _cCard.stat.eCardTear != CardTear.Basic && MainSceneManager.instance.cardDeckPanel.createOn);
        createBtn.onClick.RemoveAllListeners();
        destroyBtn.onClick.RemoveAllListeners();
        createBtn.onClick.AddListener(() => MainSceneManager.instance.cardDeckPanel.OpenCardConfirm(true, cCard));
        destroyBtn.onClick.AddListener(() => MainSceneManager.instance.cardDeckPanel.OpenCardConfirm(false, cCard));
    }

    public CardInfoDisplay infoDP { get => cardInfo; }
}
