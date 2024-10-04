using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardInfoDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    private bool isOver;

    [SerializeField]
    private bool _bSelectedCard;

    [SerializeField, ReadOnlyInspector]
    private Card cCard;

    [SerializeField, ReadOnlyInspector]
    private CardDisplay cCardDP;
    [SerializeField, ReadOnlyInspector]
    private SelectedCard cSelectCard;
    private RectTransform tfRect;

    [ReadOnlyInspector]
    public bool openMovingCard;

    void Awake()
    {
        if (_bSelectedCard)
        {
            cSelectCard = GetComponent<SelectedCard>();
        }
        else
        {
            cCardDP = GetComponent<CardDisplay>();
        }

        tfRect = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (InGameManager.instance == null && Input.GetMouseButtonDown(0))
        {
            if (!(cCardDP is CardDP_Open))
                GameManager.instance.CloseCardInfo();

            if (cCardDP != null)
                cCardDP.OnOffGlow(isOver);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        cCard = _bSelectedCard ? cSelectCard.card : cCardDP.card;

        Vector2 _v2CardPos = tfRect.position;
        Vector2 _v2AnchorPos = tfRect.anchoredPosition;

        if (cCardDP is CardDP_Main)
        {
            float _fXOffset = 400f;

            float _xResut = 0;
            Vector2 _v2Pivot = Vector2.zero;

            if (_v2CardPos.x <= Screen.width / 2f)
            {
                _v2Pivot.x = 0;
                _xResut = _v2CardPos.x + _fXOffset;
            }
            else
            {
                _v2Pivot.x = 1f;
                _xResut = _v2CardPos.x - _fXOffset;
            }

            if (_v2CardPos.y <= Screen.height / 2f)
                _v2Pivot.y = 0.5f;
            else
                _v2Pivot.y = 0.5f;

            if (!openMovingCard)
                GameManager.instance.OpenCardInfo(cCard, new Vector2(_xResut, Screen.height / 2f), Vector3.one * 0.5f);
        }
        else if (cCardDP is CardDP_InGame)
            GameManager.instance.OpenCardInfo(cCard, new Vector2(Screen.width / 2f, Screen.height / 2f), Vector2.zero);

        cCardDP.OnOffGlow(true);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        cCard = _bSelectedCard ? cSelectCard.card : cCardDP.card;

        GameManager.instance.CloseCardInfo();

        if (MainSceneManager.instance != null)
            MainSceneManager.instance.cardDeckPanel.AllOffGlow();
        if (InGameManager.instance != null)
        {
            if (InGameManager.instance.cCardController != null)
                InGameManager.instance.cCardController.AllOffGlow();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isOver = false;
    }
}
