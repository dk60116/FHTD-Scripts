using PlayFab.ClientModels;
using PlayFab;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using Sprite = UnityEngine.Sprite;
using UnityEngine.EventSystems;
using Newtonsoft.Json;
using System;

public class CardDeckPanel : MonoBehaviour
{
    [SerializeField, ReadOnlyInspector]
    private List<CardDisplay> listCardDisplay;
    [SerializeField]
    private Animator panelAnmator;
    [SerializeField]
    private CardDisplay cCardPrefab;
    [SerializeField, ReadOnlyInspector]
    private ScrollRect uiCardScroll;
    [SerializeField, ReadOnlyInspector]
    private RectTransform tfCardContent;
    [SerializeField]
    private List<SelectedCard> listDeckPanel;
    [SerializeField]
    private List<SelectDeckListButton> listDeckPanelButtons;
    [SerializeField]
    private Toggle createToggle;
    [SerializeField]
    private RectTransform confirmPanel;
    [SerializeField]
    private Image confirmIllustImage;
    [SerializeField]
    private TextMeshProUGUI powderCountText, powderResultText, confirmText;
    [SerializeField]
    private Button confirmOkBtn;

    [SerializeField]
    private TMP_Dropdown cardTypeDropDown, cardTearDropDown, cardTearValueDropDown, cardLevelDropDown, cardLevelValueDropDown, cardCostDropDown, cardCostNumberDropDown, cardCostValueDropDown, cardClassDropDown;
    [SerializeField]
    private Image typeFilterIcon, tearFilterIcon, tearValueFilterIcon, levelFilterIcon, costFilterIcon, costValueFilterIcon, classFilterIcon;
    [SerializeField]
    private List<Sprite> valueSprites;

    [SerializeField]
    private TMP_InputField searchInput;

    [SerializeField]
    private CardDP_Moving movingCard;
    [SerializeField, ReadOnlyInspector]
    private bool isMovingCard;
    [SerializeField, ReadOnlyInspector]
    private RectTransform movingCardrt;

    [SerializeField]
    private GraphicRaycaster m_Raycaster;
    private PointerEventData m_PointerEventData;
    [SerializeField]
    private EventSystem m_EventSystem;

    [SerializeField]
    private RectTransform candidateListPanel;
    [SerializeField]
    private TextMeshProUGUI deckListTitleText;
    [SerializeField]
    private TMP_InputField deckNameInput;
    private List<List<int>> candidateCardList;
    [ReadOnlyInspector]
    public List<string> deckNameList;

    [SerializeField]
    private RectTransform countPanel;
    private int confirmCount;
    [SerializeField]
    private TextMeshProUGUI confirmCountText;
    [SerializeField]
    private Button minBtn, downBtn, upBtn, maxBtn;

    void Awake()
    {
        createToggle.onValueChanged.AddListener((bool _on) => OnOffCreateMode(_on));
        cardTypeDropDown.onValueChanged.AddListener((int _value) => TypeFilterHandler(_value));
        cardTearDropDown.onValueChanged.AddListener((int _value) => TearFilterHandler(_value));
        cardTearValueDropDown.onValueChanged.AddListener((int _value) => TearValueFilterHandler(_value));
        cardCostDropDown.onValueChanged.AddListener((int _value) => CostFilterHandler(_value));
        cardLevelValueDropDown.onValueChanged.AddListener((int _value) => LevelValueFilterHandler(_value));
        cardCostNumberDropDown.onValueChanged.AddListener((int _value) => UpdateCardDisplay());
        cardCostValueDropDown.onValueChanged.AddListener((int _value) => CostValueFilterHandler(_value));
        cardClassDropDown.onValueChanged.AddListener((int _value) => ClassFilterHandler(_value));
        searchInput.onValueChanged.AddListener((string _text) => SearchText(_text));
        candidateCardList = new List<List<int>>();

        for (int i = 0; i < 5; i++)
        {
            deckNameList.Add(string.Empty);
            candidateCardList.Add(new List<int>());
        }
    }

    void Start()
    {
        InstantCardDisplay();
    }

    void OnEnable()
    {
        panelAnmator.enabled = true;
        Invoke(nameof(DisableAnimator), 1f);
        searchInput.gameObject.SetActive(false);
        OnOffCreateToggle(false);
        GetUserCardInventory();
        movingCard.gameObject.SetActive(false);
        SocialManager.instance.GetPlayerInfo(SocialManager.instance.playerInfo.masterID, SocialManager.instance.playerInfo.nickName);
        candidateListPanel.gameObject.SetActive(false);
    }

    void Update()
    {
        movingCardrt.position = Input.mousePosition;

        if (isMovingCard)
        {
            CheckUIUnderMouse();

            if (Input.GetMouseButtonUp(0))
            {
                isMovingCard = false;
                SetMovingCard(false, null);
            }
        }
    }

    [ContextMenu("Init")]
    private void Init()
    {
        panelAnmator = transform.GetChild(0).GetComponent<Animator>();
        cCardPrefab = Resources.Load<CardDisplay>("GUI/Card/CardDP_Main");
        uiCardScroll = GameObject.Find("Card Scroll View").GetComponent<ScrollRect>();
        tfCardContent = uiCardScroll.content;
        movingCardrt = movingCard.GetComponent<RectTransform>();
    }

    private void DisableAnimator()
    {
        panelAnmator.enabled = false;
    }

    private void InstantCardDisplay()
    {
        for (int i = 0; i < CardManager.instance.cardList.Count; i++)
        {
            CardDisplay _cCardDP = Instantiate(cCardPrefab, tfCardContent);
            listCardDisplay.Add(_cCardDP);
        }
    }

    public void UpdateCardDisplay()
    {
        IEnumerable<Card> _cardList = CardManager.instance.cardList;

        if (cardCostValueDropDown.value == 0)
            _cardList = _cardList.OrderBy(x => x.stat.iCost);
        else
            _cardList = _cardList.OrderByDescending(x => x.stat.iCost);

        if (cardLevelValueDropDown.value != 0)
            _cardList = _cardList.OrderByDescending(x => x.stat.iLevel);
        else
            _cardList = _cardList.OrderBy(x => x.stat.iLevel);

        if (cardTearValueDropDown.value == 0)
            _cardList = _cardList.OrderBy(x => x.stat.eCardTear);
        else
            _cardList = _cardList.OrderByDescending(x => x.stat.eCardTear);

        if ((CardType)cardTypeDropDown.value != CardType.All)
            _cardList = _cardList.OrderByDescending(x => x.stat.eCardType == (CardType)cardTypeDropDown.value);

        if ((CardTear)cardTearDropDown.value != CardTear.None)
            _cardList = _cardList.OrderByDescending(x => x.stat.eCardTear == (CardTear)cardTearDropDown.value);

        if ((CardCostType)cardCostDropDown.value != CardCostType.All)
            _cardList = _cardList.OrderByDescending(x => x.stat.eCostType == (CardCostType)cardCostDropDown.value);

        if (cardCostNumberDropDown.value > 0)
            _cardList = _cardList.OrderByDescending(x => x.stat.iCost == cardCostNumberDropDown.value - 1);

        if (cardClassDropDown.value != 0)
            _cardList = _cardList.OrderByDescending(x => x.stat.eCardClass == (CardClass)cardClassDropDown.value);

        List<Card> _resultList = _cardList.ToList();

        if (listCardDisplay.Count > 0)
        {
            for (int i = 0; i < _resultList.Count; i++)
            {
                listCardDisplay[i].name = _resultList[i].stat.strCardName;
                listCardDisplay[i].UpdateCardInfo(_resultList[i]);
            }

            foreach (var item in listDeckPanel)
                item.ResetCard();
        }
    }

    public void SetDeckCard(int _iIndex, int _iCardId)
    {
        listDeckPanel[_iIndex].SetCard(CardManager.instance.GetCardWithID(_iCardId));
    }

    public void GetUserCardInventory()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), OnInventoryReceived, OnInventoryReciveFailed);
        UpdateCardDisplay();
    }

    void OnInventoryReceived(GetUserInventoryResult result)
    {
        StringBuilder logBuilder = new StringBuilder();

        foreach (var item in CardManager.instance.cardList)
            item.UnOwnCard();

        foreach (var item in result.Inventory)
        {
            if (item.ItemClass == ItemType.Card.ToString())
                CardManager.instance.GetCardWithID(int.Parse(item.ItemId)).OwnCard(item.ItemInstanceId, (int)item.RemainingUses);

            if (logBuilder.Length > 0)
                logBuilder.Append(", ");

            logBuilder.AppendFormat("Name: {0} Calss: {1}", item.DisplayName, item.ItemClass);
        }

        UpdateCardDisplay();

        Debug.Log("OwnCardList: " + logBuilder);
    }

    void OnInventoryReciveFailed(PlayFabError error)
    {
        Debug.LogError("PlayFab Error: " + error.ErrorMessage);
    }

    private void OnOffCreateMode(bool _value)
    {
        foreach (var item in listCardDisplay)
        {
            item.UpdateCardInfo(item.card);
        }
    }

    public void OnOffCreateToggle(bool _on)
    {
        createToggle.isOn = _on;
    }

    public void OpenCardConfirm(bool _create, Card _card)
    {
        GameManager.instance.OnOffMiniLoadingPanel(true);

        confirmIllustImage.sprite = _card.stat.imgCardIcon;
        confirmOkBtn.onClick.RemoveAllListeners();

        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest
        {
            FunctionName = "OpenCardConfirmPanel",
            FunctionParameter = new
            {
                itemId = _card.stat.iCardID.ToString(),
                isCreate = _create
            },
            GeneratePlayStreamEvent = true
        };

        PlayFabClientAPI.ExecuteCloudScript(request, OnOpenConfirmSuccess, OnOpenConfirmFailed);
    }

    private void OnOpenConfirmSuccess(ExecuteCloudScriptResult result)
    {
        Debug.LogError(result.FunctionResult);

        Card _card = CardManager.instance.GetCardWithID(int.Parse(result.FunctionResult.ToString().Split(',')[0]));
        confirmCount = 1;
        int _cardCount = int.Parse(result.FunctionResult.ToString().Split(',')[1]);
        confirmCountText.text = $"{confirmCount} / {_cardCount}";
        int _cardPowder = int.Parse(result.FunctionResult.ToString().Split(',')[2]);
        powderCountText.text = _cardPowder.ToString();
        bool _isCreate = bool.Parse(result.FunctionResult.ToString().Split(',')[3]);
        int _userPowder = int.Parse(result.FunctionResult.ToString().Split(',')[4]);

        if (_isCreate)
        {
            if (_cardCount >= 2)
                confirmText.text = "There will be more cards than the deck can hold. Do you want to <color=red>create</color> this card?";
            else
                confirmText.text = "Are you sure you want to <color=red>create</color> this card?";
            confirmOkBtn.onClick.AddListener(() => CreateCard(_card, _cardPowder));
            powderResultText.text = $"{_userPowder}  >>  <color=red>{_userPowder - _cardPowder}</color>";
        }
        else
        {
            if (CardManager.instance.deckCard.Contains(_card) && _cardCount <= 2)
                confirmText.text = "The current deck contains this card. Do you really want to <color=red>destroy</color> this card?";
            else
                confirmText.text = "Are you sure you want to <color=red>destroy</color> this card?";
            confirmOkBtn.onClick.AddListener(() => SellCard(_card.stat.iCardID.ToString(), confirmCount));
            powderResultText.text = $"{_userPowder}  >>  <color=#2868FF>{_userPowder + _cardPowder}</color>";
        }

        countPanel.gameObject.SetActive(!_isCreate && _cardCount > 2);

        downBtn.interactable = false;
        upBtn.interactable = true;

        minBtn.onClick.RemoveAllListeners();
        downBtn.onClick.RemoveAllListeners();
        upBtn.onClick.RemoveAllListeners();
        maxBtn.onClick.RemoveAllListeners();

        minBtn.onClick.AddListener(() => MinBtnHandler(_cardCount,_cardPowder, _userPowder));
        downBtn.onClick.AddListener(() =>  DownBtnHandler(_cardCount, _cardPowder, _userPowder));
        upBtn.onClick.AddListener(() => UpBtnHandler(_cardCount, _cardPowder, _userPowder));
        maxBtn.onClick.AddListener(() => MaxBtnHandler(_cardCount, _cardPowder, _userPowder));

        GameManager.instance.OnOffMiniLoadingPanel(false);
        confirmPanel.gameObject.SetActive(true);
    }

    private void MinBtnHandler(int _cardCount, int _cardPowder, int _playerPowder)
    {
        confirmCount = 1;
        confirmCountText.text = $"{confirmCount} / {_cardCount}";
        
        downBtn.interactable = false;
        upBtn.interactable = true;

        powderResultText.text = $"{_playerPowder}  >>  <color=#2868FF>{_playerPowder + _cardPowder * confirmCount}</color>";
    }

    private void DownBtnHandler(int _cardCount, int _cardPowder, int _playerPowder)
    {
        confirmCount--;
        confirmCountText.text = $"{confirmCount} / {_cardCount}";

        if (confirmCount <= 1)
            downBtn.interactable = false;

        upBtn.interactable = true;

        powderResultText.text = $"{_playerPowder}  >>  <color=#2868FF>{_playerPowder + _cardPowder * confirmCount}</color>";
    }

    private void UpBtnHandler(int _cardCount, int _cardPowder, int _playerPowder)
    {
        confirmCount++;
        confirmCountText.text = $"{confirmCount} / {_cardCount}";

        if (confirmCount >= _cardCount)
            upBtn.interactable = false;

        downBtn.interactable = true;

        powderResultText.text = $"{_playerPowder}  >>  <color=#2868FF>{_playerPowder + _cardPowder * confirmCount}</color>";
    }

    private void MaxBtnHandler(int _cardCount, int _cardPowder, int _playerPowder)
    {
        confirmCount = _cardCount - 2;
        confirmCountText.text = $"{confirmCount} / {_cardCount}";

        if (confirmCount > 1)
            downBtn.interactable = true;
        upBtn.interactable = true;

        powderResultText.text = $"{_playerPowder}  >>  <color=#2868FF>{_playerPowder + _cardPowder * confirmCount}</color>";
    }

    private void OnOpenConfirmFailed(PlayFabError error)
    {
        Debug.LogError($"PlayFab Error: {error.ErrorMessage}");
    }

    private void CreateCard(Card _card, int _price)
    {
        SocialManager.instance.PurchaseItem(ItemType.Card, _card.stat.iCardID.ToString(), CurrencyType.PD, _price);
        confirmPanel.gameObject.SetActive(false);
    }

    public void SellCard(string _itemId, int _quantityToSell)
    {
        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest
        {
            FunctionName = "SellCard",
            FunctionParameter = new
            {
                itemId = _itemId,
                quantity = _quantityToSell
            }
        };

        PlayFabClientAPI.ExecuteCloudScript(request, OnSellItemSuccess, OnSellItemError);
    }

    private void OnSellItemSuccess(ExecuteCloudScriptResult result)
    {
        Debug.LogError(result.FunctionResult);
        Debug.Log("Item sold successfully! " + result.FunctionResult.ToString());

        GetUserCardInventory();
        SocialManager.instance.UpdateCurrencyCount();
        
        confirmPanel.gameObject.SetActive(false);
    }

    private void OnSellItemError(PlayFabError error)
    {
        Debug.LogError("Error selling item: " + error.GenerateErrorReport());
    }

    private void TypeFilterHandler(int _value)
    {
        typeFilterIcon.sprite = CardManager.instance.cardTypeIcons[_value];

        UpdateCardDisplay();
    }

    private void TearFilterHandler(int _value)
    {
        tearFilterIcon.sprite = CardManager.instance.cardGems[_value];

        UpdateCardDisplay();
    }
    private void TearValueFilterHandler(int _value)
    {
        tearValueFilterIcon.sprite = valueSprites[_value];

        UpdateCardDisplay();
    }

    private void LevelValueFilterHandler(int _value)
    {
        levelFilterIcon.sprite = valueSprites[_value];

        UpdateCardDisplay();
    }

    private void CostFilterHandler(int _value)
    {
        costFilterIcon.sprite = CardManager.instance.costSpriteList[_value];

        UpdateCardDisplay();
    }

    private void CostValueFilterHandler(int _value)
    {
        costValueFilterIcon.sprite = valueSprites[_value];

        UpdateCardDisplay();
    }

    private void ClassFilterHandler(int _value)
    {
        classFilterIcon.sprite = CardManager.instance.cardClassIcons[_value];

        UpdateCardDisplay();
    }
    
    public void FilterReset()
    {
        cardTypeDropDown.value = 0;
        cardTearDropDown.value = 0;
        cardTearValueDropDown.value = 0;
        cardLevelDropDown.value = 0;
        cardLevelValueDropDown.value = 0;
        cardCostDropDown.value = 0;
        cardCostNumberDropDown.value = 0;
        cardCostValueDropDown.value = 0;
        cardClassDropDown.value = 0;
    }

    public void SearchText(string _text)
    {
        if (_text.Length <= 0)
            UpdateCardDisplay();
        else
        {
            _text = _text.ToLower().Replace(" ", string.Empty);

            List<Card> _cardList = CardManager.instance.cardList;
            _cardList = _cardList.Where(x => x.stat.strCardName.ToLower().Replace(" ", string.Empty).Contains(_text)).ToList();

            foreach (var item in listCardDisplay)
                item.gameObject.SetActive(false);

            for (int i = 0; i < _cardList.Count; i++)
            {
                listCardDisplay[i].name = _cardList[i].stat.strCardName;
                listCardDisplay[i].UpdateCardInfo(_cardList[i]);
            }
        }
    }

    public void SetMovingCard(bool _on, Card _card)
    {
        isMovingCard = _on;
        uiCardScroll.enabled = !_on;

        if (_on)
            movingCard.UpdateCardInfo(_card);
        else
            movingCard.gameObject.SetActive(false);
    }

    void CheckUIUnderMouse()
    {
        foreach (var item in FindObjectsOfType<SelectedCard>())
            item.OnOffSelect(false);

        m_PointerEventData = new PointerEventData(m_EventSystem);
        m_PointerEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();

        m_Raycaster.Raycast(m_PointerEventData, results);

        foreach (RaycastResult result in results)
        {
            Button _button = result.gameObject.GetComponent<Button>();

            if (_button != null)
            {
                SelectedCard _selectCard = _button.GetComponent<SelectedCard>();

                if (_selectCard != null)
                    _selectCard.OnOffSelect(true);

                if (_selectCard != null && Input.GetMouseButtonUp(0))
                {
                    if (movingCard.card != _selectCard.card)
                    {
                        Debug.LogError(CardManager.instance.deckCard);
                        int _slotCardCount = CardManager.instance.deckCard.Where(x => x.stat.iCardID == movingCard.card.stat.iCardID).Count();
                        int _movingCardCount = movingCard.card.count - _slotCardCount;

                        Debug.LogError(_slotCardCount);

                        if (_movingCardCount > 0)
                        {
                            if (_slotCardCount < 2)
                            {
                                _selectCard.SetCard(movingCard.card);
                                SaveCurrentDeck();
                            }
                            else
                                GameManager.instance.OpenConfirm("There is a limit of 2 cards with the same name.", null, false);
                        }
                        else
                        {
                            GameManager.instance.OpenConfirm("Insufficient number of cards.", null, false);
                        }
                    }

                    _selectCard.OnOffSelect(false);
                }
            }
        }
    }

    public void OpenSaveDeckPanel()
    {
        deckListTitleText.text = "Save Deck";

        bool _cardCountNot = false;

        foreach (var item in listDeckPanel)
        {
            int _slotCardCount = CardManager.instance.deckCard.Where(x => x.stat.iCardID == item.card.stat.iCardID).Count();
            int _deckCardCount = item.card.count - _slotCardCount;

            if (_deckCardCount < 0)
            {
                _cardCountNot = true;
                break;
            }
        }

        if (CardManager.instance.deckCard.Any(x => !x.own) || _cardCountNot)
            GameManager.instance.OpenConfirm("Some card exists that you don't have in this deck.", null, false);
        else
        {
            foreach (var item in listDeckPanelButtons)
            {
                item.ChangeButtonAction(true);
                candidateListPanel.gameObject.SetActive(true);
            }
        }
    }

    public void OpenLoadDeckPanel()
    {
        deckListTitleText.text = "Load Deck";
        candidateListPanel.gameObject.SetActive(true);

        for (int i = 0; i < listDeckPanelButtons.Count; i++)
        {
            int _i = i;
            listDeckPanelButtons[_i].ChangeButtonAction(false);
            listDeckPanelButtons[_i].SetCardList(deckNameList[_i], candidateCardList[_i]);
        }
    }

    public void UpdateDeck(List<int> _deckList)
    {
        for (int i = 0; i < _deckList.Count; i++)
        {
            if (_deckList[i] != 0)
                SetDeckCard(i, _deckList[i]);
        }

        CardManager.instance.SetDeckCard(_deckList);
    }

    public void UpdateCandidate(List<string> _deckNames, List<List<int>> _deckList)
    {
        deckNameList = _deckNames;
        candidateCardList = _deckList;

        try
        {
            for (int i = 0; i < listDeckPanelButtons.Count; i++)
                listDeckPanelButtons[i].SetCardList(_deckNames[i], _deckList[i]);
        }
        catch {}
    }

    public void SaveCurrentDeck()
    {
        List<int> _deckCards = new List<int>();

        for (int i = 0; i < listDeckPanel.Count; i++)
        {
            if (listDeckPanel[i].card != null)
                _deckCards.Add(listDeckPanel[i].card.stat.iCardID);
        }
        
        string _json = JsonConvert.SerializeObject(_deckCards);

        SocialManager.instance.UpdatePlayerStringData("Deck", _json, false);
        SocialManager.instance.UpdatePlayerStringData("Deck", _json, false);

        CardManager.instance.SetDeckCard(_deckCards);

        foreach (var item in listDeckPanel)
            item.ResetCard();
    }

    public void LoadDeckList(int _num)
    {
        if (candidateCardList[_num].Count <= 0)
            return;

        string _json = JsonConvert.SerializeObject(candidateCardList[_num]);

        SocialManager.instance.UpdatePlayerStringData("Deck", _json, false);

        UpdateDeck(candidateCardList[_num]);

        candidateListPanel.gameObject.SetActive(false);
    }

    public void SaveCurrentList(int _num)
    {
        if (candidateCardList[_num].Count > 0)
            GameManager.instance.OpenConfirm($"Do you want to overwrite\n\"{deckNameList[_num]}\" slot?", () => SaveList(_num));
        else
            SaveList(_num);
    }

    private void SaveList(int _num)
    {
        string _name = deckNameInput.text == string.Empty ? (deckNameList[_num] == string.Empty ? $"My Deck {_num + 1}" : deckNameList[_num]) : deckNameInput.text;
        deckNameList[_num] = _name;
        List<int> _deckCards = new List<int>();

        for (int i = 0; i < listDeckPanel.Count; i++)
        {
            if (listDeckPanel[i].card != null)
                _deckCards.Add(listDeckPanel[i].card.stat.iCardID);
        }

        candidateCardList[_num].Clear();
        candidateCardList[_num] = _deckCards;
        listDeckPanelButtons[_num].SetCardList(_name, _deckCards);

        string _nameJson = JsonConvert.SerializeObject(deckNameList);
        string _json = JsonConvert.SerializeObject(candidateCardList);

        SocialManager.instance.UpdatePlayerStringData("DeckNameList", _nameJson, false);
        SocialManager.instance.UpdatePlayerStringData("CandidateDeckList", _json, false);

        candidateListPanel.gameObject.SetActive(false);
    }

    public void DeleteDeck(int _num)
    {
        deckNameList[_num] = string.Empty;

        candidateCardList[_num].Clear();
        listDeckPanelButtons[_num].ResetCardList();

        string _nameJson = JsonConvert.SerializeObject(deckNameList);
        string _json = JsonConvert.SerializeObject(candidateCardList);

        Debug.LogError(_json);

        SocialManager.instance.UpdatePlayerStringData("DeckNameList", _nameJson, false);
        SocialManager.instance.UpdatePlayerStringData("CandidateDeckList", _json, false);
    }

    public void OnOffDecknameInput(bool _on)
    {
        deckNameInput.text = string.Empty;
        deckNameInput.gameObject.SetActive(_on);
    }

    public void FindCard(int _cardId)
    {
        CardDisplay _card = null;

        foreach (var item in listCardDisplay)
        {
            if (item.card.stat.iCardID == _cardId)
                _card = item;
        }

        float _value = _card.GetComponent<RectTransform>().anchoredPosition.x / uiCardScroll.content.sizeDelta.x;

        if (_value < 0.5f)
            _value -= 0.05f;
        else
            _value += 0.05f;

        if (_value > 0.95f)
            _value = 1f;
        if (_value < 0.05f)
            _value = 0;

        uiCardScroll.horizontalNormalizedPosition = _value;

        StopCoroutine(BlinkCardCoroutine(_card));
        StartCoroutine(BlinkCardCoroutine(_card));
    }

    IEnumerator BlinkCardCoroutine(CardDisplay _cardDp)
    {
        _cardDp.OnOffGlow(true);

        yield return new WaitForSeconds(1f);

        _cardDp.OnOffGlow(false);
    }

    public void AllOffGlow()
    {
        foreach (var item in listCardDisplay)
            item.OnOffGlow(false);
    }

    public void ResetCandidate()
    {
        candidateCardList.Clear();
    }

    public bool createOn { get => createToggle.isOn; }
}
