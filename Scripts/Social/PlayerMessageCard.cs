using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMessageCard : MonoBehaviour
{
    [SerializeField, ReadOnlyInspector]
    private Button button;
    public PlayerMessage message;
    [SerializeField]
    private GameObject readedImage;
    [SerializeField]
    private TextMeshProUGUI titleText, senderText, dateText, btnText;
    [SerializeField, ReadOnlyInspector]
    private Color orgTextColor;
    [SerializeField]
    private Image diamondImage, presentImage;
    [SerializeField]
    private TextMeshProUGUI diaCount, preCount;
    [SerializeField]
    private Toggle toggle;
    [SerializeField]
    private Button receiveBtn;

    [ContextMenu("Init")]
    private void Init()
    {
        button = GetComponent<Button>();
        orgTextColor = btnText.color;
        diaCount = diamondImage.GetComponentInChildren<TextMeshProUGUI>();
        preCount = presentImage.GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetMessage(PlayerMessage _message)
    {
        message = _message;

        readedImage.SetActive(_message.readed);
        titleText.text = _message.title;
        senderText.text = _message.sender;
        dateText.text = _message.date.ToLocalTime().ToString("yyyy-MM-dd HH:mm");
        diamondImage.gameObject.SetActive(_message.diamond > 0);
        diamondImage.color = new Color(1f, 1f, 1f, _message.received ? 0.5f : 1f);
        diaCount.color = diamondImage.color;
        presentImage.gameObject.SetActive(_message.presentList != null && _message.presentList.Count > 0);
        diaCount.text = _message.diamond.ToString();
        if (_message.presentList != null)
            preCount.text = _message.presentList.Count.ToString();
        receiveBtn.gameObject.SetActive(_message.diamond > 0 || _message.presentList != null && _message.presentList.Count > 0);
        receiveBtn.interactable = !_message.received;
        Color _a = orgTextColor;
        _a.a = 0.5f;
        btnText.color = _message.received ? _a : orgTextColor;

        PlayerMessage _copyMessage = _message;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => MainSceneManager.instance.playerMessage.OpenPaper(_copyMessage));
        receiveBtn.onClick.RemoveAllListeners();
        receiveBtn.onClick.AddListener(() => SetMessage(SocialManager.instance.ReceivePresents(message.code)));
    }

    public void OnOffToggle(bool _on)
    {
        toggle.isOn = _on;
    }

    public bool isOn { get => toggle.isOn; }
}
