using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DealParserPanel : MonoBehaviour
{
    [SerializeField]
    private GameObject mainPanel;

    [SerializeField, ReadOnlyInspector]
    private ScrollRect uiScroll;

    [SerializeField, ReadOnlyInspector]
    private List<DealParserElemnt> listElements;

    [SerializeField, ReadOnlyInspector]
    private int iTotalDamage;
    [SerializeField]
    private TextMeshProUGUI txtTotalDmg;
    [SerializeField, ReadOnlyInspector]
    private int iFirstDmg;
    [SerializeField]
    private List<Sprite> listHeroFrame;

    [SerializeField, ReadOnlyInspector]
    private List<RectTransform> listOrgYList;

    [ContextMenu("Init")]
    private void Init()
    {
        uiScroll = GetComponentInChildren<ScrollRect>();

        listElements = new List<DealParserElemnt>();

        listOrgYList = new List<RectTransform>();

        for (int i = 0; i < uiScroll.content.childCount; i++)
        {
            var elementTransform = uiScroll.content.GetChild(i);
            listElements.Add(elementTransform.GetComponent<DealParserElemnt>());
            listOrgYList.Add(elementTransform.GetComponent<RectTransform>());
        }
    }

    void Start()
    {
        UpdateTotalDamage();

        foreach (var item in listElements)
            item.gameObject.SetActive(false);

        if (InGameManager.instance.status == StageStatus.Running)
            UpdateHeroList();
    }

    private void OnEnable()
    {
        StartCoroutine(SmoothUIUpdateCoroutine());
    }

    public void OnOffPanel(bool _bOn)
    {
        mainPanel.gameObject.SetActive(_bOn);
    }

    IEnumerator SmoothUIUpdateCoroutine()
    {
        while (true)
        {
            foreach (var item in listElements)
            {
                if (!item.gameObject.activeSelf)
                    continue;

                item.display.position = Vector3.Lerp(item.display.position, listElements[item.rank].transform.position, 0.25f);
            }
            yield return null;
        }
    }

    public void UpdateHeroList()
    {
        iTotalDamage = 0;
        UpdateTotalDamage();

        int _iActiveCount = 0;

        List<Hero> _listHero = InGameManager.instance.cPlayerController.placedHeros.OrderByDescending(x => x.heroCost).OrderByDescending(x => x.unitLevel).ToList();

        for (int i = 0; i < _listHero.Count; i++)
        {
            listElements[i].InitHero(_listHero[i]);
            listElements[i].gameObject.SetActive(true);
            listElements[i].SetRank(_iActiveCount);
            _iActiveCount++;
        }

        for (int i = _iActiveCount; i < listElements.Count; i++)
        {
            listElements[i].gameObject.SetActive(false);
        }
    }

    public void AddTotalDamage(int _iDamage)
    {
        iTotalDamage += _iDamage;
        UpdateTotalDamage();
    }

    private void UpdateTotalDamage()
    {
        txtTotalDmg.text = iTotalDamage.ToString();
    }

    public Sprite GetHeroFrameSprite(int _iLevel)
    {
        return listHeroFrame[_iLevel - 1];
    }

    public void UpdateAllUnit()
    {
        List<DealParserElemnt> _listTemp = new List<DealParserElemnt>();
        foreach (var item in listElements)
        {
            if (!item.gameObject.activeSelf) continue;
            _listTemp.Add(item);
        }

        _listTemp.Sort((x, y) => y.totalDamage.CompareTo(x.totalDamage));

        iFirstDmg = (int)_listTemp[0].totalDamage;

        for (int i = 0; i < _listTemp.Count; i++)
        {
            _listTemp[i].UpdateDamage();
            _listTemp[i].SetRank(i);
        }
    }

    public int totalDamage { get { return iTotalDamage; } }
    public int firstDamage { get { return iFirstDmg; } }
}

