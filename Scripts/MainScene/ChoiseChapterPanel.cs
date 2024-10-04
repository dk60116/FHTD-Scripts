using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public enum ChapterDifficulty { Easy, Normal, Hard, Hell}

public class ChoiseChapterPanel : MonoBehaviour
{
    [SerializeField, ReadOnlyInspector]
    private ScrollRect uiChapterScroll;
    [SerializeField, ReadOnlyInspector]
    private List<Toggle> listChpterToggle;
    [SerializeField, ReadOnlyInspector]
    private List<TextMeshProUGUI> listChapterText;
    [SerializeField, ReadOnlyInspector]
    private Button uiGameButton;
    [SerializeField, ReadOnlyInspector]
    private RectTransform tfSelectChapterBox;
    [SerializeField, ReadOnlyInspector]
    private List<Toggle> listDifficultyToggle;

    void Awake()
    {
        for (int i = 0; i < listChpterToggle.Count; i++)
        {
            int _iIndex = i;
            listChpterToggle[i].onValueChanged.AddListener((bool _bValue) => SelectChaptor(_bValue, _iIndex));
        }

        for (int i = 0; i < listDifficultyToggle.Count; i++)
        {
            int _iIndex = i;
            listDifficultyToggle[i].onValueChanged.AddListener((bool _bValue) => ChangeDifficulty(_bValue, (ChapterDifficulty)_iIndex));
        }

        uiGameButton.onClick.AddListener(() => GameManager.instance.LoadScene("InGame"));

        UploadChapter();
    }

    void Start()
    {
        SelectChaptor(true, 0);
    }

    [ContextMenu("Init")]
    private void Init()
    {
        uiChapterScroll = GetComponentInChildren<ScrollRect>();
        listChpterToggle = new List<Toggle>();
        listChapterText = new List<TextMeshProUGUI>();

        for (int i = 0; i < uiChapterScroll.content.childCount; i++)
        {
            listChpterToggle.Add(uiChapterScroll.content.GetChild(i).GetComponent<Toggle>());
            listChapterText.Add(uiChapterScroll.content.GetChild(i).GetComponentInChildren<TextMeshProUGUI>());
        }

        for (int i = 0; i < transform.GetChild(1).GetChild(0).childCount; i++)
            listDifficultyToggle.Add(transform.GetChild(1).GetChild(0).GetChild(i).GetComponent<Toggle>());

        tfSelectChapterBox = uiChapterScroll.transform.GetChild(0).GetChild(1).GetComponent<RectTransform>();
        uiGameButton = uiGameButton = transform.GetChild(1).GetComponentInChildren<Button>();
    }

    private void UploadChapter()
    {
        for (int i = 0; i < listChpterToggle.Count; i++)
        {
            listChpterToggle[i].image.sprite = GameManager.instance.chapterList[i].imgStageEmblem;
            listChapterText[i].text = GameManager.instance.chapterList[i].strChapterName;
        }
    }

    private void SelectChaptor(bool _bValue, int _iIndex)
    {
        if (_bValue)
        {
            tfSelectChapterBox.SetParent(listChpterToggle[_iIndex].transform);
            tfSelectChapterBox.transform.DOLocalMove(Vector2.zero, 0.1f).OnComplete(()=> listChpterToggle[_iIndex].image.rectTransform.sizeDelta = Vector2.one * 180f);
            GameManager.instance.selectedChapter = _iIndex;
        }
        else
        {
            tfSelectChapterBox.transform.DOLocalMove(Vector2.zero, 0.1f).OnComplete(() => listChpterToggle[_iIndex].image.rectTransform.sizeDelta = Vector2.one * 130f);
        }
    }

    private void ChangeDifficulty(bool _bValue, ChapterDifficulty _eDifficulty)
    {
        if (_bValue)
            GameManager.instance.selectedDifficulty = _eDifficulty;
    }
}
