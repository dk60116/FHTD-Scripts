using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SynergyDescriptionPanel : MonoBehaviour
{
    [SerializeField, ReadOnlyInspector]
    private TextMeshProUGUI txtTitle, txtComment;
    [SerializeField, ReadOnlyInspector]
    private RectTransform tfRect;

    [ContextMenu("Init")]
    private void Init()
    {
        tfRect = GetComponent<RectTransform>();
        txtTitle = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        txtComment = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }

    public void UpdateTribe(HeroTribe _eTribe)
    {
        UpdateSynergy(GameManager.instance.cSynergySystem.GetNumberWithTribe(_eTribe));
    }

    public void UpdateClass(HeroClass _eClass)
    {
        UpdateSynergy(GameManager.instance.cSynergySystem.GetNumberWithClass(_eClass));
    }

    public void UpdateSynergy(int _iNumber)
    {
        txtTitle.text = GameManager.instance.cSynergySystem.synergyStatusList[_iNumber].strSynergyName;
        txtTitle.color = GameManager.instance.cSynergySystem.IsSynergyActive(_iNumber, false) ? Color.white : Color.gray;
        txtComment.text = FormatText(GameManager.instance.cSynergySystem.synergyStatusList[_iNumber]);

        LayoutRebuilder.ForceRebuildLayoutImmediate(txtTitle.rectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(txtComment.rectTransform);
    }

    private string FormatText(SynergyStatus _sSynergy)
    {
        string _strSynergy = string.Empty;
        string _strPSynergy = "아군 ";
        string _strESynergy = "적 ";

        switch (_sSynergy.strSynergy)
        {
            case "Human":
                _strSynergy += "인간";
                _strPSynergy += "인간";
                _strESynergy += "인간";
                break;
            case "Beast":
                _strSynergy += "야수";
                _strPSynergy += "야수";
                _strESynergy += "인간";
                break;
            case "Dragon":
                _strSynergy += "드래곤";
                _strPSynergy += "드래곤";
                _strESynergy += "드래곤";
                break;
            case "Dragonoid":
                _strSynergy += "용인";
                _strPSynergy += "용인";
                _strESynergy += "용인";
                break;
            case "Elemental":
                _strSynergy += "용인";
                _strPSynergy += "용인";
                _strESynergy += "용인";
                break;
            case "Fairy":
                _strSynergy = "요정";
                _strPSynergy = "요정";
                _strESynergy += "요정";
                break;
            case "Undead":
                _strSynergy += "언데드";
                _strPSynergy += "언데드";
                _strESynergy += "언데드";
                break;
            case "Plant":
                _strSynergy += "식물";
                _strPSynergy += "식물";
                _strESynergy += "식물";
                break;
            case "Cavalry":
                _strSynergy += "기병대";
                _strPSynergy += "기병대";
                _strESynergy += "기병대";
                break;
            case "Guardian":
                _strSynergy += "수호자";
                _strPSynergy += "수호자";
                _strESynergy += "수호자";
                break;
            case "Berserker":
                _strSynergy += "광전사";
                _strPSynergy += "광전사";
                _strESynergy += "광전사";
                break;
            case "Magician":
                _strSynergy += "마법사";
                _strPSynergy += "마법사";
                _strESynergy += "마법사";
                break;
            case "Flying":
                _strSynergy += "비행";
                _strPSynergy += "비행";
                _strESynergy += "비행";
                break;
            case "Armoric":
                _strSynergy += "철갑";
                _strPSynergy += "철갑";
                _strESynergy += "철갑";
                break;
            case "TwinSwords":
                _strSynergy += "쌍검";
                _strPSynergy += "쌍검";
                _strESynergy += "쌍검";
                break;
            case "Insect":
                _strSynergy += "벌레";
                _strPSynergy += "벌레";
                _strESynergy += "벌레";
                break;
            case "Fighter":
                _strSynergy += "싸움꾼";
                _strPSynergy += "싸움꾼";
                _strESynergy += "싸움꾼";
                break;
            case "Natual":
                _strSynergy += "자연";
                _strPSynergy += "자연";
                _strESynergy += "자연";
                break;
            case "Swordsman":
                _strSynergy += "검사";
                _strPSynergy += "검사";
                _strESynergy += "검사";
                break;
            case "BigHorns":
                _strSynergy += "큰뿔";
                _strPSynergy += "큰뿔";
                _strESynergy += "큰뿔";
                break;
            case "Ranger":
                _strSynergy += "원거리";
                _strPSynergy += "원거리";
                _strESynergy += "원거리";
                break;
            case "Priest":
                _strSynergy += "사제";
                _strPSynergy += "사제";
                _strESynergy += "사제";
                break;
        }

        string _strResult = _sSynergy.strSynergyDescription.Replace("<Synergy>", _strSynergy);
        _strResult = _strResult.Replace("<PSynergy>", _strPSynergy);
        _strResult = _strResult.Replace("<ESynergy>", _strESynergy);
        _strResult = _strResult.Replace("<Hero>", _sSynergy.strSynergyName);
        _strResult = _strResult.Replace("<Unit>", "유닛");
        _strResult = _strResult.Replace("<PUnit>", "아군 유닛");
        _strResult = _strResult.Replace("<EUnit>", "적 유닛");
        _strResult = _strResult.Replace("<Units>", "유닛들");
        _strResult = _strResult.Replace("<PUnits>", "아군 유닛들");
        _strResult = _strResult.Replace("<EUnits>", "적 유닛들");
        _strResult = _strResult.Replace("<Tile>", "타일");
        _strResult = _strResult.Replace("<Fly>", "공중");

        _strResult = _strResult.Replace("<AD>", "물리 피해");
        _strResult = _strResult.Replace("<AP>", "마법 피해");
        _strResult = _strResult.Replace("<True>", "고정 피해");

        _strResult = _strResult.Replace("_CLevel", "캠프*레벨");
        _strResult = _strResult.Replace("_ULevel", "유닛레벨");
        _strResult = _strResult.Replace("<_Attack>", "공격력");
        _strResult = _strResult.Replace("<+Attack>", "추가 공격력");
        _strResult = _strResult.Replace("<_Power>", "주문력");
        _strResult = _strResult.Replace("<+Power>", "추가 주문력");
        _strResult = _strResult.Replace("<_MHP>", "최대체력");
        _strResult = _strResult.Replace("<_HP>", "현재체력");
        _strResult = _strResult.Replace("<+HP>", "추가 체력");
        _strResult = _strResult.Replace("<Shield>", "보호막");
        _strResult = _strResult.Replace("<_MP>", "마력");
        _strResult = _strResult.Replace("<+MP>", "추가 마력");
        _strResult = _strResult.Replace("<_ADDef>", "방어력");
        _strResult = _strResult.Replace("<_APDef>", "마법저항력");
        _strResult = _strResult.Replace("<_AtkSpd>", "공격속도");
        _strResult = _strResult.Replace("<+AtkSpd>", "추가 공격속도");
        _strResult = _strResult.Replace("<Range>", "사거리");
        _strResult = _strResult.Replace("<+Damage>", "추가 피해");

        _strResult = _strResult.Replace("<Slow>", "둔화");
        _strResult = _strResult.Replace("<Stun>", "기절");
        _strResult = _strResult.Replace("<Silent>", "침묵");

        _strResult = _strResult.Replace("<Percent>", "%");
        _strResult = _strResult.Replace("<Block>", "칸");
        _strResult = _strResult.Replace("<Gold>", "골드");

        _strResult = _strResult.Replace("<*>", "*");
        _strResult = _strResult.Replace("</>", "/");
        _strResult = _strResult.Replace("<+>", "+");
        _strResult = _strResult.Replace("<->", "-");

        _strResult = _strResult.Replace("<E>", "\n");
        _strResult = _strResult.Replace(";", ",");

        for (int i = 0; i < _sSynergy.listValue.Count; i++)
        {
            int _iIndex = i;
            if (_sSynergy.listValue[_iIndex].value.Length == 1)
            {
                _strResult = _strResult.Replace($"<Value0{_iIndex}>", $"{(GameManager.instance.cSynergySystem.GetOpenSynergyCount(_sSynergy.iNumber, false) == 1 ? "<color=white>" : "<color=grey>")}{_sSynergy.listValue[_iIndex].value[0]}</color>");
                _strResult = _strResult.Replace($"<_Value0{_iIndex}>", _sSynergy.listValue[i].value[0].ToString());
            }
            else if (_sSynergy.listValue[_iIndex].value.Length == 2)
                _strResult = _strResult.Replace($"<Value0{_iIndex}>", $"{(GameManager.instance.cSynergySystem.GetOpenSynergyCount(_sSynergy.iNumber, false) == 1 ? "<color=white>" : "<color=grey>")}{_sSynergy.listValue[_iIndex].value[0]}</color>/{(GameManager.instance.cSynergySystem.GetOpenSynergyCount(_sSynergy.iNumber, false) == 2 ? "<color=white>" : "<color=grey>")}{_sSynergy.listValue[_iIndex].value[1]}</color>");
            else if (_sSynergy.listValue[_iIndex].value.Length == 3)
                _strResult = _strResult.Replace($"<Value0{_iIndex}>", $"{(GameManager.instance.cSynergySystem.GetOpenSynergyCount(_sSynergy.iNumber, false) == 1 ? "<color=white>" : "<color=grey>")}{_sSynergy.listValue[_iIndex].value[0]}</color>/{(GameManager.instance.cSynergySystem.GetOpenSynergyCount(_sSynergy.iNumber, false) == 2 ? "<color=white>" : "<color=grey>")}{_sSynergy.listValue[_iIndex].value[1]}</color>/{(GameManager.instance.cSynergySystem.GetOpenSynergyCount(_sSynergy.iNumber, false) == 3 ? "<color=white>" : "<color=grey>")}{_sSynergy.listValue[_iIndex].value[2]}</color>");
            else if (_sSynergy.listValue[_iIndex].value.Length == 4)
                _strResult = _strResult.Replace($"<Value0{_iIndex}>", $"{(GameManager.instance.cSynergySystem.GetOpenSynergyCount(_sSynergy.iNumber, false) == 1 ? "<color=white>" : "<color=grey>")}{_sSynergy.listValue[_iIndex].value[0]}</color>/{(GameManager.instance.cSynergySystem.GetOpenSynergyCount(_sSynergy.iNumber, false) == 2 ? "<color=white>" : "<color=grey>")}{_sSynergy.listValue[_iIndex].value[1]}</color>/{(GameManager.instance.cSynergySystem.GetOpenSynergyCount(_sSynergy.iNumber, false) == 3 ? "<color=white>" : "<color=grey>")}{_sSynergy.listValue[_iIndex].value[2]}</color>/{(GameManager.instance.cSynergySystem.GetOpenSynergyCount(_sSynergy.iNumber, false) == 4 ? "<color=white>" : "<color=grey>")}{_sSynergy.listValue[_iIndex].value[3]}</color>");
        }

        return _strResult;
    }

    public RectTransform rect { get => tfRect; }
}
