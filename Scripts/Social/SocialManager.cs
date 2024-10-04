using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;
using System.Linq;
using System;
using Newtonsoft.Json;
using Assets.UTime;
using Random = UnityEngine.Random;
using GooglePlayGames.BasicApi;
using GooglePlayGames;

[Serializable]
public enum CurrencyType { DI, PD }
[Serializable]
public enum ItemType { Card, Test }

public class SocialManager : SingletonMono<SocialManager>
{
    public Text logText;

    public LoginSystem loginSys;

    public PlayerSocialInfo playerInfo;

    public List<Sprite> profileIconSpriteList, profileFrameSpriteList;

    public List<PlayerMessage> playerMessageList;

    [SerializeField]
    private RawImage gpgsimg;

    protected override void Awake()
    {
        base.Awake();
        UTime.HasConnection(connection => Debug.Log("Connection: " + connection));
        playerMessageList = new List<PlayerMessage>();
    }

    void Start()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();

#if !UNITY_EDITOR
        GoogleLogIn();
#endif
        Debug.LogError(PlayerPrefs.GetString("PlayFabSessionToken"));
    }

    [ContextMenu("Init")]
    private void Init()
    {
        loginSys = FindObjectOfType<LoginSystem>(true);
        profileIconSpriteList = new List<Sprite>();
    }

    public void GoogleLogIn()
    {
        GameManager.instance.BuildDebugLog("구글 로그인 시도");

        Social.localUser.Authenticate(success => {
            if (success)
            {
                Debug.LogError("Start SignIn for Token");
                SignInForGooglePlayerToken(Social.localUser.id);
                gpgsimg.texture = Social.localUser.image;
            }
            else
            {
                Debug.Log("Failed to log in to Google Play Games");
            }
        });
    }

    private void SignInForGooglePlayerToken(string _token)
    {
        Debug.LogError(_token);
        var request = new LoginWithEmailAddressRequest { Email = $"{_token}@gmail.com", Password = _token, InfoRequestParameters = new GetPlayerCombinedInfoRequestParams() { GetPlayerProfile = true } };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnGoogleLoginSuccess, error => OnLoginFailure(error, _token));
    }

    private void OnGoogleLoginSuccess(LoginResult result)
    {
        Debug.LogError("Google Token Login Success");

        MainSceneManager.instance.loginPanel.gameObject.SetActive(false);

        if (result.InfoResultPayload.PlayerProfile.DisplayName != null)
        {
            string _displayName = result.InfoResultPayload.PlayerProfile.DisplayName;
            Debug.Log("User Display Name: " + _displayName);
           instance.CheckAndAssignNumberToNewUser();

            GetPlayerInfo(result.PlayFabId, _displayName);
            UpdateCurrencyCount();
        }
        else
        {
            Debug.Log("No DisplayName available");
            MainSceneManager.instance.nicknamePanel.gameObject.SetActive(true);
        }
    }

    private void OnLoginFailure(PlayFabError error, string _token)
    {
        switch (error.Error)
        {
            case PlayFabErrorCode.InvalidParams:
                break;
            case PlayFabErrorCode.AccountNotFound:
                Debug.LogError("Signup Start");
                SignupForGoogleToken(_token);
                break;
        }
    }

    private void SignupForGoogleToken(string _token)
    {
        var request = new RegisterPlayFabUserRequest { Email = $"{_token}@gmail.com", Password = _token, Username = $"USER{Tools.GenerateRandomHex(8)}" };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterFailure);
    }

    void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        Debug.LogError("회원가입 성공");
        SignInForGooglePlayerToken(Social.localUser.id);
    }

    void OnRegisterFailure(PlayFabError error)
    {
        Debug.LogError("회원가입 실패");
        Debug.LogError(error.Error);
    }
    
    public string GetGoogleNickname()
    {
        return Social.localUser.userName;
    }

    public Texture2D GetGoogleIcon()
    {
        return Social.localUser.image;
    }

    public void CheckAndAssignNumberToNewUser()
    {
        Debug.LogError("Check");

        var request = new GetUserDataRequest
        {
            Keys = new List<string> { "FirstTimeLogin" }
        };

        PlayFabClientAPI.GetUserData(request, OnCheckDataReceived, OnCheckError);
    }

    void OnCheckDataReceived(GetUserDataResult result)
    {
        if (result.Data == null || !result.Data.ContainsKey("FirstTimeLogin"))
        {
            RequestFirstLogin();
        }
        else
        {
            Debug.Log("User already has a number assigned.");
        }
        
        CheckCardInventory();
    }

    public void RequestFirstLogin()
    {
        GameManager.instance.OnOffLoadingPanel(true);

        var _request = new ExecuteCloudScriptRequest()
        {
            FunctionName = "FirstLogin"
        };

        PlayFabClientAPI.ExecuteCloudScript(_request, OnFirstLoginGetItemsSuccess, FailedFirstLoginGetItems);
    }

    public void RequestFirstLoginNext()
    {
        var _request = new ExecuteCloudScriptRequest()
        {
            FunctionName = "FirstLoginNext"
        };

        PlayFabClientAPI.ExecuteCloudScript(_request, OnFirstLoginPlayerDataSuccess, FailedFirstLoginGetItems);
    }

    private void OnFirstLoginGetItemsSuccess(ExecuteCloudScriptResult _result)
    {
        var logs = _result.Logs;

        Debug.LogError(logs.Count);

        foreach (var item in logs)
        {
            Debug.LogError(item.Message);
        }

        Debug.LogError(_result.FunctionResult);

        if (_result.FunctionResult != null)
            Invoke(nameof(RequestFirstLoginNext), 0.1f);
        else
            RequestFirstLogin();
    }

    private void FailedFirstLoginGetItems(PlayFabError _error)
    {
        Debug.LogError(_error.ErrorDetails);
    }

    private void FailedFirstLoginPlayerData(PlayFabError _error)
    {
        Debug.LogError(_error.ErrorDetails);
    }

    private void OnFirstLoginPlayerDataSuccess(ExecuteCloudScriptResult _result)
    {
        var logs = _result.Logs;

        Debug.LogError(logs.Count);

        foreach (var item in logs)
        {
            Debug.LogError(item.Message);
        }

        Debug.LogError(_result.FunctionResult);

        if (_result.FunctionResult == null)
        {
            RequestFirstLogin();
            return;
        }

        UpdateCurrencyCount();
        MainSceneManager.instance.playerMessage.UpdateMessageList();
        playerInfo.haveCardFrame = new List<string> { "300400" };
        playerInfo.haveProfileIcon = new List<string> { "310100", "300101" };
        playerInfo.haveProfileFrame = new List<string> { "320100" };
        playerInfo.equipCardFrame = "300400";
        playerInfo.equipProfileIcon = "310100";
        playerInfo.equipProfileFrame = "320100";
        playerInfo.purchasedList = new List<string>();
        MainSceneManager.instance.cardDeckPanel.UpdateCandidate(new List<string>(), new List<List<int>>());
        GetPlayerInfo(playerInfo.masterID, playerInfo.nickName);

        GameManager.instance.OnOffLoadingPanel(false);
    }

    void OnCheckError(PlayFabError error)
    {
        Debug.LogError("Error: " + error.ErrorMessage);
    }

    public void SignOut()
    {
        PlayFabClientAPI.ForgetAllCredentials();
        Debug.Log("Logout");
        logText.text = "로그아웃";
        PlayerPrefs.DeleteKey("PlayFabSessionToken");
        PlayGamesPlatform.Instance.SignOut();
        MainSceneManager.instance.OnOffBlackPanel(true);
        MainSceneManager.instance.loginSys.SignOutBtn();
        MainSceneManager.instance.loginPanel.gameObject.SetActive(true);
    }

    private void CheckCardInventory()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), OnCheckCardReceived, OnCheckCardFailed);
    }

    private void OnCheckCardReceived(GetUserInventoryResult result)
    {
        if (!result.Inventory.Any(x => x.ItemId == "301000"))
        {
            PurchaseItem(ItemType.Card, "301000", CurrencyType.PD, 0);
            PurchaseItem(ItemType.Card, "301001", CurrencyType.PD, 0);
            PurchaseItem(ItemType.Card, "301002", CurrencyType.PD, 0);
            PurchaseItem(ItemType.Card, "301003", CurrencyType.PD, 0);
            PurchaseItem(ItemType.Card, "301004", CurrencyType.PD, 0);
            PurchaseItem(ItemType.Card, "301005", CurrencyType.PD, 0);
            PurchaseItem(ItemType.Card, "301006", CurrencyType.PD, 0);
            PurchaseItem(ItemType.Card, "301000", CurrencyType.PD, 0);
            PurchaseItem(ItemType.Card, "301001", CurrencyType.PD, 0);
            PurchaseItem(ItemType.Card, "301002", CurrencyType.PD, 0);
            PurchaseItem(ItemType.Card, "301003", CurrencyType.PD, 0);
            PurchaseItem(ItemType.Card, "301004", CurrencyType.PD, 0);
            PurchaseItem(ItemType.Card, "301005", CurrencyType.PD, 0);
            PurchaseItem(ItemType.Card, "301006", CurrencyType.PD, 0);
        }

        MainSceneManager.instance.OnOffBlackPanel(false);
        GameManager.instance.OnOffLoadingPanel(false);
    }

    void OnCheckCardFailed(PlayFabError error)
    {
        Debug.LogError("PlayFab Error: " + error.ErrorMessage);
    }

    public void UpdateNickname(string _nickname)
    {
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = _nickname
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnDisplayNameUpdated, OnUpdateNicknameError);
    }

    void OnDisplayNameUpdated(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log("DisplayName updated successfully to: " + result.DisplayName);

        MainSceneManager.instance.OnOffBlackPanel(false);
        MainSceneManager.instance.nicknamePanel.gameObject.SetActive(false);
        MainSceneManager.instance.loginPanel.gameObject.SetActive(false);
    }

    void OnUpdateNicknameError(PlayFabError error)
    {
        Debug.LogError("Error updating DisplayName: " + error.ErrorMessage);
    }

    public void UpdatePlayerIntData(string _key, int _value, bool _permission)
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { _key, _value.ToString() }
            },
            Permission = _permission ? UserDataPermission.Public : UserDataPermission.Private
        };

        PlayFabClientAPI.UpdateUserData(request, OnPlayerIntValueChanged, OnPlayerIntValueChangeFailed);
    }

    private void OnPlayerIntValueChanged(UpdateUserDataResult result)
    {
        Debug.LogError(result);
    }

    private void OnPlayerIntValueChangeFailed(PlayFabError error)
    {
        Debug.LogError(error.Error + " " + error.ErrorMessage);
    }

    public void UpdatePlayerStringData(string _key, string _data, bool _permission)
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { _key, _data }
            },
            Permission = _permission ? UserDataPermission.Public : UserDataPermission.Private
        };

        PlayFabClientAPI.UpdateUserData(request, OnPlayerStringValueChanged, OnPlayerStringValueChangeFailed);
    }

    private void OnPlayerStringValueChanged(UpdateUserDataResult result)
    {
        Debug.LogError(result);
        GetPlayerInfo(playerInfo.masterID, playerInfo.nickName);
    }

    private void OnPlayerStringValueChangeFailed(PlayFabError error)
    {
        Debug.LogError(error.Error + " " + error.ErrorMessage);
    }

    public void GetPlayerInfo(string _masterID, string _nickname)
    {
        playerInfo.masterID = _masterID;
        playerInfo.nickName = _nickname;

        var _request = new GetUserDataRequest
        {
        };

        PlayFabClientAPI.GetUserData(_request, OnPlayerInfoReceived, OnPlayerInfoReceiveFailed);
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), OnGetInventorySuccess, OnGetInventoryFailure);
    }

    private void OnPlayerInfoReceived(GetUserDataResult result)
    {
        Debug.LogError("InfoReceive");

        if (!result.Data.ContainsKey("Level"))
            return;

        Debug.LogError($"Level: {result.Data["Level"].Value}");
        Debug.LogError($"Exp: {result.Data["Exp"].Value}");
        Debug.LogError($"ProfileImage: {result.Data["ProfileImage"].Value}");
        Debug.LogError($"Deck: {result.Data["Deck"].Value}");
        Debug.LogError($"DeckNameList: {result.Data["DeckNameList"]}");
        Debug.LogError($"CandidateDeckList: {result.Data["CandidateDeckList"].Value}");
        Debug.LogError($"PlayerMessage: {result.Data["PlayerMessage"].Value}");
        Debug.LogError($"ProfileImage: {result.Data["ProfileImage"].Value}");
        Debug.LogError($"ProfileFrame: {result.Data["ProfileFrame"].Value}");
        Debug.LogError($"EuipCardFrame: {result.Data["EuipCardFrame"].Value}");
        Debug.LogError($"PurchasedList: {result.Data["PurchasedList"].Value}");

        playerInfo.level = int.Parse(result.Data["Level"].Value);
        playerInfo.exp = int.Parse(result.Data["Exp"].Value);
        playerInfo.equipProfileIcon = result.Data["ProfileImage"].Value;
        playerInfo.equipProfileFrame = result.Data["ProfileFrame"].Value;
        playerMessageList = JsonConvert.DeserializeObject<List<PlayerMessage>>(result.Data["PlayerMessage"].Value);
        playerInfo.equipCardFrame = result.Data["EuipCardFrame"].Value;
        playerInfo.purchasedList = JsonConvert.DeserializeObject<List<string>>(result.Data["PurchasedList"].Value);
        List<int> _deckCardList = JsonConvert.DeserializeObject<List<int>>(result.Data["Deck"].Value);
        List<string> _deckNameList = JsonConvert.DeserializeObject<List<string>>(result.Data["DeckNameList"].Value);
        if (_deckNameList.Count == 0)
            _deckNameList = new List<string>() { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty };
        List<List<int>> _deckList = JsonConvert.DeserializeObject<List<List<int>>>(result.Data["CandidateDeckList"].Value);

        MainSceneManager.instance.socialPanel.UpdateProfile(playerInfo.masterID, Social.localUser.userName, GetGoogleIcon());
        MainSceneManager.instance.userFrame.UpdateProfile(playerInfo.level, playerInfo.exp, playerInfo.nickName, playerInfo.equipProfileIcon, playerInfo.equipProfileFrame);
        MainSceneManager.instance.playerMessage.UpdateMessageList();
        MainSceneManager.instance.cardDeckPanel.UpdateDeck(_deckCardList);
        MainSceneManager.instance.cardDeckPanel.UpdateCandidate(_deckNameList, _deckList);
    }

    private void OnGetInventorySuccess(GetUserInventoryResult result)
    {
        List<string> _cardframeList = new List<string>();
        List<string> _iconList = new List<string>();
        List<string> _profileFrameList = new List<string>();

        foreach (var item in result.Inventory)
        {
            if (item.ItemClass == "CardFrame")
                _cardframeList.Add(item.ItemId);
            else if (item.ItemClass == "ProfielIcon")
                _iconList.Add(item.ItemId);
            else if (item.ItemClass == "ProfileFrame")
                _profileFrameList.Add(item.ItemId);
        }

        playerInfo.haveCardFrame = _cardframeList;
        playerInfo.haveProfileIcon = _iconList;
        playerInfo.haveProfileFrame = _profileFrameList;
    }

    void OnGetInventoryFailure(PlayFabError error)
    {
        Debug.LogError($"Error getting inventory: {error.ErrorMessage}");
    }

    public int GetCurrency(CurrencyType _type)
    {
        switch (_type)
        {
            case CurrencyType.DI:
                return playerInfo.diamond;
            case CurrencyType.PD:
                return playerInfo.powder;
        }

        return 0;
    }

    private void OnPlayerInfoReceiveFailed(PlayFabError error)
    {
        Debug.LogError(error.Error);
    }

    public void UpdateCurrencyText(int _diamonds, int _powder)
    {
        Debug.LogError("UpdateCurrency");

        foreach (var item in FindObjectsOfType<CurrencyText>(true))
        {
            switch (item.currency)
            {
                case CurrencyType.DI:
                    item.UpdateCount(_diamonds);
                    break;
                case CurrencyType.PD:
                    item.UpdateCount(_powder);
                    break;
            }
        }
    }

    public void UpdateCurrencyCount()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), OnUpdateCurrencySuccess, OnUpdateDiamondFailed);
    }

    private void OnUpdateCurrencySuccess(GetUserInventoryResult result)
    {
        if (result.VirtualCurrency.ContainsKey(CurrencyType.DI.ToString()))
        {
            int _diamonds = result.VirtualCurrency[CurrencyType.DI.ToString()];
            int _powder = result.VirtualCurrency[CurrencyType.PD.ToString()];

            playerInfo.diamond = _diamonds;
            playerInfo.powder = _powder;

            Debug.Log($"Diamonds: {_diamonds}, Powder: {_powder}");

            UpdateCurrencyText(_diamonds, _powder);

            MainSceneManager.instance.cardDeckPanel.GetUserCardInventory();
        }
    }

    private void OnUpdateDiamondFailed(PlayFabError error)
    {
        Debug.LogError("Error: " + error.GenerateErrorReport());
    }

    public void AddCurrency(CurrencyType _currency, int _amount)
    {
        Debug.LogError(_currency);

        var request = new AddUserVirtualCurrencyRequest
        {
            VirtualCurrency = _currency.ToString(),
            Amount = _amount
        };

        PlayFabClientAPI.AddUserVirtualCurrency(request, (result) => OnAddCurrencySuccess(result, _currency), OnAddCurrencyFailed);
    }

    private void OnAddCurrencySuccess(ModifyUserVirtualCurrencyResult result, CurrencyType _currency)
    {
        foreach (var item in FindObjectsOfType<CurrencyText>(true))
        {
            switch (_currency)
            {
                case CurrencyType.DI:
                    if (item.currency == CurrencyType.DI)
                    {
                        item.UpdateCount(result.Balance);
                        playerInfo.diamond = result.Balance;
                        Debug.Log("Add Diamond: " + result.Balance);
                    }
                    break;
                case CurrencyType.PD:
                    if (item.currency == CurrencyType.PD)
                    {
                        item.UpdateCount(result.Balance);
                        playerInfo.powder = result.Balance;
                        Debug.Log("Add Powder: " + result.Balance);
                    }
                    break;
            }
        }
    }

    void OnAddCurrencyFailed(PlayFabError error)
    {
        Debug.LogError("Error: " + error.GenerateErrorReport());
    }

    public void AddDiamond(int _count)
    {
        AddCurrency(CurrencyType.DI, _count);
    }

    public void AddPowder(int _count)
    {
        AddCurrency(CurrencyType.PD, _count);
    }

    public void SubDiamond(int _amount)
    {
        var request = new SubtractUserVirtualCurrencyRequest
        {
            VirtualCurrency = CurrencyType.DI.ToString(),
            Amount = _amount
        };

        PlayFabClientAPI.SubtractUserVirtualCurrency(request, OnSubCurrencySuccess, OnSubCurrencyFailed);
    }

    private void OnSubCurrencySuccess(ModifyUserVirtualCurrencyResult result)
    {
        Debug.Log("Sub Diamond: " + result.Balance);

        foreach (var item in FindObjectsOfType<CurrencyText>(true))
        {
            if (item.currency == CurrencyType.DI)
            {
                item.UpdateCount(result.Balance);
                playerInfo.diamond = result.Balance;
            }
            else if (item.currency == CurrencyType.PD)
            {
                item.UpdateCount(result.Balance);
                playerInfo.powder = result.Balance;
            }
        }
    }

    void OnSubCurrencyFailed(PlayFabError error)
    {
        Debug.LogError("Error: " + error.GenerateErrorReport());
    }

    public void ChangeProfileImage(string _id, string _type)
    {
        GameManager.instance.OnOffMiniLoadingPanel(true);

        var _request = new ExecuteCloudScriptRequest()
        {
            FunctionName = "ChangeProfileImage",
            FunctionParameter = new
            {
                type = _type,
                id = _id
            }
        };

        PlayFabClientAPI.ExecuteCloudScript(_request, OnChangeProfileIconSuccess, FailedChangeProfileIcon);
    }

    private void OnChangeProfileIconSuccess(ExecuteCloudScriptResult _result)
    {
        Debug.LogError(_result.FunctionResult);

        if (_result.FunctionResult == null)
        {
            GameManager.instance.OnOffMiniLoadingPanel(false);
            GameManager.instance.OpenConfirm("Temporary errors", null, false);
            return;
        }

        bool _failed = true;

        _failed = bool.TryParse(_result.FunctionResult.ToString(), out _failed);

        string[] _results = _result.FunctionResult.ToString().Split(":");

        if (!_failed)
        {
            if (_results[0] == "Icon")
            {
                MainSceneManager.instance.userFrame.UpdateProfile(playerInfo.level, playerInfo.exp, playerInfo.nickName, _results[1], playerInfo.equipProfileFrame);
                playerInfo.equipProfileIcon = _result.FunctionResult.ToString().Split(":")[1];
            }
            else if (_results[0] == "Frame")
            {
                MainSceneManager.instance.userFrame.UpdateProfile(playerInfo.level, playerInfo.exp, playerInfo.nickName, playerInfo.equipProfileIcon, _results[1]);
                playerInfo.equipProfileFrame = _result.FunctionResult.ToString().Split(":")[1];
            }
        }

        GameManager.instance.OnOffMiniLoadingPanel(false);
    }

    private void FailedChangeProfileIcon(PlayFabError error)
    {
        Debug.LogError(error.ErrorDetails);
    }

    public void GetCatalogItems()
    {
        var request = new GetCatalogItemsRequest
        {
            CatalogVersion = "1"
        };

        PlayFabClientAPI.GetCatalogItems(request, OnCatalogItemsReceived, OnCatalogItemsReceiveFailed);
    }

    private void OnCatalogItemsReceived(GetCatalogItemsResult result)
    {
        foreach (var item in result.Catalog)
        {
            Debug.Log("Item ID: " + item.ItemId + ", Name: " + item.DisplayName);
        }
    }

    private void OnCatalogItemsReceiveFailed(PlayFabError error)
    {
        Debug.LogError("PlayFab Error: " + error.ErrorMessage);
    }

    public void PurchaseItem(ItemType _type, string _itemId, CurrencyType _currency, int _price)
    {
        var request = new PurchaseItemRequest
        {
            CatalogVersion = _type.ToString(),
            ItemId = _itemId,
            VirtualCurrency = _currency.ToString(),
            Price = _price
        };

        Debug.LogError($"{_type}: {_itemId}");

        PlayFabClientAPI.PurchaseItem(request, result => OnItemPurchased(result, _type), error => OnItemPurchaseError(error, _currency));
    }

    private void OnItemPurchased(PurchaseItemResult result, ItemType _type)
    {
        UpdateCurrencyCount();

        foreach (var item in result.Items)
            Debug.LogError($"{item.DisplayName} purchase success!");

        if (_type == ItemType.Card)
            MainSceneManager.instance.cardDeckPanel.GetUserCardInventory();
    }

    private void OnItemPurchaseError(PlayFabError error, CurrencyType _currency)
    {
        Debug.LogError("PurchaseError: " + error.ErrorMessage);

        if (error.Error == PlayFabErrorCode.InsufficientFunds)
        {
            switch (_currency)
            {
                case CurrencyType.DI:
                    GameManager.instance.OpenConfirm("You're out of diamond.", null, false);
                    break;
                case CurrencyType.PD:
                    GameManager.instance.OpenConfirm("You're out of powder.", null, false);
                    break;
            }
        }

        UpdateCurrencyCount();
    }

    public void PurchaseTest()
    {
        PurchaseItem(ItemType.Test, "9990000", CurrencyType.DI, 10);
    }

    public void SellPlayerCard(string _instanceId, int _powder)
    {
        Debug.LogError(_instanceId);
        var request = new ConsumeItemRequest
        {
            ConsumeCount = 1,
            ItemInstanceId = _instanceId
        };

        PlayFabClientAPI.ConsumeItem(request, request => OnSellCardSuccess(request, _powder), OnSellCardError);
    }

    void OnSellCardSuccess(ConsumeItemResult result, int _powder)
    {
        AddPowder(_powder);
        Debug.Log($"{result.ItemInstanceId} sell Success");
        MainSceneManager.instance.cardDeckPanel.GetUserCardInventory();
    }

    void OnSellCardError(PlayFabError error)
    {
        Debug.LogError(error);
    }

    public void SendPlayerMessageToMine(string _title, string _detail, DateTime _time, int _diamond, List<PlayerItemBundle> _present, bool _empty = false)
    {
        if (!_empty)
            playerMessageList.Add(new PlayerMessage() { code = $"{Tools.GenerateRandomHex(8)}{DateTime.Now.ToString("yyyyMMddHHmmss")}", title = _title, sender = "Dummy games", detail = _detail, date = _time, diamond = _diamond, presentList = _present });
        string _json = JsonConvert.SerializeObject(playerMessageList);
        Debug.LogError(_json);
        UpdatePlayerMessageData(_json);
    }

    public void UpdatePlayerMessageData(string _data)
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { "PlayerMessage", _data }
            },
            Permission =  UserDataPermission.Private
        };

        PlayFabClientAPI.UpdateUserData(request, OnPlayerPlayerMessageChanged, OnPlayerStringValueChangeFailed);
    }

    private void OnPlayerPlayerMessageChanged(UpdateUserDataResult result)
    {
        Debug.LogError(result);

        MainSceneManager.instance.playerMessage.UpdateMessageList();
    }

    public void TestMessage()
    {
        UTime.GetUtcTimeAsync(OnTimeReceivedForSendMessage);
    }

    private void OnTimeReceivedForSendMessage(bool success, string error, DateTime time)
    {
        if (success)
        {
            Debug.Log(time);
            SendPlayerMessageToMine($"TEST{Random.Range(0, 99)}", Tools.GenerateRandomHex(500), time, 10, null);
        }
        else
        {
            Debug.LogError(error);
        }
    }

    public void ReadMessage(string _code)
    {
        for (int i = 0; i < playerMessageList.Count; i++)
        {
            if (playerMessageList[i].code == _code)
            {
                var message = playerMessageList[i];
                message.readed = true;
                if (message.diamond <= 0 && (message.presentList == null || message.presentList.Count <= 0))
                    message.received = true;
                playerMessageList[i] = message;
                break;
            }
        }

        SendPlayerMessageToMine(string.Empty, string.Empty, DateTime.Now, 0, null, true);
    }

    public void DeletePlayerMessage(List<string> _codeList)
    {
        foreach (var item in _codeList)
            playerMessageList.RemoveAll(x => x.code == item);

        string _json = JsonConvert.SerializeObject(playerMessageList);
        Debug.LogError(_json);
        UpdatePlayerMessageData(_json);
    }

    public PlayerMessage ReceivePresents(string _code)
    {
        var _message = new PlayerMessage();

        for (int i = 0; i < playerMessageList.Count; i++)
        {
            if (playerMessageList[i].code == _code)
            {
                _message = playerMessageList[i];

                if (_message.diamond > 0)
                    ReceiveMessageDiamond(_message, i);
                _message.readed = true;
                _message.received = true;
                playerMessageList[i] = _message;
            }
        }

        SendPlayerMessageToMine(string.Empty, string.Empty, DateTime.Now, 0, null, true);

        return _message;
    }

    private void ReceiveMessageDiamond(PlayerMessage _message, int _messageIndex)
    {
        var request = new AddUserVirtualCurrencyRequest
        {
            VirtualCurrency = CurrencyType.DI.ToString(),
            Amount = _message.diamond
        };

        PlayFabClientAPI.AddUserVirtualCurrency(request, result => OnAddCurrencySuccess(result, _message, _messageIndex), OnAddCurrencyFailed);
    }

    private void OnAddCurrencySuccess(ModifyUserVirtualCurrencyResult result, PlayerMessage message, int messageIndex)
    {
        Debug.Log("Add Diamond: " + result.Balance);

        foreach (var item in FindObjectsOfType<CurrencyText>(true))
            item.UpdateCount(result.Balance);

        message.readed = true;
        message.received = true;
        playerMessageList[messageIndex] = message;
    }

    public void UpdateMainShopContents()
    {
        GameManager.instance.OnOffLoadingPanel(true);

        MainSceneManager.instance.isCompleteLoadShop = false;

        var request = new ExecuteCloudScriptRequest
        {
            FunctionName = "GetStoreInfo",
            GeneratePlayStreamEvent = true,
        };

        PlayFabClientAPI.ExecuteCloudScript(request, OnGetStoreInfoSuccess, OnGetStoreInfoFailure);
    }

    private void OnGetStoreInfoSuccess(ExecuteCloudScriptResult result)
    {
        Debug.Log("Cloud Script executed successfully!");
        if (result.FunctionResult != null)
        {
            Debug.LogError(result.FunctionResult);
            List<MainShopComponents> _newComponentList = JsonConvert.DeserializeObject<List<MainShopComponents>>(result.FunctionResult.ToString());
            MainSceneManager.instance.mainShopPanel.shopComponentList = _newComponentList;
            MainSceneManager.instance.mainShopPanel.SpawnLine();

            StartCoroutine(OpenShopPanelCoroutine());
        }
    }

    IEnumerator OpenShopPanelCoroutine()
    {
        yield return new WaitUntil(() => MainSceneManager.instance.isCompleteLoadShop);

        GameManager.instance.OnOffLoadingPanel(false);
        MainSceneManager.instance.mainShopPanel.gameObject.SetActive(true);
    }

    private void OnGetStoreInfoFailure(PlayFabError error)
    {
        Debug.LogError("Cloud Script execution failed.");
        Debug.LogError(error.GenerateErrorReport());
    }

    public Sprite GetProfilelIconSpriteWithId(string _id)
    {
        foreach (var item in profileIconSpriteList)
        {
            if (item.name == _id)
                return item;
        }

        return null;
    }

    public Sprite GetProfileFrameSpriteWithId(string _id)
    {
        foreach (var item in profileFrameSpriteList)
        {
            if (item.name == _id)
                return item;
        }

        return null;
    }
}
