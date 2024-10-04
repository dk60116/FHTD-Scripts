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
        string _strPSynergy = "�Ʊ� ";
        string _strESynergy = "�� ";

        switch (_sSynergy.strSynergy)
        {
            case "Human":
                _strSynergy += "�ΰ�";
                _strPSynergy += "�ΰ�";
                _strESynergy += "�ΰ�";
                break;
            case "Beast":
                _strSynergy += "�߼�";
                _strPSynergy += "�߼�";
                _strESynergy += "�ΰ�";
                break;
            case "Dragon":
                _strSynergy += "�巡��";
                _strPSynergy += "�巡��";
                _strESynergy += "�巡��";
                break;
            case "Dragonoid":
                _strSynergy += "����";
                _strPSynergy += "����";
                _strESynergy += "����";
                break;
            case "Elemental":
                _strSynergy += "����";
                _strPSynergy += "����";
                _strESynergy += "����";
                break;
            case "Fairy":
                _strSynergy = "����";
                _strPSynergy = "����";
                _strESynergy += "����";
                break;
            case "Undead":
                _strSynergy += "�𵥵�";
                _strPSynergy += "�𵥵�";
                _strESynergy += "�𵥵�";
                break;
            case "Plant":
                _strSynergy += "�Ĺ�";
                _strPSynergy += "�Ĺ�";
                _strESynergy += "�Ĺ�";
                break;
            case "Cavalry":
                _strSynergy += "�⺴��";
                _strPSynergy += "�⺴��";
                _strESynergy += "�⺴��";
                break;
            case "Guardian":
                _strSynergy += "��ȣ��";
                _strPSynergy += "��ȣ��";
                _strESynergy += "��ȣ��";
                break;
            case "Berserker":
                _strSynergy += "������";
                _strPSynergy += "������";
                _strESynergy += "������";
                break;
            case "Magician":
                _strSynergy += "������";
                _strPSynergy += "������";
                _strESynergy += "������";
                break;
            case "Flying":
                _strSynergy += "����";
                _strPSynergy += "����";
                _strESynergy += "����";
                break;
            case "Armoric":
                _strSynergy += "ö��";
                _strPSynergy += "ö��";
                _strESynergy += "ö��";
                break;
            case "TwinSwords":
                _strSynergy += "�ְ�";
                _strPSynergy += "�ְ�";
                _strESynergy += "�ְ�";
                break;
            case "Insect":
                _strSynergy += "����";
                _strPSynergy += "����";
                _strESynergy += "����";
                break;
            case "Fighter":
                _strSynergy += "�ο��";
                _strPSynergy += "�ο��";
                _strESynergy += "�ο��";
                break;
            case "Natual":
                _strSynergy += "�ڿ�";
                _strPSynergy += "�ڿ�";
                _strESynergy += "�ڿ�";
                break;
            case "Swordsman":
                _strSynergy += "�˻�";
                _strPSynergy += "�˻�";
                _strESynergy += "�˻�";
                break;
            case "BigHorns":
                _strSynergy += "ū��";
                _strPSynergy += "ū��";
                _strESynergy += "ū��";
                break;
            case "Ranger":
                _strSynergy += "���Ÿ�";
                _strPSynergy += "���Ÿ�";
                _strESynergy += "���Ÿ�";
                break;
            case "Priest":
                _strSynergy += "����";
                _strPSynergy += "����";
                _strESynergy += "����";
                break;
        }

        string _strResult = _sSynergy.strSynergyDescription.Replace("<Synergy>", _strSynergy);
        _strResult = _strResult.Replace("<PSynergy>", _strPSynergy);
        _strResult = _strResult.Replace("<ESynergy>", _strESynergy);
        _strResult = _strResult.Replace("<Hero>", _sSynergy.strSynergyName);
        _strResult = _strResult.Replace("<Unit>", "����");
        _strResult = _strResult.Replace("<PUnit>", "�Ʊ� ����");
        _strResult = _strResult.Replace("<EUnit>", "�� ����");
        _strResult = _strResult.Replace("<Units>", "���ֵ�");
        _strResult = _strResult.Replace("<PUnits>", "�Ʊ� ���ֵ�");
        _strResult = _strResult.Replace("<EUnits>", "�� ���ֵ�");
        _strResult = _strResult.Replace("<Tile>", "Ÿ��");
        _strResult = _strResult.Replace("<Fly>", "����");

        _strResult = _strResult.Replace("<AD>", "���� ����");
        _strResult = _strResult.Replace("<AP>", "���� ����");
        _strResult = _strResult.Replace("<True>", "���� ����");

        _strResult = _strResult.Replace("_CLevel", "ķ��*����");
        _strResult = _strResult.Replace("_ULevel", "���ַ���");
        _strResult = _strResult.Replace("<_Attack>", "���ݷ�");
        _strResult = _strResult.Replace("<+Attack>", "�߰� ���ݷ�");
        _strResult = _strResult.Replace("<_Power>", "�ֹ���");
        _strResult = _strResult.Replace("<+Power>", "�߰� �ֹ���");
        _strResult = _strResult.Replace("<_MHP>", "�ִ�ü��");
        _strResult = _strResult.Replace("<_HP>", "����ü��");
        _strResult = _strResult.Replace("<+HP>", "�߰� ü��");
        _strResult = _strResult.Replace("<Shield>", "��ȣ��");
        _strResult = _strResult.Replace("<_MP>", "����");
        _strResult = _strResult.Replace("<+MP>", "�߰� ����");
        _strResult = _strResult.Replace("<_ADDef>", "����");
        _strResult = _strResult.Replace("<_APDef>", "�������׷�");
        _strResult = _strResult.Replace("<_AtkSpd>", "���ݼӵ�");
        _strResult = _strResult.Replace("<+AtkSpd>", "�߰� ���ݼӵ�");
        _strResult = _strResult.Replace("<Range>", "��Ÿ�");
        _strResult = _strResult.Replace("<+Damage>", "�߰� ����");

        _strResult = _strResult.Replace("<Slow>", "��ȭ");
        _strResult = _strResult.Replace("<Stun>", "����");
        _strResult = _strResult.Replace("<Silent>", "ħ��");

        _strResult = _strResult.Replace("<Percent>", "%");
        _strResult = _strResult.Replace("<Block>", "ĭ");
        _strResult = _strResult.Replace("<Gold>", "���");

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
