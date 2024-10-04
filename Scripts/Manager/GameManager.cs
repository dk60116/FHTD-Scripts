using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Events;

[Serializable]
public struct MarketPercentageArray
{
    [ReadOnlyInspector]
    public List<float> list;
}

public class GameManager : SingletonMono<GameManager>
{
    [SerializeField, ReadOnlyInspector]
    private List<Hero> listHeroData;
    [SerializeField]
    private MarketPercentageArray[] fHeroShopPercentageArray;
    [SerializeField]
    private Texture2D imgCursor;
    [SerializeField]
    private Text txtLogArea;
    [SerializeField, ReadOnlyInspector]
    private bool bIsGPGS;
    [SerializeField, ReadOnlyInspector]
    private ConfirmPanel cConfrimPanel;
    [SerializeField, ReadOnlyInspector]
    private SimpleNoticePanel cNoticePanel;
    [SerializeField]
    private GameObject goLoadingPanel, goMiniLoadingPanel;
    [SerializeField, ReadOnlyInspector]
    private RectTransform tfLoadingIcon;
    [SerializeField, ReadOnlyInspector]
    private CardInfoPanel cCardInfoPanel;
    [SerializeField, ReadOnlyInspector]
    public SynergySystem cSynergySystem;

    [SerializeField, ReadOnlyInspector]
    private List<Sprite> listAvatarSprite;

    [SerializeField]
    private List<ChapterStatus> listChapterStat;

    [SerializeField, ReadOnlyInspector]
    private int iSelectedChapter;
    [SerializeField, ReadOnlyInspector]
    private ChapterDifficulty iSelectedDifficulty;

    protected override void Awake()
    {
        base.Awake();
        Application.targetFrameRate = 250;
        cConfrimPanel.gameObject.SetActive(false);
        goLoadingPanel.SetActive(false);
        CloseCardInfo();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
            Time.timeScale = 0;
        if (Input.GetKeyDown(KeyCode.Alpha1))
            Time.timeScale = 1f;
        if (Input.GetKeyDown(KeyCode.Alpha2))
            Time.timeScale = 1.5f;
        if (Input.GetKeyDown(KeyCode.Alpha3))
            Time.timeScale = 2f;
        if (Input.GetKeyDown(KeyCode.Alpha4))
            Time.timeScale = 4f;
        if (Input.GetKeyDown(KeyCode.Alpha9))
            Time.timeScale = 0.5f;
        if (Input.GetKeyDown(KeyCode.Alpha8))
            Time.timeScale = 0.1f;
    }


    [ContextMenu("Init")]
    private void Init()
    {
        cConfrimPanel = GetComponentInChildren<ConfirmPanel>(true);
        cNoticePanel = GetComponentInChildren<SimpleNoticePanel>(true);
        tfLoadingIcon = goLoadingPanel.transform.GetChild(0).GetComponent<RectTransform>();
        cCardInfoPanel = GetComponentInChildren<CardInfoPanel>(true);
        cSynergySystem = GetComponent<SynergySystem>();

        listAvatarSprite.Clear();

        for (int i = 0; i < 100; i++)
        {
            int _iIndex = i;
            listAvatarSprite.Add(Resources.Load<Sprite>("ProfileIcons/" + _iIndex));
        }
    }

    public Hero GetHeroWithId(int _iId)
    {
        for (int i = 0; i < listHeroData.Count; i++)
        {
            if (listHeroData[i].unitID == _iId)
                return listHeroData[i];
        }

        return null;
    }

    public void BuildDebugLog(object _obj)
    {
        if (_obj == null)
            txtLogArea.text = "null";
        else if (_obj is string)
            txtLogArea.text = _obj as string;
        else txtLogArea.text = _obj.ToString();
    }

    public void SetAuth(bool _bOn)
    {
        bIsGPGS = _bOn;
    }

    public void OpenConfirm(string _strText, UnityAction _cEvent, bool _bCancelButton = true)
    {
        cConfrimPanel.gameObject.SetActive(true);
        cConfrimPanel.OpenPaenl(_strText, _cEvent, _bCancelButton);
    }

    public void OpenNoticePanel(string _strText)
    {
        cNoticePanel.gameObject.SetActive(true);
        cNoticePanel.SetNotice(_strText);
    }

    public void OnOffLoadingPanel(bool _bOn)
    {
        tfLoadingIcon.rotation = Quaternion.identity;
        goLoadingPanel.gameObject.SetActive(_bOn);
    }

    public void OnOffMiniLoadingPanel(bool _bOn)
    {
        goMiniLoadingPanel.gameObject.SetActive(_bOn);
    }

    public void LoadScene(string _strPath)
    {
        SceneManager.LoadScene(_strPath);
    }

    public void OpenCardInfo(Card _cCard, Vector2 _v2Pos, Vector2 _v2Pivot, bool _bPivot = true)
    {
        if (_bPivot)
            cCardInfoPanel.SetPivot(_v2Pivot);
        cCardInfoPanel.transform.position = _v2Pos;
        cCardInfoPanel.gameObject.SetActive(true);
        cCardInfoPanel.UpdateCard(_cCard);
    }

    public void CloseCardInfo()
    {
        cCardInfoPanel.gameObject.SetActive(false);
    }

    public List<Hero> heroData { get => listHeroData; set => listHeroData = value; }
    public MarketPercentageArray[] marketPercentage { get => fHeroShopPercentageArray; set => fHeroShopPercentageArray = value; }
    public bool isAuth { get => bIsGPGS; }
    public bool isOpenInfoPanel { get => cCardInfoPanel.gameObject.activeSelf; }
    public Text logArea { get => txtLogArea; }
    public List<Sprite> avatarList { get => listAvatarSprite; }
    public List<ChapterStatus> chapterList { get => listChapterStat; set => listChapterStat = value; } 
    public int selectedChapter { get => iSelectedChapter; set => iSelectedChapter = value; }
    public ChapterDifficulty selectedDifficulty { get => iSelectedDifficulty; set => iSelectedDifficulty = value; }
}
