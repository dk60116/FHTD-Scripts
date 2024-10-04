using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Newtonsoft.Json;

public class MainShopConfirm : MonoBehaviour
{
    [SerializeField]
    private ScrollRect scroll;
    [SerializeField]
    private TextMeshProUGUI titleText, descriptionText, diamondText;
    [SerializeField]
    private RawImage conceptImage;
    [SerializeField]
    private Button singleBuybutton, pluralBuyBtn;
    [SerializeField]
    private TextMeshProUGUI diamondText_Single, diamondText_Plural, buyText_Single, buyText_Plural;

    private void OnEnable()
    {
        scroll.verticalNormalizedPosition = 1f;
        diamondText.text = SocialManager.instance.GetCurrency(CurrencyType.DI).ToString();
    }

    public void UpdateConfirm(MainShopComponents _component, RawImage _image, int _number)
    {
        pluralBuyBtn.gameObject.SetActive(_component.stackable[_number]);
        titleText.text = _component.contentName[_number];
        descriptionText.text = _component.itemDescription[_number];
        conceptImage.texture = _image.texture;
        diamondText_Single.text = _component.cost[_number].ToString();
        int _totalCost = _component.cost[_number] * _component.stackCount[_number];
        diamondText_Plural.text = _totalCost.ToString();

        string _buyText_Single = "Buy";
        string _buyText_Plural = "Buy";

        if (_component.stackable[_number])
        {
            _buyText_Single = "Buy 1";
            _buyText_Plural += $" {_component.stackCount[_number]}";
        }

        buyText_Single.text = _buyText_Single;
        buyText_Plural.text = _buyText_Plural;

        string _items = string.Empty;

        if (_component.itemList != null)
            _items = JsonConvert.SerializeObject(_component.itemList[_number]);

        Debug.LogError(_items);

        singleBuybutton.onClick.RemoveAllListeners();
        singleBuybutton.onClick.AddListener(() => GameManager.instance.OpenConfirm($"Are you really buying\nthis item?\nDiamond {SocialManager.instance.GetCurrency(CurrencyType.DI)} >> {SocialManager.instance.GetCurrency(CurrencyType.DI) - _component.cost[_number]}", new UnityAction(() => BuyItem(_component.requestFunctionName[_number], _number, _component.contentId[_number], _items, 1, _component.canSoldOut[_number]))));
        pluralBuyBtn.onClick.AddListener(() => GameManager.instance.OpenConfirm($"Are you really buying\nthis item?\nDiamond {SocialManager.instance.GetCurrency(CurrencyType.DI)} >> {SocialManager.instance.GetCurrency(CurrencyType.DI) - _totalCost}", new UnityAction(() => BuyItem(_component.requestFunctionName[_number], _number, _component.contentId[_number], _items, _component.stackCount[_number], _component.canSoldOut[_number]))));

        singleBuybutton.interactable = SocialManager.instance.GetCurrency(CurrencyType.DI) >= _component.cost[_number];
        pluralBuyBtn.interactable = SocialManager.instance.GetCurrency(CurrencyType.DI) >= _totalCost;
    }

    private void BuyItem(string _requestFunctionName, int _num, string _shopId, string _itemId, int _count, bool _soldOut)
    {
        GameManager.instance.OnOffLoadingPanel(true);

        string _id = $"[{_itemId}]";

        var request = new ExecuteCloudScriptRequest
        {
            FunctionName = _requestFunctionName,
            FunctionParameter = new
            {
                shopId = _shopId,
                itemId = _id,
                count = _count
            },
            GeneratePlayStreamEvent = true,
        };

        Debug.LogError($"{_requestFunctionName}: {_itemId}");

        PlayFabClientAPI.ExecuteCloudScript(request, result => OnRequestBuyItemSuccess(result, _num, _shopId, _soldOut), OnRequestBuyItemFailure);
    }

    private void OnRequestBuyItemSuccess(ExecuteCloudScriptResult result, int _num, string _shopId, bool _soldOut)
    {
        Debug.LogError(result.FunctionResult);

        if (result.FunctionResult == null)
        {
            GameManager.instance.OnOffLoadingPanel(false);
            GameManager.instance.OpenConfirm("Temporary error.\nPlease try again.", null, false);
        }

        string _functionResult = result.FunctionResult.ToString().Replace("CardList", string.Empty);

        if (_functionResult != "Not enough currency")
        {
            if (result.FunctionResult.ToString().Contains("CardList"))
            {
                SocialManager.instance.UpdateCurrencyCount();
                List<string> _cardList = JsonConvert.DeserializeObject<List<string>>(_functionResult);
                MainSceneManager.instance.openCardSystem.cg.alpha = 0;
                MainSceneManager.instance.openCardSystem.gameObject.SetActive(true);
                MainSceneManager.instance.openCardSystem.UpdateCards(_cardList);
            }
            else
            {
                GameManager.instance.OpenConfirm(result.FunctionResult.ToString(), null, false);
            }

            gameObject.SetActive(false);

            GameManager.instance.OnOffLoadingPanel(false);
            SocialManager.instance.UpdateCurrencyCount();
            SocialManager.instance.GetPlayerInfo(SocialManager.instance.playerInfo.masterID, SocialManager.instance.playerInfo.nickName);

            MainSceneManager.instance.mainShopPanel.ResetLine(_num, _shopId, _soldOut);
        }
    }

    private void OnRequestBuyItemFailure(PlayFabError error) 
    {
        Debug.LogError(error.GenerateErrorReport());
        GameManager.instance.OnOffLoadingPanel(false);
    }
}
