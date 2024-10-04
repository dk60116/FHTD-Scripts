using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardInfoPanel : MonoBehaviour
{
    [SerializeField, ReadOnlyInspector]
    private Card cCard;

    [SerializeField]
    private TextMeshProUGUI txtCardName, txtCardDescription, txtClassName, txtClassInfo, txtLevel, txtCost, txtLevelDescription, txtCostDescription,
        txtTimming, txtTimmingDescription, txtTarget, txtTargetDescription;

    [SerializeField, ReadOnlyInspector]
    private RectTransform tfRect;

    [SerializeField]
    private Image imgClassIcon, imgCostIcon, imgTimingIcon, imgTargetIcon;

    [SerializeField, ReadOnlyInspector]
    private List<string> listClassDescriotion, timmingDescriptionList, targetDescriptionList;
    [SerializeField]
    private List<Sprite> timmingIconList, targetIconList;

[ContextMenu("Init")]
    private void Init()
    {
        listClassDescriotion = new List<string>
        {
            string.Empty,
            "Use it to damage enemies.",
            "Grants a beneficial effect to an ally unit.",
            "Help outside the battle.",
            "Equipment that increases destructive power.",
            "Equipment that increases defense.",
            "Equipment that increases special abilities."
        };

        timmingDescriptionList = new List<string>
        {
            "Always available.",
            "Only available when the round has started.",
            "Only available when preparing a round."
        };

        targetDescriptionList = new List<string>
        {
            "No specific target.",
            "Only available when the round has started.",
            "Only available when preparing a round.",
            "Use anywhere on the battlefield tiles.",
            "Use for one specific tile.",
            "Use for one specific unit.",
            "Use on one specific friendly unit.",
            "Use on one specific enemy unit",
            "Available for elite units that can be purchased in the Store",
            "Available for elite friendly units that can be purchased in the Store",
            "Available for elite enemy units that can be purchased in the Store",
            "Use for things like buildings, not units."
        };
    }

    public void UpdateCard(Card _cCard)
    {
        cCard = _cCard;

        txtCardName.text = cCard.stat.strCardName;
        txtCardDescription.text = FormatText(_cCard.stat);
        imgClassIcon.sprite = CardManager.instance.cardClassIcons[(int)_cCard.stat.eCardClass];
        txtClassName.text = _cCard.stat.eCardClass.ToString();
        txtLevel.text = _cCard.stat.iLevel.ToString();
        txtLevelDescription.text = $"Available at Player camp level <color=red>{_cCard.stat.iLevel}</color> or higher";
        imgCostIcon.sprite = CardManager.instance.costSpriteList[(int)_cCard.stat.eCostType];
        txtCost.text = _cCard.stat.iCost.ToString();
        txtCostDescription.text = _cCard.stat.eCostType == CardCostType.Gold ? $"Costs <color=red>{_cCard.stat.iCost}</color> gold to use." : $"Spent <color=red>{_cCard.stat.iCost}</color> health points to use.";
        imgTimingIcon.sprite = timmingIconList[(int)_cCard.stat.eCastTiming];
        txtTimming.text = _cCard.stat.eCastTiming.ToString();
        txtTimmingDescription.text = timmingDescriptionList[(int)_cCard.stat.eCastTiming];
        imgTargetIcon.sprite = targetIconList[(int)_cCard.stat.eCastTarget];
        txtTarget.text = _cCard.stat.eCastTarget.ToString();
        txtTargetDescription.text = targetDescriptionList[(int)_cCard.stat.eCastTarget];
        try
        {
            txtClassInfo.text = listClassDescriotion[(int)_cCard.stat.eCardClass];
        }
        catch
        {}

        foreach (var item in GetComponentsInChildren<RectTransform>())
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(item);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(tfRect);
    }

    private string FormatText(CardStatus _sCardStat)
    {
        string _strResult = _sCardStat.strCardDescription.Replace("<CardName>", _sCardStat.strCardName);
        _strResult = _strResult.Replace("<AD>", "���� ����");
        _strResult = _strResult.Replace("<AP>", "���� ����");
        _strResult = _strResult.Replace("<True>", "���� ����");

        _strResult = _strResult.Replace("<_Attack>", "���ݷ�");
        _strResult = _strResult.Replace("<+Attack>", "�߰� ���ݷ�");
        _strResult = _strResult.Replace("<_Power>", "�ֹ���");
        _strResult = _strResult.Replace("<+Power>", "�߰� �ֹ���");
        _strResult = _strResult.Replace("<HP>", "ü��");
        _strResult = _strResult.Replace("<+HP>", "�߰� ü��");
        _strResult = _strResult.Replace("<_ADDef>", "����");
        _strResult = _strResult.Replace("<_APDef>", "�������׷�");

        _strResult = _strResult.Replace("<Gold>", "���");

        _strResult = _strResult.Replace("<Slow>", "��ȭ");
        _strResult = _strResult.Replace("<Stun>", "����");
        _strResult = _strResult.Replace("<Silent>", "ħ��");

        _strResult = _strResult.Replace("<Percent>", "%");
        _strResult = _strResult.Replace("<Block>", "ĭ");

        _strResult = _strResult.Replace("<*>", "*");
        _strResult = _strResult.Replace("</>", "/");
        _strResult = _strResult.Replace("<+>", "+");
        _strResult = _strResult.Replace("<->", "-");

        bool _bRatio = false;

        if (_strResult.Contains("<@AtkSpd>"))
            _bRatio = true;

        _strResult = _strResult.Replace("[", string.Empty);
        _strResult = _strResult.Replace("]", string.Empty);
        _strResult = _strResult.Replace("|", string.Empty);
        _strResult = _strResult.Replace("&", string.Empty);
        _strResult = _strResult.Replace("#", string.Empty);
        _strResult = _strResult.Replace("<@Attack>", "���ݷ�");
        _strResult = _strResult.Replace("<@Power>", "�ֹ���");
        _strResult = _strResult.Replace("<@Hp>", "ü��");
        _strResult = _strResult.Replace("<@ADDef>", "����");
        _strResult = _strResult.Replace("<@ADDef>", "����");
        _strResult = _strResult.Replace("<@APDef>", "��������");
        _strResult = _strResult.Replace("<@AtkSpd>", "���ݼӵ�");

        for (int i = 0; i < _sCardStat.listValue.Count; i++)
        {
            int _iIndex = i;

            _strResult = _strResult.Replace($"<Value0{_iIndex}>", _bRatio ? $"{_sCardStat.listValue[_iIndex]}%" : _sCardStat.listValue[_iIndex].ToString());
        }

        return _strResult;
    }

    public void SetPivot(Vector2 _v2Pivot)
    {
        tfRect.pivot = _v2Pivot;
    }
}
