using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HeroSkillInfoPanel : MonoBehaviour
{
    [SerializeField, ReadOnlyInspector]
    private RectTransform tfRect;
    [SerializeField, ReadOnlyInspector]
    private TextMeshProUGUI txtName, txtDetail;
    [SerializeField, ReadOnlyInspector]
    private Unit cUnit;

    private void Update()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(txtDetail.rectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(tfRect);
    }

    [ContextMenu("Init")]
    private void Init()
    {
        tfRect = GetComponent<RectTransform>();
        txtName = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        txtDetail = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }

    public void SetText(Hero _cHero, bool _bIsAttack)
    {
        cUnit = _cHero;

        switch (_bIsAttack)
        {
            case true:
                txtName.text = _cHero.skill.skillComponent_Atk.strSkillName;
                txtDetail.text = FormatText(_cHero.skill.skillComponent_Atk);
                break;
            case false:
                txtName.text = _cHero.skill.skillComponent_Def.strSkillName;
                txtDetail.text = FormatText(_cHero.skill.skillComponent_Def);
                break;
        }
    }

    private string FormatText(HeroSkillComponent _sSkill)
    {
        string _strResult = _sSkill.strExplanation.Replace("<Hero>", _sSkill.strHeroName);

        _strResult = _strResult.Replace("<AD>", "���� ����");
        _strResult = _strResult.Replace("<AP>", "���� ����");
        _strResult = _strResult.Replace("<True>", "���� ����");

        _strResult = _strResult.Replace("<BaseAttack>", "�⺻����");
        _strResult = _strResult.Replace("<Buff>", "����");

        _strResult = _strResult.Replace("<Unit>", "����");
        _strResult = _strResult.Replace("<Units>", "���ֵ�");
        _strResult = _strResult.Replace("<Ally>", "�Ʊ�");
        _strResult = _strResult.Replace("<Allys>", "�Ʊ���");
        _strResult = _strResult.Replace("<Enemy>", "��");
        _strResult = _strResult.Replace("<Enemys>", "����");
        _strResult = _strResult.Replace("<Target>", "���");
        _strResult = _strResult.Replace("<PlayerCamp>", "�Ʊ� ����");
        _strResult = _strResult.Replace("<EnemyCamp>", "�� ����");
        _strResult = _strResult.Replace("<Gold>", "���");

        _strResult = _strResult.Replace("<Level>", cUnit.unitLevel.ToString());
        _strResult = _strResult.Replace("<Attack>", (cUnit.unitStat.iCrtAttack + cUnit.unitStat.iAddAttack).ToString());
        _strResult = _strResult.Replace("<Power>", ((cUnit.unitStat.iCrtAbilPower + cUnit.unitStat.iAddPower) * 0.01f).ToString());
        _strResult = _strResult.Replace("<MHP>", cUnit.unitStat.iMaxHp.ToString());
        _strResult = _strResult.Replace("<ADDef>", (cUnit.unitStat.iCrtADDef + cUnit.unitStat.iAddADDef).ToString());
        _strResult = _strResult.Replace("<APDef>", (cUnit.unitStat.iCrtAPDef + cUnit.unitStat.iAddAPDef).ToString());

        _strResult = _strResult.Replace("<_Attack>", "���ݷ�");
        _strResult = _strResult.Replace("<+Attack>", "�߰� ���ݷ�");
        _strResult = _strResult.Replace("<_Power>", "�ֹ���");
        _strResult = _strResult.Replace("<+Power>", "�߰� �ֹ���");
        _strResult = _strResult.Replace("<_HP>", "ü��");
        _strResult = _strResult.Replace("<_MHP>", "�ִ�ü��");
        _strResult = _strResult.Replace("<_CHP>", "����ü��");
        _strResult = _strResult.Replace("<+HP>", "�߰� ü��");
        _strResult = _strResult.Replace("<Shield>", "��ȣ��");
        _strResult = _strResult.Replace("<_MP>", "����");
        _strResult = _strResult.Replace("<+MP>", "�߰� ����");
        _strResult = _strResult.Replace("<_AtkSpd>", "���ݼӵ�");
        _strResult = _strResult.Replace("<_MoveSpd>", "�̵��ӵ�");
        _strResult = _strResult.Replace("<_ADDef>", "����");
        _strResult = _strResult.Replace("<+ADDef>", "�߰� ����");
        _strResult = _strResult.Replace("<_APDef>", "�������׷�");
        _strResult = _strResult.Replace("<+APDef>", "�߰� �������׷�");
        _strResult = _strResult.Replace("<_Range>", "��Ÿ�");

        _strResult = _strResult.Replace("<Slow>", "��ȭ");
        _strResult = _strResult.Replace("<Stun>", "����");
        _strResult = _strResult.Replace("<Stiff>", "�ӹ�");
        _strResult = _strResult.Replace("<Silent>", "ħ��");
        _strResult = _strResult.Replace("<Airborne>", "���߿� ��");
        _strResult = _strResult.Replace("<HealAmount>", "ġ����");
        _strResult = _strResult.Replace("<Unspecified>", "�����Ұ�");

        _strResult = _strResult.Replace("<Percent>", "%");
        _strResult = _strResult.Replace("<Block>", "ĭ");

        _strResult = _strResult.Replace("<Second>", "��");

        _strResult = _strResult.Replace("<*>", "*");
        _strResult = _strResult.Replace("</>", "/");
        _strResult = _strResult.Replace("<+>", "+");
        _strResult = _strResult.Replace("<->", "-");

        _strResult = _strResult.Replace("<E>", "\n");
        _strResult = _strResult.Replace(";", ",");

        for (int i = 0; i < _sSkill.listValue.Count; i++)
        {
            int _iIndex = i;

            if (_sSkill.listValue[i].value.Length <= 1)
                _strResult = _strResult.Replace($"<Value0{_iIndex}>", _sSkill.listValue[_iIndex].value[0].ToString());
            if (_sSkill.listValue[i].value.Length > 1)
                _strResult = _strResult.Replace($"<Value0{_iIndex}>", _sSkill.listValue[_iIndex].value[cUnit.unitLevel - 1].ToString());
        }

        string _strFinal_0 = Tools.GetMiddleString(_strResult, "[", "]");
        string _strFinal_1 = Tools.GetMiddleString(_strResult, "|", "|");
        _strResult = _strResult.Replace($"[{_strFinal_0}]", _sSkill.listFinalValue[0].ToString());
        _strResult = _strResult.Replace($"|{_strFinal_1}|", _sSkill.listFinalValue[1].ToString());

        return _strResult;
    }
    
    public Unit unit { get => cUnit; }
}
