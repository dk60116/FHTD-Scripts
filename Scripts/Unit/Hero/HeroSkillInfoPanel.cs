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

        _strResult = _strResult.Replace("<AD>", "물리 피해");
        _strResult = _strResult.Replace("<AP>", "마법 피해");
        _strResult = _strResult.Replace("<True>", "고정 피해");

        _strResult = _strResult.Replace("<BaseAttack>", "기본공격");
        _strResult = _strResult.Replace("<Buff>", "버프");

        _strResult = _strResult.Replace("<Unit>", "유닛");
        _strResult = _strResult.Replace("<Units>", "유닛들");
        _strResult = _strResult.Replace("<Ally>", "아군");
        _strResult = _strResult.Replace("<Allys>", "아군들");
        _strResult = _strResult.Replace("<Enemy>", "적");
        _strResult = _strResult.Replace("<Enemys>", "적들");
        _strResult = _strResult.Replace("<Target>", "대상");
        _strResult = _strResult.Replace("<PlayerCamp>", "아군 기지");
        _strResult = _strResult.Replace("<EnemyCamp>", "적 기지");
        _strResult = _strResult.Replace("<Gold>", "골드");

        _strResult = _strResult.Replace("<Level>", cUnit.unitLevel.ToString());
        _strResult = _strResult.Replace("<Attack>", (cUnit.unitStat.iCrtAttack + cUnit.unitStat.iAddAttack).ToString());
        _strResult = _strResult.Replace("<Power>", ((cUnit.unitStat.iCrtAbilPower + cUnit.unitStat.iAddPower) * 0.01f).ToString());
        _strResult = _strResult.Replace("<MHP>", cUnit.unitStat.iMaxHp.ToString());
        _strResult = _strResult.Replace("<ADDef>", (cUnit.unitStat.iCrtADDef + cUnit.unitStat.iAddADDef).ToString());
        _strResult = _strResult.Replace("<APDef>", (cUnit.unitStat.iCrtAPDef + cUnit.unitStat.iAddAPDef).ToString());

        _strResult = _strResult.Replace("<_Attack>", "공격력");
        _strResult = _strResult.Replace("<+Attack>", "추가 공격력");
        _strResult = _strResult.Replace("<_Power>", "주문력");
        _strResult = _strResult.Replace("<+Power>", "추가 주문력");
        _strResult = _strResult.Replace("<_HP>", "체력");
        _strResult = _strResult.Replace("<_MHP>", "최대체력");
        _strResult = _strResult.Replace("<_CHP>", "현재체력");
        _strResult = _strResult.Replace("<+HP>", "추가 체력");
        _strResult = _strResult.Replace("<Shield>", "보호막");
        _strResult = _strResult.Replace("<_MP>", "마력");
        _strResult = _strResult.Replace("<+MP>", "추가 마력");
        _strResult = _strResult.Replace("<_AtkSpd>", "공격속도");
        _strResult = _strResult.Replace("<_MoveSpd>", "이동속도");
        _strResult = _strResult.Replace("<_ADDef>", "방어력");
        _strResult = _strResult.Replace("<+ADDef>", "추가 방어력");
        _strResult = _strResult.Replace("<_APDef>", "마법저항력");
        _strResult = _strResult.Replace("<+APDef>", "추가 마법저항력");
        _strResult = _strResult.Replace("<_Range>", "사거리");

        _strResult = _strResult.Replace("<Slow>", "둔화");
        _strResult = _strResult.Replace("<Stun>", "기절");
        _strResult = _strResult.Replace("<Stiff>", "속박");
        _strResult = _strResult.Replace("<Silent>", "침묵");
        _strResult = _strResult.Replace("<Airborne>", "공중에 띄");
        _strResult = _strResult.Replace("<HealAmount>", "치유량");
        _strResult = _strResult.Replace("<Unspecified>", "지정불가");

        _strResult = _strResult.Replace("<Percent>", "%");
        _strResult = _strResult.Replace("<Block>", "칸");

        _strResult = _strResult.Replace("<Second>", "초");

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
