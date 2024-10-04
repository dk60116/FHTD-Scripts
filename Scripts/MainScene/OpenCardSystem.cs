using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OpenCardSystem : MonoBehaviour
{
    public CanvasGroup cg;
    [SerializeField]
    private List<CardDP_Open> cardDP;
    [SerializeField]
    private Button skipBtn;
    [SerializeField]
    private TextMeshProUGUI skipText;
    public RectTransform infoArea;

    void OnDisable()
    {
        GameManager.instance.CloseCardInfo();
    }

    public void UpdateCards(List<string> _cardList)
    {
        Debug.LogError(_cardList.Count);

        foreach (var item in cardDP)
        {
            item.isOpen = false;
            item.gameObject.SetActive(false);
        }

        for (int i = 0; i < _cardList.Count; i++)
        {
            cardDP[i].UpdateCardInfo(CardManager.instance.GetCardWithID(int.Parse(_cardList[i])));
            cardDP[i].gameObject.SetActive(true);
        }

        skipBtn.onClick.RemoveAllListeners();
        skipBtn.onClick.AddListener(() => SkipButtonHandler(_cardList.Count));
        skipText.text = "OPEN";

        StartCoroutine(ChekAllOpen(_cardList.Count));

        cg.alpha = 1f;
    }

    private void SkipButtonHandler(int _count)
    {
        StartCoroutine(SkipCoroutine(_count));
    }

    IEnumerator SkipCoroutine(int _count)
    {
        skipBtn.interactable = false;

        skipText.text = "OK";

        for (int i = 0; i < _count; i++)
        {
            cardDP[i].OpenCard();

            yield return new WaitForSeconds(0.1f);
        }

        skipBtn.onClick.RemoveAllListeners();
        skipBtn.onClick.AddListener(() => gameObject.SetActive(false));
        skipBtn.interactable = true;
    }

    IEnumerator ChekAllOpen(int _count)
    {
        skipText.text = "OPEN";
        yield return new WaitUntil(() => OpenCardCount() >= _count);

        skipText.text = "OK";
        skipBtn.onClick.RemoveAllListeners();
        skipBtn.onClick.AddListener(() => gameObject.SetActive(false));
    }

    private int OpenCardCount()
    {
        int _count = 0;

        foreach (var item in cardDP)
        {
            if (item.isOpen)
                _count++;
        }

        return _count;
    }
}
