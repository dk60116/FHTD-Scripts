using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UIHealthAlchemy;

public class PlayerInfoUI : MonoBehaviour
{
    [SerializeField, ReadOnlyInspector]
    private CampHealthBar cCampHealthBowl;
    [SerializeField, ReadOnlyInspector]
    private TextMeshProUGUI txtGold, txtLevel, txtExp, txtExpClick;
    [SerializeField, ReadOnlyInspector]
    private Image imgExpFill;
    [SerializeField, ReadOnlyInspector]
    private Button uiIncreaseExpButton;
    [SerializeField, ReadOnlyInspector]
    private TextMeshProUGUI txtExpCost;
    [SerializeField, ReadOnlyInspector]
    private List<TextMeshProUGUI> listMarketPercentageText;
    [SerializeField, ReadOnlyInspector]
    private TextMeshProUGUI txtWinningStreak;

    void Awake()
    {
        uiIncreaseExpButton.onClick.AddListener(() => InGameManager.instance.cPlayerController.IncreaseBasicExp());
    }

    [ContextMenu("Init")]
    private void Init()
    {
        txtGold = transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
        txtLevel = transform.GetChild(1).GetChild(0).GetComponentInChildren<TextMeshProUGUI>();
        txtExp = transform.GetChild(3).GetChild(0).GetChild(1).GetComponentInChildren<TextMeshProUGUI>();
        txtExpClick = transform.GetChild(3).GetChild(0).GetChild(0).GetComponentInChildren<TextMeshProUGUI>();
        imgExpFill = transform.GetChild(3).GetChild(0).GetChild(0).GetComponent<Image>();
        uiIncreaseExpButton = transform.GetChild(3).GetChild(1).GetComponent <Button>();
        txtExpCost = transform.GetChild(3).GetChild(1).GetChild(0).GetComponentInChildren<TextMeshProUGUI>();
        listMarketPercentageText = new List<TextMeshProUGUI>();
        cCampHealthBowl = FindObjectOfType<CampHealthBar>(true);
        txtWinningStreak = GameObject.Find("Winning Streak Text").GetComponent<TextMeshProUGUI>();

        for (int i = 0; i < GameObject.Find("Reroll Percentage Info").transform.GetChild(1).transform.childCount; i++)
            listMarketPercentageText.Add(GameObject.Find("Reroll Percentage Info").transform.GetChild(1).GetChild(i).GetComponentInChildren<TextMeshProUGUI>());
    }

    public void UpdatePlayerInfo(CampStatus _sCamp)
    {
        txtGold.text = _sCamp.iGold.ToString();
        uiIncreaseExpButton.interactable = _sCamp.iExpIncreasePoint <= _sCamp.iGold;
        txtLevel.text = "LV " + _sCamp.iCampLevel.ToString();
        txtExp.text = _sCamp.iCampCrtExp.ToString() + " / " + _sCamp.iCampNeedExp.ToString();
        imgExpFill.fillAmount = (float)_sCamp.iCampCrtExp / _sCamp.iCampNeedExp;
        txtExpClick.text = (((_sCamp.iCampNeedExp / _sCamp.iExpIncreasePoint)) - _sCamp.iExpClickCount).ToString();
        txtExpCost.text = _sCamp.iExpCost.ToString();
    }

    public void UpdatePercentage(int _iIndex, float _fPercent)
    {
        listMarketPercentageText[_iIndex].text = _fPercent.ToString() + " %";
    }

    public void UpdateHPBowl(CampStatus _cCamp)
    {
        cCampHealthBowl.UpdateBowl(_cCamp.iCampMaxHp, _cCamp.iCampCrtHp);
    }

    public void UpdateWinningStreakText(bool _isWinning, int _iCount)
    {
        string _strResult = string.Empty;

        _strResult += _isWinning ? "Winning\nstreak\n" : "Losing \nstreak\n";
        _strResult += "<size=20>" + _iCount.ToString() + "</size>";

        txtWinningStreak.text = _strResult;
    }
}
