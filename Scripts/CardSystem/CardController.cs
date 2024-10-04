using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public enum SkillScopeType { None, Circle}

public class CardController : MonoBehaviour, IPointerDownHandler
{
    [SerializeField, ReadOnlyInspector]
    private CardDisplay cCrtCard;
    [SerializeField, ReadOnlyInspector]
    private List<RectTransform> listCardDP;
    [SerializeField, ReadOnlyInspector]
    private int iHandCount;
    private RectTransform tfRect;
    private Vector2 v2OrgBundlePos, v2AbleBundlePos;
    private float fCardMoveTime, fCardHeight, fCardUpHehght;
    private bool bAbleBundle, bEditing;
    private Image imgPanel;
    [SerializeField]
    private Image imgAnyPanel;
    [SerializeField]
    private Transform tfSkillScopesParent;
    [SerializeField, ReadOnlyInspector]
    private List<SpellScope> listSpellScope;

    [SerializeField, ReadOnlyInspector]
    private bool bAny, bUseHero;
    [SerializeField, ReadOnlyInspector]
    private bool bCasting;
    [SerializeField, ReadOnlyInspector]
    private CardInfoPanel cCardInfoPanel;
    
    void Awake()
    {
        imgPanel = GetComponent<Image>();
        imgPanel.raycastTarget = false;
        tfRect = transform.GetChild(0).GetComponent<RectTransform>();

        v2OrgBundlePos = tfRect.position;
        v2AbleBundlePos = ((float)Screen.width / Screen.height < 1.5f) ? v2OrgBundlePos + Vector2.up * Screen.height * 0.025f : v2OrgBundlePos + Vector2.up * Screen.height * 0.2f;

        fCardMoveTime = 0.3f;
        fCardHeight = -590f;
        fCardUpHehght = fCardHeight + 23f;

        imgAnyPanel.gameObject.SetActive(false);
        InOutAny(false);

        foreach (var item in listSpellScope)
            item.gameObject.SetActive(false);

        cCardInfoPanel = FindObjectOfType<CardInfoPanel>(true);
    }

    void Start()
    {
        foreach (var item in listCardDP)
            item.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            AddCard();
        if (Input.GetKey(KeyCode.W))
            ThrowCard(0);
        if (Input.GetKey(KeyCode.E))
            ThrowCard(1);
        if (Input.GetKey(KeyCode.R))
            ThrowCard(2);
    }

    [ContextMenu("Init")]
    private void Init()
    {
        listCardDP = new List<RectTransform>();

        for (int i = 0; i < transform.GetChild(0).childCount; i++)
            listCardDP.Add(transform.GetChild(0).GetChild(i).GetComponent<RectTransform>());

        listSpellScope = new List<SpellScope>();

        for (int i = 0; i < tfSkillScopesParent.childCount; i++)
            listSpellScope.Add(tfSkillScopesParent.GetChild(i).GetComponent<SpellScope>());
    }

    public void SetCurrentCard(CardDisplay _cCard)
    {
        cCrtCard = _cCard;
    }

    public void AbleCardBundle()
    {
        if (bAbleBundle || bCasting)
            return;

        bAbleBundle = true;
        transform.GetChild(0).position = v2OrgBundlePos;
        transform.GetChild(0).DOMove(v2AbleBundlePos, 0.5f);
        imgPanel.raycastTarget = true;
    }

    public void HideCardBundle()
    {
        if (!bAbleBundle)
            return;

        bAbleBundle = false;
        transform.GetChild(0).position = v2AbleBundlePos;
        transform.GetChild(0).DOMove(v2OrgBundlePos, 0.5f);
        AllOffGlow();
    }

    public void AddCard()
    {
        if (bEditing || iHandCount > 2)
            return;

        bEditing = true;

        StartCoroutine(AddCardCoroutine(0));
    }

    public void AddCard(int _cardId)
    {
        if (bEditing || iHandCount > 2)
            return;

        bEditing = true;

        StartCoroutine(AddCardCoroutine(_cardId));
    }

    IEnumerator AddCardCoroutine(int _cardId)
    {
        AbleCardBundle();

        listCardDP[iHandCount].GetComponent<CardDisplay>().cv.alpha = 0;

        if (_cardId == 0)
            listCardDP[iHandCount].GetComponent<CardDisplay>().UpdateCardInfo(InGameManager.instance.cCardSystem.listDeck[Random.Range(0, 7)]);
        else
            listCardDP[iHandCount].GetComponent<CardDisplay>().UpdateCardInfo(CardManager.instance.GetCardWithID(_cardId));

        yield return new WaitForSeconds(0.5f);

        listCardDP[iHandCount].GetComponent<CardDisplay>().cv.alpha = 1f;

        switch (iHandCount)
        {
            case 0:
                listCardDP[iHandCount].anchoredPosition = new Vector2(800f, fCardUpHehght);
                listCardDP[iHandCount].gameObject.SetActive(true);
                listCardDP[iHandCount].DOAnchorPos(new Vector2(0, fCardUpHehght), fCardMoveTime);
                listCardDP[iHandCount].DORotate(Vector3.zero, fCardUpHehght);
                break;
            case 1:
                listCardDP[0].DOAnchorPos(new Vector2(-120f, fCardUpHehght), fCardMoveTime);
                listCardDP[0].DORotate(Vector3.forward * 5f, fCardMoveTime);

                listCardDP[iHandCount].anchoredPosition = new Vector2(800f, fCardUpHehght);
                listCardDP[iHandCount].gameObject.SetActive(true);
                listCardDP[iHandCount].DOAnchorPos(new Vector2(120f, fCardUpHehght), fCardMoveTime);
                listCardDP[iHandCount].DORotate(Vector3.forward * -5f, fCardUpHehght);
                break;
            case 2:
                listCardDP[0].DORotate(Vector3.forward * 10f, fCardMoveTime);
                listCardDP[0].DOAnchorPos(new Vector2(-220f, fCardHeight), fCardMoveTime);

                listCardDP[1].DORotate(Vector3.zero, fCardMoveTime);
                listCardDP[1].DOAnchorPos(new Vector2(0, fCardUpHehght), fCardMoveTime);

                listCardDP[iHandCount].anchoredPosition = new Vector2(800f, fCardUpHehght);
                listCardDP[iHandCount].gameObject.SetActive(true);
                listCardDP[iHandCount].DOAnchorPos(new Vector2(220f, fCardHeight), fCardMoveTime);
                listCardDP[iHandCount].DORotate(Vector3.forward * -10f, fCardMoveTime);
                break;
        }

        yield return new WaitForSeconds(0.5f);

        HideCardBundle();

        iHandCount++;

        bEditing = false;
        imgPanel.raycastTarget = false;
    }

    public void ThrowCard(int _iIndex)
    {
        if (bEditing || iHandCount <= 0 || !listCardDP[_iIndex].gameObject.activeSelf)
            return;

        bEditing = true;

        StartCoroutine(ThrowCardCoroutine(_iIndex));
    }

    IEnumerator ThrowCardCoroutine(int _iIndex)
    {
        yield return null;

        listCardDP[_iIndex].DOAnchorPos(new Vector2(-1200f, fCardHeight), fCardMoveTime);
        listCardDP[_iIndex].DORotate(Vector3.zero, fCardMoveTime).OnComplete(() => listCardDP[_iIndex].gameObject.SetActive(false));

        switch (_iIndex)
        {
            case 0:
                if (iHandCount == 3)
                {
                    listCardDP[1].DOAnchorPos(new Vector2(-120f, fCardUpHehght), fCardMoveTime);
                    listCardDP[1].DORotate(Vector3.forward * 5f, fCardUpHehght);

                    listCardDP[2].DOAnchorPos(new Vector2(120f, fCardUpHehght), fCardUpHehght);
                    listCardDP[2].DORotate(Vector3.forward * -5f, fCardUpHehght);
                }
                else if (iHandCount == 2)
                {
                    listCardDP[1].DOAnchorPos(new Vector2(0, fCardUpHehght), fCardUpHehght);
                    listCardDP[1].DORotate(Vector3.zero, fCardMoveTime);
                }
                break;
            case 1:
                if (iHandCount == 3)
                {
                    listCardDP[0].DOAnchorPos(new Vector2(-120f, fCardUpHehght), fCardMoveTime);
                    listCardDP[0].DORotate(Vector3.forward * 5f, fCardMoveTime);

                    listCardDP[2].DOAnchorPos(new Vector2(120f, fCardUpHehght), fCardMoveTime);
                    listCardDP[2].DORotate(Vector3.forward * -5f, fCardMoveTime);
                }
                else if (iHandCount == 2)
                {
                    listCardDP[0].DOAnchorPos(new Vector2(0, fCardUpHehght), fCardMoveTime);
                    listCardDP[0].DORotate(Vector3.zero, fCardMoveTime);
                }
                break;
            case 2:
                if (iHandCount == 3)
                {
                    listCardDP[0].DOAnchorPos(new Vector2(-120f, fCardUpHehght), fCardMoveTime);
                    listCardDP[0].DORotate(Vector3.forward * 5f, fCardMoveTime);

                    listCardDP[1].DOAnchorPos(new Vector2(120f, fCardUpHehght), fCardMoveTime);
                    listCardDP[1].DORotate(Vector3.forward * -5f, fCardMoveTime);
                }
                break;
        }

        yield return new WaitForSeconds(fCardMoveTime + 0.5f);

        listCardDP[_iIndex].SetSiblingIndex(2);

        listCardDP = new List<RectTransform>();

        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < transform.GetChild(0).childCount; i++)
            listCardDP.Add(transform.GetChild(0).GetChild(i).GetComponent<RectTransform>());

        iHandCount--;

        bEditing = false;
    }

    public void UseCard(int _iIndex)
    {
        if (bEditing || iHandCount <= 0 || !listCardDP[_iIndex].gameObject.activeSelf)
            return;

        bEditing = true;

        StartCoroutine(UseCardCoroutine(_iIndex));
    }

    IEnumerator UseCardCoroutine(int _iIndex)
    {
        listCardDP[_iIndex].gameObject.SetActive(false);
        listCardDP[_iIndex].rotation = Quaternion.identity;

        switch (_iIndex)
        {
            case 0:
                if (iHandCount == 3)
                {
                    listCardDP[1].DOAnchorPos(new Vector2(-120f, fCardUpHehght), fCardMoveTime);
                    listCardDP[1].DORotate(Vector3.forward * 5f, fCardUpHehght);

                    listCardDP[2].DOAnchorPos(new Vector2(120f, fCardUpHehght), fCardUpHehght);
                    listCardDP[2].DORotate(Vector3.forward * -5f, fCardUpHehght);
                }
                else if (iHandCount == 2)
                {
                    listCardDP[1].DOAnchorPos(new Vector2(0, fCardUpHehght), fCardUpHehght);
                    listCardDP[1].DORotate(Vector3.zero, fCardMoveTime);
                }
                break;
            case 1:
                if (iHandCount == 3)
                {
                    listCardDP[0].DOAnchorPos(new Vector2(-120f, fCardUpHehght), fCardMoveTime);
                    listCardDP[0].DORotate(Vector3.forward * 5f, fCardMoveTime);

                    listCardDP[2].DOAnchorPos(new Vector2(120f, fCardUpHehght), fCardMoveTime);
                    listCardDP[2].DORotate(Vector3.forward * -5f, fCardMoveTime);
                }
                else if (iHandCount == 2)
                {
                    listCardDP[0].DOAnchorPos(new Vector2(0, fCardUpHehght), fCardMoveTime);
                    listCardDP[0].DORotate(Vector3.zero, fCardMoveTime);
                }
                break;
            case 2:
                if (iHandCount == 3)
                {
                    listCardDP[0].DOAnchorPos(new Vector2(-120f, fCardUpHehght), fCardMoveTime);
                    listCardDP[0].DORotate(Vector3.forward * 5f, fCardMoveTime);

                    listCardDP[1].DOAnchorPos(new Vector2(120f, fCardUpHehght), fCardMoveTime);
                    listCardDP[1].DORotate(Vector3.forward * -5f, fCardMoveTime);
                }
                break;
        }

        iHandCount--;

        yield return new WaitForSeconds(fCardMoveTime + 0.5f);

        listCardDP[_iIndex].SetSiblingIndex(2);

        listCardDP = new List<RectTransform>();

        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < transform.GetChild(0).childCount; i++)
            listCardDP.Add(transform.GetChild(0).GetChild(i).GetComponent<RectTransform>());

        bEditing = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        imgPanel.raycastTarget = false;
        HideCardBundle();
        cCrtCard = null;
    }

    public void InOutAny(bool _bIn)
    {
        bAny = _bIn;

        imgAnyPanel.color = _bIn ? Color.white : new Color(1f, 1f, 1f, 0.7f);
    }

    public void OnOffAnyPanel(bool _bOn)
    {
        if (!_bOn)
            bAny = false;

        imgAnyPanel.gameObject.SetActive(_bOn);
    }

    public void SetEquip(bool _bOn)
    {
        bUseHero = _bOn;
    }

    public void AllOffGlow()
    {
        foreach (var item in listCardDP)
            item.GetComponent<CardDisplay>().OnOffGlow(false);
    }

    public void OnOffScope(int _iIndex, bool _bOn)
    {
        listSpellScope[_iIndex].gameObject.SetActive(_bOn);
    }

    public void AllOffScope()
    {
        foreach (var item in listSpellScope)
            item.gameObject.SetActive(false);
    }

    public void MoveScope(int _iIndex, Vector3 _v3Pos)
    {
        listSpellScope[_iIndex].transform.position = _v3Pos;
    }

    public void AllOff()
    {
        AllOffGlow();
        AllOffScope();
    }

    public void SetScopeColor(int _iIndex, Color _cColor)
    {
        listSpellScope[_iIndex].SetColor(_cColor);
    }

    public void OpenCardInfo(Card _cCard, Vector2 _v2Pos, Vector2 _v2Pivot)
    {
        GameManager.instance.OpenCardInfo(_cCard, _v2Pos, _v2Pivot);
    }

    public CardDisplay crtCard { get => cCrtCard; }
    public bool any { get => bAny; }
    public bool useHero { get => bUseHero; }
    public bool casting { get => bCasting; set => bCasting = value; }
}
