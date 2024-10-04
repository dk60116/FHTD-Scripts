using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardDP_InGame : CardDisplay, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IBeginDragHandler
{
    void Awake()
    {
        tfRect = GetComponent<RectTransform>();
        cCanvasGroup = GetComponent<CanvasGroup>();
        iLayerMask_Tile = 1 << LayerMask.NameToLayer("Tile");
    }

    void Update()
    {
        if (Input.GetMouseButton(0) && Vector2.Distance(v2ClickPos, Input.mousePosition) > 100f)
            bSelect = true;

        if (InGameManager.instance.cCardController.crtCard == this && !InGameManager.instance.cCardController.casting)
        {
            if (Input.GetMouseButton(0) && bSelect)
            {
                GameManager.instance.CloseCardInfo();
                InGameManager.instance.cCardController.AllOffScope();

                tfRect.position = Input.mousePosition;
                tfRect.rotation = Quaternion.identity;
                InGameManager.instance.cCardController.HideCardBundle();
                cCanvasGroup.alpha = cCard.stat.eCastTarget == CardUseTarget.Field ? 0 : 0.25f;

                switch (cCard.stat.eCastTarget)
                {
                    case CardUseTarget.Anywhere:
                        InGameManager.instance.cCardController.OnOffAnyPanel(true);
                        break;
                    case CardUseTarget.Field:
                        InGameManager.instance.cCardController.OnOffScope((int)cCard.stat.eScopeType - 1, true);
                        rayMouseHit_Tile = Camera.main.ScreenPointToRay(Input.mousePosition);
                        if (Physics.Raycast(rayMouseHit_Tile, out rayHit_Tile, 100f, iLayerMask_Tile))
                        {
                            InGameManager.instance.cCardController.MoveScope((int)cCard.stat.eScopeType - 1, new Vector3(rayHit_Tile.point.x, 0.51f, rayHit_Tile.point.z));
                            InGameManager.instance.cCardController.SetScopeColor((int)cCard.stat.eScopeType - 1, (rayHit_Tile.transform.CompareTag("Tile") && !rayHit_Tile.transform.GetComponent<Tile>().bSlot) ? Color.blue : Color.red);
                        }
                        break;
                    case CardUseTarget.Tile:
                        break;
                    case CardUseTarget.Unit:
                        break;
                    case CardUseTarget.FriendlyUnit:
                        break;
                    case CardUseTarget.EnemyUnit:
                        break;
                    case CardUseTarget.Hero:
                        break;
                    case CardUseTarget.FriendlyHero:
                        foreach (var item in InGameManager.instance.cPlayerController.ownHeros)
                        {
                            item.ChangeOutlineColor(Color.green);
                            item.SwitchOutLine(true);
                        }
                        InGameManager.instance.cCardController.SetEquip(true);

                        if (InGameManager.instance.crtOverTile != null && InGameManager.instance.crtOverTile.isUnit)
                            InGameManager.instance.crtOverTile.placedUnit.ChangeOutlineColor(Color.blue);
                        break;
                    case CardUseTarget.EnemyHero:

                        break;
                    case CardUseTarget.Obstacle:
                        break;
                }
            }
        }
    }

    [ContextMenu("Init")]
    protected void Init()
    {
        InGameManager.instance.cCardController = GetComponentInParent<CardController>();
    }

    public override void UpdateCardInfo(Card _cCard)
    {
        base.UpdateCardInfo(_cCard);
        cAbility.SetCard();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        InGameManager.instance.cCardController.SetCurrentCard(this);
        v2OrgPos = tfRect.anchoredPosition;
        v3OrgRot = tfRect.eulerAngles;
        v2ClickPos = Input.mousePosition;
        bSelect = false;
        InGameManager.instance.cHeroInfoPanel.OnOffPanel(false);
        InGameManager.instance.cEnemyCampInfoPanel.OnOffPanel(false);

        foreach (var item in FindObjectsOfType<Unit>())
            item.SwitchOutLine(false);

        foreach (var item in InGameManager.instance.cPlayerController.ownUnits)
        {
            if (item is Hero)
                (item as Hero).OnOffRangeCircle(false);
        }

        InGameManager.instance.cCardController.OpenCardInfo(cCard, new Vector2(transform.position.x + 100f, 30f), Vector2.zero);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        InGameManager.instance.cCardController.SetCurrentCard(null);
        InGameManager.instance.cCardController.AllOffScope();
        tfRect.anchoredPosition = v2OrgPos;
        tfRect.rotation = Quaternion.Euler(v3OrgRot);
        cCanvasGroup.alpha = 1f;
        v2ClickPos = Vector2.zero;

        if (bSelect && CheckAbleCast())
        {
            if (CheckCastCondition() && !InGameManager.instance.cCardController.casting)
                cAbility.cardCast.Invoke();
        }

        InGameManager.instance.cCardController.OnOffAnyPanel(false);

        foreach (var item in InGameManager.instance.cPlayerController.ownHeros)
            item.SwitchOutLine(false);

        InGameManager.instance.cCardController.SetEquip(false);

        GameManager.instance.CloseCardInfo();

        Invoke(nameof(FalseSelect), Time.deltaTime);
    }

    public bool CheckCastCondition()
    {
        if (!bSelect)
            return false;

        bool _bResult = true;

        if (cCard.stat.iCardID == 301006)
        {
            if ((int)InGameManager.instance.cEditMap.stayTile.placedUnit.unitStat.eRank > 2)
            {
                GameManager.instance.OpenNoticePanel("Hero's rank is too high");
                InGameManager.instance.cCardController.AllOffGlow();
                return false;
            }

            if (!InGameManager.instance.CheckNotSlotFull())
                _bResult = false;
        }

        if (!InGameManager.instance.cPlayerController.CheckCampLevel(cCard.stat.iLevel))
            return false;

        bool _bGoldResult = InGameManager.instance.cPlayerController.TryUseGold(cCard.stat.iCost, _bResult);

        if (!_bGoldResult)
            _bResult = false;

        return _bResult;
    }

    protected bool CheckAbleCast()
    {
        bool _bResult = true;

        switch (cCard.stat.eCastTiming)
        {
            case CardUseTimiing.AnyTime:
                break;
            case CardUseTimiing.RunTime:
                if (InGameManager.instance.status != StageStatus.Running)
                {
                    GameManager.instance.OpenNoticePanel("It can only be used while running");
                    InGameManager.instance.cCardController.AllOffGlow();
                    return false;
                }
                break;
            case CardUseTimiing.Preparation:
                break;
        }

        switch (cCard.stat.eCastTarget)
        {
            case CardUseTarget.Anywhere:
                _bResult = InGameManager.instance.cCardController.any;
                break;
            case CardUseTarget.Field:
                if (InGameManager.instance.cEditMap.stayTile == null)
                {
                    GameManager.instance.OpenNoticePanel("Use over tiles.");
                    InGameManager.instance.cCardController.AllOffGlow();
                    return false;
                }
                break;
            case CardUseTarget.Tile:
            case CardUseTarget.Unit:
            case CardUseTarget.FriendlyUnit:
            case CardUseTarget.EnemyUnit:
            case CardUseTarget.Hero:
            case CardUseTarget.FriendlyHero:
                if (InGameManager.instance.cCardController.useHero)
                {
                    if (InGameManager.instance.cEditMap.stayTile == null || !InGameManager.instance.cEditMap.stayTile.isUnit)
                        return false;

                    if (cCard.stat.eCardClass == CardClass.Weapon && (InGameManager.instance.cEditMap.stayTile.placedUnit as Hero).equipBool[0])
                    {
                        GameManager.instance.OpenNoticePanel("Weapon is already equipped.");
                        InGameManager.instance.cCardController.AllOffGlow();
                        return false;
                    }
                    if (cCard.stat.eCardClass == CardClass.Armor && (InGameManager.instance.cEditMap.stayTile.placedUnit as Hero).equipBool[1])
                    {
                        GameManager.instance.OpenNoticePanel("Armor is already equipped.");
                        InGameManager.instance.cCardController.AllOffGlow();
                        return false;
                    }
                    if (cCard.stat.eCardClass == CardClass.Accessories && (InGameManager.instance.cEditMap.stayTile.placedUnit as Hero).equipBool[2])
                    {
                        GameManager.instance.OpenNoticePanel("Accessorie is already equipped.");
                        InGameManager.instance.cCardController.AllOffGlow();
                        return false;
                    }

                    if (InGameManager.instance.cEditMap.stayTile.placedUnit.attackUnit)
                        return false;
                }
                else _bResult = false;
                break;
            case CardUseTarget.EnemyHero:
            case CardUseTarget.Obstacle:
                break;
        }

        return _bResult;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!bSelect)
            InGameManager.instance.cCardController.AbleCardBundle();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        gameObject.SetActive(false);
    }

    protected void FalseSelect()
    {
        bSelect = false;
    }

    public RaycastHit rayHit { get => rayHit_Tile; }
}
