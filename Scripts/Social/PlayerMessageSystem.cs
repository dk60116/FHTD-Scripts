using Newtonsoft.Json;
using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class PlayerMessageSystem : MonoBehaviour
{
    [SerializeField]
    private ScrollRect scroll, paperScroll;
    [SerializeField]
    private PlayerMessageCard messageCardPrefab;
    [SerializeField]
    private List<PlayerMessageCard> messageCardList;
    [SerializeField]
    private Button deleteBtn, receiveAllBtn;
    [SerializeField]
    private Toggle selectAllToggle;

    [SerializeField]
    private RectTransform paper;
    [SerializeField]
    private TextMeshProUGUI paperTitleText, paperSenderText, PaperDetailText, paperDiaCountText, paperBtnText;
    [SerializeField]
    private Image paperProfileImage;
    [SerializeField]
    private Image paperDiamond;
    [SerializeField]
    private Button paperReceveBtn;
    private Color orgTextColor;

    [SerializeField]
    private Image messageCountFrame;
    [SerializeField]
    private TextMeshProUGUI messageCountText;

    void Awake()
    {
        orgTextColor = paperBtnText.color;
        deleteBtn.onClick.AddListener(() => DeleteSelectedMessage());
        selectAllToggle.onValueChanged.AddListener((bool _on) => SelectAllCard(_on));
    }

    void OnEnable()
    {
        scroll.verticalNormalizedPosition = 1f;
        foreach (var item in messageCardList)
            item.gameObject.SetActive(false);
        UpdateMessageList();

        foreach (var item in messageCardList)
            item.OnOffToggle(false);

        selectAllToggle.isOn = false;
    }

    [ContextMenu("Init")]
    private void Init()
    {
        messageCardList = new List<PlayerMessageCard>();

        for (int i = 0; i < scroll.content.childCount; i++)
            messageCardList.Add(scroll.content.GetChild(i).GetComponent<PlayerMessageCard>());
    }

    public void UpdateMessageList()
    {
        GetPlayerMessageList();
    }

    private void GetPlayerMessageList()
    {
        var request = new GetUserDataRequest
        {
            Keys = new List<string> { "Level", "Exp", "ProfileImage", "PlayerMessage" }
        };

        PlayFabClientAPI.GetUserData(request, OnMessageListReceived, OnMessageListReceiveFailed);
    }

    private void OnMessageListReceived(GetUserDataResult result)
    {
        SocialManager.instance.playerMessageList = JsonConvert.DeserializeObject<List<PlayerMessage>>(result.Data["PlayerMessage"].Value);

        for (int i = 0; i < SocialManager.instance.playerMessageList.Count; i++)
        {
            if (i >= messageCardList.Count)
            {
                PlayerMessageCard _newCard = Instantiate(messageCardPrefab, scroll.content).GetComponent<PlayerMessageCard>();
                messageCardList.Add(_newCard);
            }

            messageCardList[i].SetMessage(SocialManager.instance.playerMessageList[i]);
            messageCardList[i].gameObject.SetActive(true);
        }

        for (int i = 0; i < messageCardList.Count; i++)
        {
            if (i >= SocialManager.instance.playerMessageList.Count)
                messageCardList[i].gameObject.SetActive(false);
        }

        int _messageCount = SocialManager.instance.playerMessageList.Where(x => !x.readed || !x.received).ToList().Count;

        messageCountFrame.gameObject.SetActive(_messageCount > 0);
        messageCountText.text = _messageCount.ToString();
    }

    private void OnMessageListReceiveFailed(PlayFabError error)
    {
        Debug.LogError(error.Error);
    }

    public void OpenPaper(PlayerMessage _message)
    {
        paperScroll.verticalNormalizedPosition = 1f;
        paper.gameObject.SetActive(true);
        paperTitleText.text = _message.title;
        paperSenderText.text = _message.sender;
        PaperDetailText.text = _message.detail;
        paperDiamond.gameObject.SetActive(_message.diamond > 0);
        paperDiamond.color = _message.received ? new Color(1f, 1f, 1f, 0.5f) : Color.white;
        paperDiaCountText.color = paperDiamond.color;
        paperDiaCountText.text = _message.diamond.ToString();
        paperReceveBtn.interactable = !_message.received;
        Color _a = orgTextColor;
        _a.a = 0.5f;
        paperBtnText.color = _message.received ? _a : orgTextColor;

        SocialManager.instance.ReadMessage(_message.code);
    }

    public void DeleteSelectedMessage()
    {
        List<PlayerMessageCard> _messages = messageCardList.Where(x => x.isOn && x.message.readed && x.message.received).ToList();

        if (_messages.Count <= 0)
        {
            GameManager.instance.OpenConfirm("No messages are available for deletion.", null, false);
            return;
        }

        List<string> _codeList = new List<string>();

        for (int i = 0; i < _messages.Count; i++)
            _codeList.Add(_messages[i].message.code);
        
        SocialManager.instance.DeletePlayerMessage(_codeList);
    }

    private void SelectAllCard(bool _on)
    {
        foreach (var item in messageCardList)
            item.OnOffToggle(_on);
    }
}
