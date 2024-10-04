using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardUseTimiing { AnyTime, RunTime, Preparation }
public enum CardUseTarget { Anywhere, Field, Tile, Unit, FriendlyUnit, EnemyUnit, Hero, FriendlyHero, EnemyHero, Obstacle};

public class CardAbility : MonoBehaviour
{
    [SerializeField,ReadOnlyInspector]
    private CardDP_InGame cCardDisplay;

    [SerializeField]
    private Card cCard;

    public delegate void CardCast();
    public CardCast cardCast;

    private LayerMask cLayerMask;

    private CardAbility tempAbility;

    void Awake()
    {
        cCardDisplay = GetComponent<CardDP_InGame>();
        cLayerMask = 1 << LayerMask.NameToLayer("Unit");
    }

    public void SetCard()
    {
        cCard = cCardDisplay.card;
        cardCast = null;

        cardCast += Cast;
    }

    public Card GetCard()
    {
        return cCard;
    }

    private void Cast()
    {
        InGameManager.instance.cCardController.UseCard(transform.GetSiblingIndex());
        
        Invoke($"Effect_{cCard.stat.iCardID}", 0);

        switch (cCard.stat.eCardType)
        {
            case CardType.Equip:
                EquipCard();
                break;
            case CardType.Spell:
                switch (cCard.stat.eCastTarget)
                {
                    case CardUseTarget.Anywhere:
                        break;
                    case CardUseTarget.Field:
                        Instantiate(cCard.stat.goEffect, cCardDisplay.rayHit.point, Quaternion.identity);
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
                        break;
                    case CardUseTarget.EnemyHero:
                        break;
                    case CardUseTarget.Obstacle:
                        break;
                }
                break;
            default:
                break;
        }

        InGameManager.instance.cCardController.HideCardBundle();
        InGameManager.instance.cCardController.AllOffScope();
    }

    public void EquipCard()
    {
        (InGameManager.instance.cEditMap.stayTile.placedUnit as Hero).EquipmentMounting(cCard.stat.iCardID);
    }

    #region Donation
    public void Effect_301000()
    {
        InGameManager.instance.cPlayerController.AddGold((int)cCard.stat.listValue[0]);
    }
    #endregion

    #region Fire ball
    public void Effect_301005()
    {
        InGameManager.instance.cCardController.casting = true;
        tempAbility = Instantiate(this).GetComponent<CardAbility>();
        tempAbility.gameObject.SetActive(true);
        tempAbility.StartCoroutine(FireBallAttackCoroutine(cCardDisplay.rayHit.point));
    }

    IEnumerator FireBallAttackCoroutine(Vector3 _v3TargetPos)
    {
        yield return new WaitForSeconds(1f);

        Collider[] _targetHeros = Physics.OverlapSphere(_v3TargetPos + Vector3.up * 0.5f, 1f, cLayerMask.value);

        foreach (var item in _targetHeros)
        {
            Hero _cHero = item.GetComponent<Hero>();
            if (_cHero.attackUnit)
                _cHero.AddHP(null, _cHero.CalcDamage(DamageType.AP, null, (int)tempAbility.cCard.stat.listValue[0]), DamageType.AP);
        }

        InGameManager.instance.cCardController.casting = false;

        Destroy(tempAbility.gameObject);
    }
    #endregion

    #region Lesser hero replicator
    public void Effect_301006()
    {
        InGameManager.instance.cCardController.casting = true;
        tempAbility = Instantiate(this).GetComponent<CardAbility>();
        tempAbility.gameObject.SetActive(true);

        tempAbility.StartCoroutine(LesserHeroReplicatorCoroutine(InGameManager.instance.cEditMap.stayTile.placedUnit as Hero));

        InGameManager.instance.cCardController.casting = false;
    }

    IEnumerator LesserHeroReplicatorCoroutine(Hero _cTargetHero)
    {
        InGameManager.instance.cCardController.casting = true;

        yield return new WaitForSeconds(0.5f);

        InGameManager.instance.GetEmptySlotTile().SetNewUnit(InGameManager.instance.cMarketSystem.objPool.GetObj(_cTargetHero.heroNumber).GetComponent<Unit>());
    }
    #endregion
}
