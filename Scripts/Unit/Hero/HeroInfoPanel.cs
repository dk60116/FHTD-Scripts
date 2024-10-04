using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class HeroInfoPanel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField, ReadOnlyInspector]
    private RectTransform tfMainPanel;
    [SerializeField, ReadOnlyInspector]
    private bool bOnPointerMain;
    [SerializeField, ReadOnlyInspector]
    private Hero cHero;
    [SerializeField]
    private Image imgHeroFace;
    [SerializeField]
    private Transform tfStarArea, tfHeroTribeArea, tfHeroStatArea;
    [SerializeField]
    private Sprite[] imgStarSprites;
    [SerializeField, ReadOnlyInspector]
    private Image[] imgStar, imgTribeEmblems, imgStats;
    [SerializeField]
    private TextMeshProUGUI txtHeroName, txtHeroCost;
    [SerializeField]
    private TextMeshProUGUI txtHeroHPText, txtHeroMPText;
    [SerializeField]
    private Image imgHeroHPFill, imgHeroMPFill;
    [SerializeField]
    private TextMeshProUGUI[] txtHeroTribes, txtStatValues;
    [SerializeField, ReadOnlyInspector]
    private Image[] imgTribeTriggers;
    [SerializeField]
    private Image imgDefSkillIcon, imgAtkSkillIcon;
    [SerializeField]
    private TextMeshProUGUI txtDefSkillName, txtAtkSkillName;
    [SerializeField]
    private HeroSkillInfoPanel cSkillInfoPanel;
    [SerializeField]
    private Image imgSkill_DefPanel, imgSkill_AtkPanel;

    [Header("Developer Menu")]
    [SerializeField]
    private Button uiSkillPlayButton;
    [SerializeField]
    private Slider uiHeroRotateSlider;
    [SerializeField]
    private Toggle uiAtkUnitToggle;

    void Awake()
    {
        imgStar = new Image[3];
        for (int i = 0; i < tfStarArea.childCount; i++)
            imgStar[i] = tfStarArea.GetChild(i).GetComponent<Image>();
        imgTribeEmblems = new Image[3];
        txtHeroTribes = new TextMeshProUGUI[3];
        imgTribeTriggers = new Image[3];
        
        for (int i = 0; i < tfHeroTribeArea.childCount; i++)
        {
            imgTribeEmblems[i] = tfHeroTribeArea.GetChild(i).GetChild(0).GetComponent<Image>();
            txtHeroTribes[i] = tfHeroTribeArea.GetChild(i).GetChild(1).GetComponent<TextMeshProUGUI>();
            imgTribeTriggers[i] = tfHeroTribeArea.GetChild(i).GetComponent<Image>();
        }
        
        imgStats = new Image[tfHeroStatArea.childCount];     
        txtStatValues = new TextMeshProUGUI[tfHeroStatArea.childCount];

        for (int i = 0; i < tfHeroStatArea.childCount; i++)
        {
            imgStats[i] = tfHeroStatArea.GetChild(i).GetComponentInChildren<Image>();
            txtStatValues[i] = tfHeroStatArea.GetChild(i).GetComponentInChildren<TextMeshProUGUI>();
        }

        cSkillInfoPanel.gameObject.SetActive(false);
        tfMainPanel = transform.GetChild(0).GetComponent<RectTransform>();
        tfMainPanel.gameObject.SetActive(false);
    }

    void OnDisable()
    {
        Init();  
    }

    private void Init()
    {
        imgHeroFace.sprite = null;
        txtHeroName.text = "Hero name";
        txtHeroCost.text = "00";
        for (int i = 0; i < imgStar.Length; i++)
            imgStar[i].gameObject.SetActive(false);
        txtHeroHPText.text = "000 / 000";
        txtHeroMPText.text = "000 / 000";
    }

    public void OnOffPanel(bool _bValue)
    {
        cSkillInfoPanel.gameObject.SetActive(false);
        InGameManager.instance.cHeroInfoPanel.CloseSynergyExplationPanel();

        if (_bValue)
        {
            tfMainPanel.gameObject.SetActive(true);
            imgSkill_DefPanel.color = Color.white;
            imgSkill_AtkPanel.color = Color.white;
        }
        else
        {
            tfMainPanel.gameObject.SetActive(bOnPointerMain);

            if (!bOnPointerMain)
                cSkillInfoPanel.gameObject.SetActive(false);
        }
    }

    public void UpdateInfo(Hero _cHero)
    {
        Init();
        cHero = _cHero;
        imgHeroFace.sprite = _cHero.faceIllust;

        for (int i = 0; i < _cHero.unitLevel; i++)
        {
            imgStar[i].sprite = imgStarSprites[_cHero.unitLevel - 1];
            imgStar[i].gameObject.SetActive(true);
        }
        
        txtHeroName.text = _cHero.unitName;
        txtHeroCost.text = _cHero.heroCost.ToString();
        UpdateHPMP();
        imgTribeEmblems[0].sprite = InGameManager.instance. GetEmblemSprite(_cHero.heroStat.eTribe, null);
        imgTribeEmblems[1].sprite = InGameManager.instance.GetEmblemSprite(null, _cHero.heroStat.eClass_1);
        imgTribeEmblems[2].sprite = InGameManager.instance.GetEmblemSprite(null, _cHero.heroStat.eClass_2);
        txtHeroTribes[0].text = _cHero.heroStat.eTribe.ToString();
        txtHeroTribes[1].text = _cHero.heroStat.eClass_1.ToString();
        txtHeroTribes[2].text = _cHero.heroStat.eClass_2.ToString();
        
        if (_cHero.heroStat.eClass_2 == HeroClass.None)
        {
            imgTribeEmblems[2].color = new Color(0, 0, 0, 0);
            txtHeroTribes[2].color = new Color(0, 0, 0, 0);
        }
        else
        {
            imgTribeEmblems[2].color = Color.white;
            txtHeroTribes[2].color = Color.white;
        }
        
        imgDefSkillIcon.sprite = _cHero.skill.skillComponent_Def.imgSkillIcon;
        imgAtkSkillIcon.sprite = _cHero.skill.skillComponent_Atk.imgSkillIcon;
        txtDefSkillName.text = _cHero.skill.skillComponent_Def.strSkillName;
        txtAtkSkillName.text = _cHero.skill.skillComponent_Atk.strSkillName;
        UpdateStatus();

        uiSkillPlayButton.onClick.RemoveAllListeners();
        uiSkillPlayButton.onClick.AddListener(() => _cHero.CastDefenceSkill());
        uiHeroRotateSlider.onValueChanged.RemoveAllListeners(); 
        uiHeroRotateSlider.onValueChanged.AddListener((float _fValue) => RotateHero(_fValue));
        uiAtkUnitToggle.onValueChanged.RemoveAllListeners();
        uiAtkUnitToggle.isOn = cHero.attackUnit;
        uiAtkUnitToggle.onValueChanged.AddListener((bool _bValue) => cHero.SetAtkUnit(_bValue));
        uiAtkUnitToggle.onValueChanged.AddListener((bool _bValue) => cHero.footHold.gameObject.SetActive(!_bValue));
    }

    private void RotateHero(float _fValue)
    {
        cHero.body.transform.rotation = Quaternion.Euler(new Vector3(cHero.transform.rotation.eulerAngles.x, 360f * _fValue, cHero.transform.rotation.eulerAngles.z));
    }

    public void UpdateHPMP()
    {
        if (cHero == null)
            return;

        txtHeroHPText.text = $"{cHero.unitStat.iCrtHp} / {cHero.unitStat.iMaxHp}";
        imgHeroHPFill.fillAmount = (float)cHero.unitStat.iCrtHp / cHero.unitStat.iMaxHp;
        txtHeroMPText.text = $"{cHero.unitStat.iCrtMP} / {cHero.unitStat.iMaxMp}";
        if (cHero.unitStat.iMaxMp > 0)
            imgHeroMPFill.fillAmount = (float)cHero.unitStat.iCrtMP / cHero.unitStat.iMaxMp;
        else
        {
            txtHeroMPText.text = string.Empty;
            imgHeroMPFill.fillAmount = 0;
        }
    }

    public void UpdateStatus()
    {
        if (cHero == null)
            return;

        UpdateHPMP();

        txtStatValues[0].text = (cHero.unitStat.iCrtAttack + cHero.unitStat.iAddAttack).ToString();
        txtStatValues[1].text = (cHero.unitStat.iCrtAbilPower + cHero.unitStat.iAddPower).ToString();
        txtStatValues[2].text = (cHero.unitStat.iCrtADDef + cHero.unitStat.iAddADDef).ToString();
        txtStatValues[3].text = (cHero.unitStat.iCrtAPDef + cHero.unitStat.iAddAPDef).ToString();
        txtStatValues[4].text = decimal.Parse(cHero.unitStat.fCrtAtkSpeed.ToString("F2")).ToString("G29");
        txtStatValues[5].text = decimal.Parse(cHero.unitStat.fCrtAtkRange.ToString("F2")).ToString("G29");
        txtStatValues[6].text = decimal.Parse(cHero.unitStat.fCrtCriRate.ToString("F1")).ToString("G29") + "%";
        txtStatValues[7].text = cHero.unitStat.fCrtCriDamage.ToString("F1");
        txtStatValues[8].text = decimal.Parse(cHero.unitStat.fCrtEvasionRate.ToString("F1")).ToString("G29") + "%";
        txtStatValues[9].text = decimal.Parse(cHero.unitStat.fCrtMoveSpeed.ToString("F2")).ToString("G29");
    }

    public void OpenSynergyExplationPanel(int _iIndex)
    {
        if (_iIndex == 2 && cHero.heroStat.eClass_2 == HeroClass.None)
            return;

        foreach (var item in imgTribeTriggers)
            item.color = new Color(0, 0f, 0f, 0f);
        imgSkill_DefPanel.color = Color.white;
        imgSkill_AtkPanel.color = Color.white;
        imgTribeTriggers[_iIndex].color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        cSkillInfoPanel.gameObject.SetActive(false);
        GameManager.instance.cSynergySystem.synergyDescriptionPanel.rect.pivot = new Vector2(1f, 0.5f);
        GameManager.instance.cSynergySystem.synergyDescriptionPanel.gameObject.SetActive(true);
        GameManager.instance.cSynergySystem.synergyDescriptionPanel.rect.position = imgTribeEmblems[_iIndex].rectTransform.position + Vector3.left * 25f;
        if (_iIndex == 0)
            GameManager.instance.cSynergySystem.synergyDescriptionPanel.UpdateTribe(cHero.heroStat.eTribe);
        else if (_iIndex == 1)
            GameManager.instance.cSynergySystem.synergyDescriptionPanel.UpdateClass(cHero.heroStat.eClass_1);
        else if (_iIndex == 2)
            GameManager.instance.cSynergySystem.synergyDescriptionPanel.UpdateClass(cHero.heroStat.eClass_2);
    }

    public void CloseSynergyExplationPanel()
    {
        StartCoroutine(CloseSynergyExplationCoroutine());
    }

    IEnumerator CloseSynergyExplationCoroutine()
    {
        yield return new WaitForEndOfFrame();

        foreach (var item in imgTribeTriggers)
            item.color = new Color(0, 0f, 0f, 0f);
        GameManager.instance.cSynergySystem.synergyDescriptionPanel.gameObject.SetActive(false);
    }

    public void OpenSkillExplationPanel(bool _bValue)
    {
        CloseSynergyExplationPanel();
        GameManager.instance.cSynergySystem.synergyDescriptionPanel.gameObject.SetActive(false);
        StartCoroutine(OpenSkillExplationPanelCoroutine(_bValue));
    }

    IEnumerator OpenSkillExplationPanelCoroutine(bool _bValue)
    {
        yield return cWaitFrame;

        cSkillInfoPanel.gameObject.SetActive(true);
        cSkillInfoPanel.SetText(cHero, _bValue);

        imgSkill_DefPanel.color = _bValue ? Color.white : Color.blue;
        imgSkill_AtkPanel.color = _bValue ? Color.red : Color.white;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        PointerDown();
    }

    public void PointerDown()
    {
        bOnPointerMain = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        PointerUp();
    }

    public void PointerUp()
    {
        StartCoroutine(PointerUpCoroutine());
    }

    private WaitForEndOfFrame cWaitFrame = new WaitForEndOfFrame();

    IEnumerator PointerUpCoroutine()
    {
        yield return cWaitFrame;

        bOnPointerMain = false;
    }

    public Hero currentHero { get => cHero; }
}
