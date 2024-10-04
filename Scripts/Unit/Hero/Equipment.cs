using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;
using UnityEngine.Windows;

public enum EquipType { Weapon, Armor, Accessories }

[Serializable]
public class Equipment
{
    [SerializeField]
    private Card cCard;
    [SerializeField, ReadOnlyInspector]
    private EquipType eType;

    public Equipment(int _iCardId)
    {
        cCard = CardManager.instance.GetCardWithID(_iCardId);

        switch (cCard.stat.eCardClass)
        {
            case CardClass.Weapon:
                eType = EquipType.Weapon;
                break;
            case CardClass.Armor:
                eType = EquipType.Armor;    
                break;
            case CardClass.Accessories:
                eType = EquipType.Accessories;
                break;
        }
    }

    public Card card { get => cCard; }
    public EquipType equipType { get => eType; }
}
