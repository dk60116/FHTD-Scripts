using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EnemyCampInfoPanel : MonoBehaviour
{
    [SerializeField, ReadOnlyInspector]
    private Image imgCampProfile;
    [SerializeField, ReadOnlyInspector]
    private ScrollRect uiSynergyRect;
    [SerializeField, ReadOnlyInspector]
    private List<Image> listSynergyIcon;
    [SerializeField, ReadOnlyInspector]
    private List<UnitIconFrame> listUnitIcon;
    [SerializeField, ReadOnlyInspector]
    private RectTransform tfSynergyInfoPanel;
    [SerializeField, ReadOnlyInspector]
    private TextMeshProUGUI txtStageCount;

    [ContextMenu("Init")]
    private void Init()
    {
        imgCampProfile = transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<Image>();

        listSynergyIcon = new List<Image>();
        listUnitIcon = new List<UnitIconFrame>();

        uiSynergyRect = transform.GetChild(2).GetChild(1).GetComponent<ScrollRect>();

        for (int i = 0; i < uiSynergyRect.content.childCount; i++)
            listSynergyIcon.Add(uiSynergyRect.content.GetChild(i).GetComponent<Image>());

        for (int i = 0; i < transform.GetChild(3).GetChild(0).childCount; i++)
            listUnitIcon.Add(transform.GetChild(3).GetChild(0).GetChild(i).GetComponent<UnitIconFrame>());

        txtStageCount = transform.GetChild(2).GetChild(0).GetChild(1).GetComponentInChildren<TextMeshProUGUI>();
    }

    public void OnOffPanel(bool _bOn)
    {
        gameObject.SetActive(_bOn);
    }

    public void UpdateEnemyCampInfo(int _iCrtChapter, int _iCrtStage)
    {
        txtStageCount.text = _iCrtStage.ToString();

        for (int i = 0; i < listUnitIcon.Count; i++)
            listUnitIcon[i].gameObject.SetActive(false);

        for (int i = 0; i < GameManager.instance.chapterList[_iCrtChapter].listStageUnits[_iCrtStage - 1].listId.Count; i++)
        {
            listUnitIcon[i].gameObject.SetActive(true);
            listUnitIcon[i].UpdateUnit(GameManager.instance.chapterList[_iCrtChapter].listStageUnits[_iCrtStage - 1].listId[i], GameManager.instance.chapterList[_iCrtChapter].listStageUnits[_iCrtStage - 1].listLevel[i]);
        }
    }

    public List<UnitIconFrame> unitIconList { get => listUnitIcon; }
}
