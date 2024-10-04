using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum MainShopCategories { Recommend, CardPack, HeroToken, Special }

public class MainShopPanel : MonoBehaviour
{
    public List<MainShopComponents> shopComponentList;
    [SerializeField]
    private List<Toggle> upperToggleList;
    [SerializeField]
    private List<MainShopScroll> scrollList;
    [SerializeField, ReadOnlyInspector]
    private string categorie, sideTag;
    [ReadOnlyInspector]
    public int categorieIndex, sideTagIndex;
    [SerializeField]
    private TextMeshProUGUI categorie_tag_Text;
    [SerializeField]
    private ShopLine singleLinePrefab, dualLinePrefab;
    private List<ShopLine> spawnedLines;
    [SerializeField]
    private MainShopConfirm confirmPanel;

    private void Awake()
    {
        for (int i = 0; i < upperToggleList.Count; i++)
        {
            int _i = i;
            upperToggleList[_i].onValueChanged.AddListener((bool _on) => OnScrollHandler(_on, _i));
        }
    }

    void OnEnable()
    {
        confirmPanel.gameObject.SetActive(false);
        categorieIndex = 0;
        sideTagIndex = 0;

        Invoke(nameof(EnableHandler), Time.deltaTime);
    }

    private void EnableHandler()
    {
        upperToggleList[categorieIndex].isOn = true;
        scrollList[categorieIndex].scroll.verticalNormalizedPosition = 0;
        scrollList[categorieIndex].TriggerTagToggle(sideTagIndex);
    }

    private void SideInitHandler()
    {
        scrollList[categorieIndex].scroll.verticalNormalizedPosition = 0;
        scrollList[categorieIndex].TriggerTagToggle(0);
    }

    private void OnScrollHandler(bool _on, int _index)
    {
        scrollList[_index].gameObject.SetActive(_on);
        categorie = ((MainShopCategories)_index).ToString();
        sideTag = scrollList[_index].GetSideToggleText(sideTagIndex);
        categorieIndex = _index;
        sideTagIndex = 0;
        categorie_tag_Text.text = $"{categorie} - {sideTag}";
        SideInitHandler();
    }

    public void SetTag(int _tag)
    {
        sideTagIndex = _tag;
        sideTag = scrollList[categorieIndex].GetSideToggleText(sideTagIndex);
        categorie_tag_Text.text = $"{categorie} - {sideTag}";

        foreach (var item in spawnedLines)
            item.gameObject.SetActive(item.component.tag.Contains(sideTagIndex));
    }

    public void SpawnLine()
    {
        if (spawnedLines == null)
            spawnedLines = new List<ShopLine>();

        foreach (var item in spawnedLines)
            Destroy(item.gameObject);

        spawnedLines = new List<ShopLine>();

        foreach (var item in shopComponentList)
        {
            ShopLine _newLine;

            if (!item.isDual)
                _newLine = Instantiate(singleLinePrefab);
            else
                _newLine = Instantiate(dualLinePrefab);

            _newLine.transform.SetParent(scrollList[item.category].scroll.content);
            _newLine.transform.localScale = Vector3.one;
            _newLine.SetInfo(item);
            spawnedLines.Add(_newLine);
        }
    }

    public void ResetLine(int _num, string _shopId, bool _soldOut)
    {
        foreach (var item in spawnedLines)
        {
            try
            {
                if (item.component.contentId[_num] == _shopId)
                    item.SetSoldOut(_num, _soldOut);
            }
            catch { }
        }
    }

    public void OpenConfirm(MainShopComponents _component, RawImage _image, int _num)
    {
        confirmPanel.UpdateConfirm(_component, _image, _num);

        confirmPanel.gameObject.SetActive(true);
    }
}
