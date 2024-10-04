using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UnitHUD : MonoBehaviour
{
    private Unit cUnit;
    private RectTransform tfRect;
    [SerializeField]
    private RectTransform tfHpBar, tfMpBar;
    [SerializeField]
    private Image imgHpFill, imgMpFill, imgShieldFill, imgEveFill;
    [SerializeField]
    private Sprite[] imgStarSprites;
    [SerializeField]
    private Image imgStar;
    [SerializeField]
    private RectTransform tfTribeArea, tfBuffIconArea_P, tfBuffIconArea_N;
    [SerializeField]
    private Image[] imgITemIcons;
    [SerializeField, ReadOnlyInspector]
    private Image[] imgTribeEmblem, imgBuffIcon_P, imgBuffIcon_N;
    [SerializeField]
    private EquipmentSlotHUD cEquipSlot;
    [SerializeField]
    private UnitDamageTextArea cDamgeArea;


    public delegate void UpdateHUDFunc(bool _bNoneTarget);
    public UpdateHUDFunc updateHUD;

    private float fTargetHP;

    void Awake()
    {
        tfRect = GetComponent<RectTransform>();
        updateHUD += UpdateStar;
        updateHUD += UpdateHP;
        updateHUD += UpdateMP;
    }

    void FixedUpdate()
    {
        if (gameObject.activeSelf)
            imgHpFill.fillAmount = Mathf.Lerp(imgHpFill.fillAmount, fTargetHP, 0.1f);
    }

    [ContextMenu("Init")]
    private void Init()
    {
        imgBuffIcon_P = new Image[(int)BuffType.Count];
        imgBuffIcon_N = new Image[(int)BuffType.Count];

        for (int i = 0; i < tfBuffIconArea_P.childCount; i++)
        {
            imgBuffIcon_P[i] = tfBuffIconArea_P.GetChild(i).GetComponent<Image>();
            imgBuffIcon_N[i] = tfBuffIconArea_N.GetChild(i).GetComponent<Image>();
        }

        cDamgeArea = GetComponentInChildren<UnitDamageTextArea>();
    }

    public Unit GetHero()
    {
        return cUnit;
    }

    public void SetUnit(Unit _cUnit)
    {
        if (!_cUnit.battleUnit)
            return;

        cUnit = _cUnit;
        imgHpFill.fillAmount = 1f;
        imgMpFill.fillAmount = 0;
        imgEveFill.fillAmount = 1f;

        if (_cUnit is Hero)
            SetTribeEmblems(_cUnit as Hero);
        else
            tfTribeArea.gameObject.SetActive(false);
    }

    private void SetTribeEmblems(Hero _cHero)
    {
        if (tfTribeArea == null)
            return;

        imgTribeEmblem = new Image[3];

        for (int i = 0; i < imgTribeEmblem.Length; i++)
            imgTribeEmblem[i] = tfTribeArea.GetChild(i).GetChild(0).GetComponent<Image>();

        imgTribeEmblem[2].transform.parent.gameObject.SetActive(_cHero.heroStat.eClass_2 != 0);

        imgTribeEmblem[0].sprite = InGameManager.instance.GetEmblemSprite(_cHero.heroStat.eTribe, null);
        imgTribeEmblem[1].sprite = InGameManager.instance.GetEmblemSprite(null, _cHero.heroStat.eClass_1);
        imgTribeEmblem[2].sprite = InGameManager.instance.GetEmblemSprite(null, _cHero.heroStat.eClass_2);
    }

    public void UpdateHP(bool _bNoneTarget = false)
    {
        imgEveFill.fillAmount = (float)cUnit.eveHp / cUnit.unitStat.iMaxHp;
        Invoke(nameof(DisableEve), 0.2f);

        if (cUnit.unitStat.iShield <= 0)
        {
            
            fTargetHP = (float)cUnit.unitStat.iCrtHp / cUnit.unitStat.iMaxHp;
            imgShieldFill.fillAmount = 0;
        }
        else
        {
            if (cUnit.unitStat.iCrtHp + cUnit.unitStat.iShield <= cUnit.unitStat.iMaxHp)
            {
                fTargetHP = (float)cUnit.unitStat.iCrtHp / cUnit.unitStat.iMaxHp;
                imgHpFill.fillAmount = fTargetHP;
                imgShieldFill.fillAmount = ((float)cUnit.unitStat.iCrtHp + cUnit.unitStat.iShield) / cUnit.unitStat.iMaxHp;
            }
            else
            {
                fTargetHP = (float)cUnit.unitStat.iCrtHp / (cUnit.unitStat.iMaxHp + cUnit.unitStat.iShield);
                imgHpFill.fillAmount = fTargetHP;
                imgShieldFill.fillAmount = 1f;
            }
        }

        if (!_bNoneTarget)
            tfRect.SetSiblingIndex(transform.parent.childCount - 1);
    }

    private void DisableEve()
    {
        imgEveFill.fillAmount = 0;
    }

    public void AbleDamageText(int _iDamge, DamageType _eDmgTpye, bool _bIsCritical, bool _bIsEvation, int _iCode = 0)
    {
        cDamgeArea.OpenDamage(_iDamge, _eDmgTpye, _bIsCritical, _bIsEvation, _iCode);
    }

    public void UpdateMP(bool _bNoneTarget = false)
    {
        imgMpFill.fillAmount = (float)cUnit.unitStat.iCrtMP / cUnit.unitStat.iMaxMp;
    }

    private void UpdateStar(bool _bNoneTarget = false)
    {
        imgStar.sprite = imgStarSprites[cUnit.unitLevel - 1];
    }

    public void FollowUnit()
    {
        Vector3 _v3Pos = Camera.main.WorldToScreenPoint(cUnit.transform.position);  
        transform.position = new Vector3(_v3Pos.x, _v3Pos.y, 0);
    }

    public void SwitchHud(bool _bIsOn)
    {
        gameObject.SetActive(_bIsOn);
    }

    public void OnOffUpperHUD(bool _bIsOn)
    {
        imgStar.gameObject.SetActive(_bIsOn);
        tfHpBar.gameObject.SetActive(_bIsOn);
        tfMpBar.gameObject.SetActive(_bIsOn);
    }

    public void OnOffEmblem(bool _bIsOn)
    {
        if (tfTribeArea != null)
            tfTribeArea.gameObject.SetActive(_bIsOn && cUnit is Hero);
    }

    public void ResetBuffIcons()
    {
        foreach (var item in imgBuffIcon_P)
            item.gameObject.SetActive(false);
        foreach (var item in imgBuffIcon_N)
            item.gameObject.SetActive(false);
    }

    public void OnOffBuffIcon(BuffType _cType, bool _bPositive, bool _bOn)
    {
        if (_bPositive)
            imgBuffIcon_P[(int)_cType].gameObject.SetActive(_bOn);
        else
            imgBuffIcon_N[(int)_cType].gameObject.SetActive(_bOn);
    }

    public void AllOffCCIcon()
    {
        foreach (var item in imgBuffIcon_P)
            item.gameObject.SetActive(false);
    }

    public void OnOffEquipSlot(bool _bOn)
    {
        cEquipSlot.gameObject.SetActive(_bOn);
    }

    public void ResetHP()
    {
        imgHpFill.fillAmount = 1f;
    }

    public void ResetDmgText()
    {
        cDamgeArea.ResetTextList();
    }

    public float hpValue { get => imgHpFill.fillAmount; }
    public float mPValue { get => imgMpFill.fillAmount; }
    public EquipmentSlotHUD equpSlot { get => cEquipSlot; }
}
