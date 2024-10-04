using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SynergyCountSlot : MonoBehaviour, IPointerDownHandler, IPointerExitHandler
{
    [SerializeField]
    private bool bIsEnemy;

    private int iSynergyNum;
    private CanvasGroup cGroup;
    private Image imgBackground;
    [SerializeField]
    private Image imgEmblem;
    [SerializeField]
    private TextMeshProUGUI txtTribe, txtCount, txtSynergy;
    private int iCount;
    private bool bIsOn;
    [SerializeField]
    private bool[] bIsSynergyOn, bTearCount;
    [SerializeField]
    private int iSynergyMax, iTearMax;

    void Awake()
    {
        cGroup = GetComponent<CanvasGroup>();
        imgBackground = GetComponent<Image>();
    }

    public void SetInfo(HeroTribe _eTribe, List<int> _iCount_2, int _iNumber)
    {
        imgEmblem.sprite = InGameManager.instance.GetEmblemSprite(_eTribe, null);

        iSynergyNum = _iNumber;
        iCount = GameManager.instance.cSynergySystem.GetSynergyCount(_eTribe, bIsEnemy);
        bIsOn = iCount > 0;

        name = $"SynergyCountSlot({_eTribe})";

        txtTribe.text = _eTribe.ToString();
        txtCount.text = iCount.ToString();
        txtSynergy.text = string.Empty;

        bIsSynergyOn = new bool[_iCount_2.Count];
        bTearCount = new bool[4];
        iSynergyMax = 0;
        iTearMax = 0;

        for (int i = 0; i < _iCount_2.Count; i++)
        {
            if (i < bIsSynergyOn.Length - 1)
                bIsSynergyOn[i] = iCount >= _iCount_2[i] && iCount < _iCount_2[i + 1];
            else
                bIsSynergyOn[i] = iCount >= _iCount_2[i];

            if (bIsSynergyOn.Length > 0)
            {
                if (bIsSynergyOn[0])
                {
                    bTearCount[0] = true;
                    txtSynergy.text += "<color=white>";
                    if (bIsSynergyOn.Length == 1)
                        bTearCount[2] = true;
                }
            }
            if (bIsSynergyOn.Length > 1)
            {
                if (bIsSynergyOn[1])
                {
                    bTearCount[1] = true;
                    txtSynergy.text += "<color=white>";

                    if (bIsSynergyOn.Length == 2)
                        bTearCount[2] = true;
                }
            }
            if (bIsSynergyOn.Length > 2)
            {
                if (bIsSynergyOn[2])
                {
                    bTearCount[2] = true;
                    txtSynergy.text += "<color=white>";
                }
            }
            if (bIsSynergyOn.Length > 3)
            {
                if (bIsSynergyOn[3])
                {
                    bTearCount[3] = true;
                    txtSynergy.text += "<color=white>";
                }
            }
            if (!bIsSynergyOn[i])
                txtSynergy.text += "<color=grey>";

            txtSynergy.text += _iCount_2[i];
            txtSynergy.text += "</color>";

            if (i == _iCount_2.Count - 1)
                break;

            txtSynergy.text += " ¡¤ ";
        }

        if (!bTearCount[0])
            imgEmblem.color = Color.grey;
        if (bTearCount[0])
            imgEmblem.color = Color.white;
        if (bTearCount[1])
            imgEmblem.color = Color.blue;
        if (bTearCount[2])
            imgEmblem.color = Color.yellow;
        if (bTearCount[3])
            imgEmblem.color = Color.red;

        bool _bIsSynergyOn = GameManager.instance.cSynergySystem.GetSynergyCount(_eTribe, bIsEnemy) >= _iCount_2[0];

        for (int i = 0; i < bIsSynergyOn.Length; i++)
        {
            int _iCount = i;

            if (bIsSynergyOn[i])
                iSynergyMax = _iCount + 1;
        }

        for (int i = 0; i < bTearCount.Length; i++)
        {
            int _iCount = i;

            if (bTearCount[i])
                iTearMax = _iCount + 1;
        }

        if (imgBackground != null)
            imgBackground.color = new Color(imgBackground.color.r, imgBackground.color.g, imgBackground.color.b, _bIsSynergyOn ? 1f : 0.3f);
        
        gameObject.SetActive(isOn);

        txtTribe.rectTransform.anchoredPosition = new Vector2(90f, txtSynergy.rectTransform.anchoredPosition.y);
    }

    public void SetInfo(HeroClass _eClass, List<int> _iCount_2, int _iNumber)
    {
        imgEmblem.sprite = InGameManager.instance.GetEmblemSprite(null, _eClass);

        iSynergyNum = _iNumber;
        iCount = GameManager.instance.cSynergySystem.GetSynergyCount(_eClass, bIsEnemy);
        bIsOn = iCount > 0;

        name = $"SynergyCountSlot({_eClass})";

        txtTribe.text = _eClass.ToString();
        txtCount.text = iCount.ToString();
        txtSynergy.text = string.Empty;

        bIsSynergyOn = new bool[_iCount_2.Count];
        bTearCount = new bool[4];
        iSynergyMax = 0;
        iTearMax = 0;

        for (int i = 0; i < _iCount_2.Count; i++)
        {
            if (i < bIsSynergyOn.Length - 1)
                bIsSynergyOn[i] = iCount >= _iCount_2[i] && iCount < _iCount_2[i + 1];
            else
                bIsSynergyOn[i] = iCount >= _iCount_2[i];

            if (bIsSynergyOn.Length > 0)
            {
                if (bIsSynergyOn[0])
                {
                    bTearCount[0] = true;
                    txtSynergy.text += "<color=white>";
                    if (bIsSynergyOn.Length == 1)
                        bTearCount[2] = true;
                }
            }
            if (bIsSynergyOn.Length > 1)
            {
                if (bIsSynergyOn[1])
                {
                    bTearCount[1] = true;
                    txtSynergy.text += "<color=white>";

                    if (bIsSynergyOn.Length == 2)
                        bTearCount[2] = true;
                }
            }
            if (bIsSynergyOn.Length > 2)
            {
                if (bIsSynergyOn[2])
                {
                    bTearCount[2] = true;
                    txtSynergy.text += "<color=white>";
                }
            }
            if (bIsSynergyOn.Length > 3)
            {
                if (bIsSynergyOn[3])
                {
                    bTearCount[3] = true;
                    txtSynergy.text += "<color=white>";
                }
            }
            if (!bIsSynergyOn[i])
                txtSynergy.text += "<color=grey>";

            txtSynergy.text += _iCount_2[i];
            txtSynergy.text += "</color>";

            if (i == _iCount_2.Count - 1)
                break;
            
            txtSynergy.text += " ¡¤ ";
        }

        if (!bTearCount[0])
            imgEmblem.color = Color.grey;
        if (bTearCount[0])
            imgEmblem.color = Color.white;
        if (bTearCount[1])
            imgEmblem.color = Color.blue;
        if (bTearCount[2])
            imgEmblem.color = Color.yellow;
        if (bTearCount[3])
            imgEmblem.color = Color.red;

        bool _bIsSynergyOn = GameManager.instance.cSynergySystem.GetSynergyCount(_eClass, bIsEnemy) >= _iCount_2[0];

        for (int i = 0; i < bIsSynergyOn.Length; i++)
        {
            int _iCount = i;

            if (bIsSynergyOn[i])
                iSynergyMax = _iCount + 1;
        }

        for (int i = 0; i < bTearCount.Length; i++)
        {
            int _iCount = i;

            if (bTearCount[i])
                iTearMax = _iCount + 1;
        }

        if (imgBackground != null)
            imgBackground.color = new Color(imgBackground.color.r, imgBackground.color.g, imgBackground.color.b, _bIsSynergyOn ? 1f : 0.3f);
        gameObject.SetActive(isOn);

        txtTribe.rectTransform.anchoredPosition = new Vector2(90f, txtSynergy.rectTransform.anchoredPosition.y);
    }

    public void OpenSynergyPanel(Vector2 _v2Pos)
    {
        Vector2 _v2Pivot = Vector2.zero;
        Vector2 _v2resultPos = Vector2.zero;

        if (_v2Pos.x < Screen.width * 0.5f)
        {
            _v2Pivot.x = 0;
            _v2resultPos.x = 60f;
        }
        else
        {
            _v2Pivot.x = 1f;
            _v2resultPos.x = -60f;
        }

        if (_v2Pos.y < Screen.height * 0.5f)
        {
            _v2Pivot.y = 0;
            _v2resultPos.y = 15f;
        }
        else
        {
            _v2Pivot.y = 1;
            _v2resultPos.y = -15f;
        }

        GameManager.instance.cSynergySystem.synergyDescriptionPanel.rect.pivot = _v2Pivot;
        GameManager.instance.cSynergySystem.synergyDescriptionPanel.transform.position = _v2Pos + _v2resultPos;

        GameManager.instance.cSynergySystem.synergyDescriptionPanel.gameObject.SetActive(true);
        GameManager.instance.cSynergySystem.synergyDescriptionPanel.UpdateSynergy(21 - iSynergyNum);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OpenSynergyPanel(transform.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GameManager.instance.cSynergySystem.synergyDescriptionPanel.gameObject.SetActive(false);
    }

    public int number { get => iSynergyNum; }
    public int count { get => iCount; }
    public bool isOn { get => bIsOn; }
    public bool[] isSynergyOn { get => bIsSynergyOn; }
    public bool[] tearCount { get => bTearCount; }
    public int synergyMax { get => iSynergyMax; }
    public int tearMax { get => iTearMax; }
}
