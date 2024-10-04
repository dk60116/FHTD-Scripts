using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UnitIconFrame : MonoBehaviour, IPointerClickHandler
{
    [SerializeField, ReadOnlyInspector]
    private Unit cUnit;
    [SerializeField, ReadOnlyInspector]
    private Image imgProfile;
    [SerializeField, ReadOnlyInspector]
    private Image[] imgStars;

    [ContextMenu("Init")]
    private void Init()
    {
        imgProfile = transform.GetChild(0).GetComponent<Image>();

        imgStars = new Image[3];

        for (int i = 0; i < imgStars.Length; i++)
            imgStars[i] = transform.GetChild(1).GetChild(i).GetComponent<Image>();
    }

    public void UpdateUnit(int _iUnitId, int _iLevel)
    {
       Unit _cUnit = GameManager.instance.GetHeroWithId(_iUnitId);
       imgProfile.sprite = _cUnit.faceIllust;

        foreach (var item in imgStars)
            item.gameObject.SetActive(false);

        for (int i = 0; i < _iLevel; i++)
            imgStars[i].gameObject.SetActive(true);
    }

    public void SetUnit(Unit _cUnit)
    {
        cUnit = _cUnit;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        InGameManager.instance.cHeroInfoPanel.UpdateInfo(cUnit as Hero);
        InGameManager.instance.cHeroInfoPanel.OnOffPanel(true);
    }
}
