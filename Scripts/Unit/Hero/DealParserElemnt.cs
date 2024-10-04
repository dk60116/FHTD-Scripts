using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DealParserElemnt : MonoBehaviour
{
    [SerializeField, ReadOnlyInspector]
    private DealParserPanel cParserPanel;

    [SerializeField, ReadOnlyInspector]
    private Hero cHero;

    [SerializeField]
    private Transform tfDisplay;
    [SerializeField]
    private Image imgHeroFrame, imgHeroFace;
    [SerializeField]
    private TextMeshProUGUI txtTotalDmg;
    [SerializeField]
    private RectTransform tfDmgBar;
    [SerializeField]
    private Image imgAdDamage, imgApDamage, imgTrueDamage;
    private const float fBarXSize = 184f;
    [SerializeField, ReadOnlyInspector]
    private float fTotalDmg, fAdDmg, fApDmg, fTrueDmg;
    [SerializeField, ReadOnlyInspector]
    private int iRank;

    void Awake()
    {
        tfDisplay = transform.GetChild(0).GetComponent<Transform>();
        cParserPanel = GetComponentInParent<DealParserPanel>(true);
    }

    public void InitHero(Hero _cHero)
    {
        cHero = _cHero;
        cHero.SetDealParser(this);
        imgHeroFrame.sprite = cParserPanel.GetHeroFrameSprite(_cHero.unitLevel);
        imgHeroFace.sprite = _cHero.faceIllust;
        fTotalDmg = fAdDmg = fApDmg = fTrueDmg = 0;
        imgAdDamage.fillAmount = imgApDamage.fillAmount = imgTrueDamage.fillAmount = 0;
        UpdateTotalDamage();
        tfDmgBar.sizeDelta = new Vector2(0, 20f);
    }

    public void AddDamage(int _iValue, DamageType _eDamageType)
    {
        cParserPanel.AddTotalDamage(_iValue);
        fTotalDmg += _iValue;

        Dictionary<DamageType, float> damageDict = new Dictionary<DamageType, float>
        {
            { DamageType.AD, fAdDmg },
            { DamageType.AP, fApDmg },
            { DamageType.True, fTrueDmg }
        };

        damageDict[_eDamageType] += _iValue;

        fAdDmg = damageDict[DamageType.AD];
        fApDmg = damageDict[DamageType.AP];
        fTrueDmg = damageDict[DamageType.True];

        UpdateTotalDamage();
        cParserPanel.UpdateAllUnit();
    }

    private void UpdateTotalDamage()
    {
        txtTotalDmg.text = fTotalDmg.ToString();
    }

    public void UpdateDamage()
    {
        if (!gameObject.activeSelf)
            return;

        tfDmgBar.sizeDelta = new Vector2(fTotalDmg / cParserPanel.firstDamage * fBarXSize, imgTrueDamage.rectTransform.sizeDelta.y);
        imgAdDamage.fillAmount = fAdDmg / fTotalDmg;
        imgApDamage.fillAmount = (fAdDmg + fApDmg) / fTotalDmg;
        imgTrueDamage.fillAmount = (fAdDmg + fApDmg + fTrueDmg) / fTotalDmg;
    }

    public void SetRank(int _iValue)
    {
        iRank = _iValue;
    }

    public Transform display { get => tfDisplay; }
    public float totalDamage { get => fTotalDmg; }
    public int rank { get => iRank; }
}
