using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectDeckListButton : MonoBehaviour
{
    public int num;

    [SerializeField]
    private RectTransform btnArea;
    [SerializeField]
    private Button btn, deleteBtn;

    [SerializeField]
    private Image firstCardIcon;
    [SerializeField]
    private Sprite emptyCardIcon;
    [SerializeField]
    private TextMeshProUGUI deckNameText;

    [ReadOnlyInspector]
    private List<int> cardList;

    void Awake()
    {
        deleteBtn.onClick.AddListener(() => DeleteDeck());
        deleteBtn.interactable = false;
    }

    public void ResetCardList()
    {
        deckNameText.text = string.Empty;
        cardList = new List<int>();
        firstCardIcon.sprite = emptyCardIcon;
    }

    public void SetCardList(string _deckName, List<int> _cardList)
    {
        deckNameText.text = _deckName;
        cardList = _cardList;
        bool _on = _cardList.Count > 0;
        if (_on)
        {
            firstCardIcon.sprite = CardManager.instance.GetCardWithID(_cardList[0]).stat.imgCardIcon;
        }
        else
            firstCardIcon.sprite = emptyCardIcon;

        deleteBtn.interactable = _on;
    }

    public void ChangeButtonAction(bool _saveMode)
    {
        btnArea.gameObject.SetActive(!_saveMode);
        MainSceneManager.instance.cardDeckPanel.OnOffDecknameInput(_saveMode);  
        btn.onClick.RemoveAllListeners();

        if (_saveMode)
            btn.onClick.AddListener(() => MainSceneManager.instance.cardDeckPanel.SaveCurrentList(num));
        else
            btn.onClick.AddListener(() => MainSceneManager.instance.cardDeckPanel.LoadDeckList(num));
    }

    public void DeleteDeck()
    {
        if (cardList.Count > 0)
            GameManager.instance.OpenConfirm($"Do you really want to delete \"{deckNameText.text}\" deck?", () => DeleteDeckHandler());
    }

    private void DeleteDeckHandler()
    {
        MainSceneManager.instance.cardDeckPanel.DeleteDeck(num);
        deleteBtn.interactable = false;
    }

    public void DisinteractableDeleteBtn()
    {
        deleteBtn.interactable = false;
    }

    public List<int> getCardList { get => cardList;}
}
