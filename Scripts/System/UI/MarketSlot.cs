using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class MarketSlot : MonoBehaviour
{
    [SerializeField, ReadOnlyInspector]
    private HeroMarketSystem cMarket;
    private bool bSoldOut;
    [SerializeField]
    private Image imgBG, imgHeroProfile, imgTearImage, imgNameImage, imgSoldOut;
    [SerializeField]
    private RectTransform tfHeroRankGemSlot;
    [SerializeField]
    private TextMeshProUGUI txtHeroName, txtHeroCost;
    private TextMeshProUGUI txtTribe, txtClass_1, txtClass_2;
    [SerializeField]
    private Image[] imgTribeEmblem;
    [SerializeField]
    private CanvasGroup cTribeCanvasGroup;

    private Hero cHero;

    void Awake()
    {
        txtTribe = imgTribeEmblem[0].transform.GetComponentInChildren<TextMeshProUGUI>();
        txtClass_1 = imgTribeEmblem[1].transform.GetComponentInChildren<TextMeshProUGUI>();
        txtClass_2 = imgTribeEmblem[2].transform.GetComponentInChildren<TextMeshProUGUI>();
    }

    [ContextMenu("Init")]
    private void Init()
    {
        cMarket = GetComponentInParent<HeroMarketSystem>();
    }

    public void SetHero(Hero _cHero)
    {
        bSoldOut = false;
        imgTearImage.gameObject.gameObject.SetActive(true);

        cHero = _cHero;

        txtHeroName.text = _cHero.unitName;
        txtHeroCost.text = ((int)_cHero.unitStat.eRank).ToString();
        imgBG.sprite = cMarket.heroBGImages[(int)_cHero.unitStat.eRank - 1];
        imgHeroProfile.sprite = _cHero.unitIllust;
        cTribeCanvasGroup.alpha = (_cHero.heroStat.eClass_2 == HeroClass.None) ? 0 : 1f;
        imgTribeEmblem[0].sprite = InGameManager.instance.GetEmblemSprite(_cHero.heroStat.eTribe, null);
        imgTribeEmblem[1].sprite = InGameManager.instance.GetEmblemSprite(null, _cHero.heroStat.eClass_1);
        imgTribeEmblem[2].sprite = InGameManager.instance.GetEmblemSprite(null, _cHero.heroStat.eClass_2);
        txtTribe.text = _cHero.heroStat.eTribe.ToString();
        txtClass_1.text = _cHero.heroStat.eClass_1.ToString();
        txtClass_2.text = _cHero.heroStat.eClass_2.ToString();

        for (int i = 0; i < tfHeroRankGemSlot.childCount; i++)
        {
            tfHeroRankGemSlot.GetChild(i).gameObject.SetActive(false);
            tfHeroRankGemSlot.GetChild((int)_cHero.unitStat.eRank - 1).gameObject.SetActive(true);
        }

        switch (_cHero.unitStat.eRank)
        {
            case UnitRank.None:
                break;
            case UnitRank.Common:
                imgTearImage.color = new Color(0.7f, 0.8f, 1f);
                imgNameImage.color = new Color(0.7f, 0.8f, 1f);
                break;
            case UnitRank.UnCommon:
                imgTearImage.color = new Color(0.7f, 1f, 0.7f);
                imgNameImage.color = new Color(0.7f, 1f, 0.7f);
                break;
            case UnitRank.Rare:
                imgTearImage.color = new Color(0.3f, 0.5f, 1f);
                imgNameImage.color = new Color(0.3f, 0.5f, 1f);
                break;
            case UnitRank.Epic:
                imgTearImage.color = new Color(0.7f, 0.4f, 0.7f);
                imgNameImage.color = new Color(0.7f, 0.4f, 0.7f);
                break;
            case UnitRank.Legendary:
                imgTearImage.color = new Color(1f, 0.7f, 0.3f);
                imgNameImage.color = new Color(1f, 0.7f, 0.3f);
                break;
            default:
                break;
        }

        imgSoldOut.gameObject.SetActive(false);
    }

    public void BuyUnit()
    {
        if (bSoldOut || cHero == null)
            return;

        if (!InGameManager.instance.CheckNotSlotFull())
            return;

        if (!InGameManager.instance.cPlayerController.TryUseGold((int)cHero.unitStat.eRank))
            return;

        Tile _cEmptyTile = InGameManager.instance.GetEmptySlotTile();

        if (_cEmptyTile != null)
            _cEmptyTile.SetNewUnit(cMarket.objPool.GetObj(cHero.heroNumber).GetComponent<Unit>());

        bSoldOut = true;

        imgTearImage.gameObject.gameObject.SetActive(false);

        imgSoldOut.gameObject.SetActive(true);
    }
}
