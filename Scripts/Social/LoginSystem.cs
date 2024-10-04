using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoginSystem : MonoBehaviour
{
    [SerializeField]
    private RectTransform googleLoginPanel, customLoginPanel;
    [SerializeField]
    private TMP_InputField emailInput, pwInput, nickNameInut;

    void Start()
    {
#if UNITY_EDITOR
        googleLoginPanel.gameObject.SetActive(false);
        customLoginPanel.gameObject.SetActive(true);
#else
        googleLoginPanel.gameObject.SetActive(true);
        customLoginPanel.gameObject.SetActive(false);
#endif

        if (PlayerPrefs.HasKey("PlayFabSessionToken") && PlayerPrefs.GetString("PlayFabSessionToken") != string.Empty)
        {
            emailInput.text = PlayerPrefs.GetString("PlayFabSessionToken");
            pwInput.text = PlayerPrefs.GetString("PlayerPW");
            SignIn(PlayerPrefs.GetString("PlayFabSessionToken"), PlayerPrefs.GetString("PlayerPW"));
        }
        else
            MainSceneManager.instance.loginSys.gameObject.SetActive(true);
    }

    public void SignInBtn()
    {
        SignIn(emailInput.text, pwInput.text);
    }

    private void SignIn(string _email, string _password)
    {
        GameManager.instance.OnOffLoadingPanel(true);
        var request = new LoginWithEmailAddressRequest { Email = _email, Password = _password, InfoRequestParameters = new GetPlayerCombinedInfoRequestParams() { GetPlayerProfile = true } };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
    }

    public void SignOutBtn()
    {
        pwInput.text = string.Empty;
    }

    void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Login Success");
        SocialManager.instance.logText.text = "로그인 성공";

        if (result.InfoResultPayload.PlayerProfile.DisplayName != null)
        {
            string _displayName = result.InfoResultPayload.PlayerProfile.DisplayName;
            Debug.Log("User Display Name: " + _displayName);
            SocialManager.instance.CheckAndAssignNumberToNewUser();

            SocialManager.instance.GetPlayerInfo(result.PlayFabId, _displayName);
            SocialManager.instance.UpdateCurrencyCount();
        }
        else
        {
            Debug.Log("No DisplayName available");
            GameManager.instance.OnOffLoadingPanel(false);
            MainSceneManager.instance.nicknamePanel.gameObject.SetActive(true);
        }

        PlayerPrefs.SetString("PlayFabSessionToken", emailInput.text);
        PlayerPrefs.SetString("PlayerPW", pwInput.text);
    }

    public void CreateNickname()
    {
        SocialManager.instance.UpdateNickname(nickNameInut.text);
    }

    void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError("Login Failed");
        SocialManager.instance.logText.text = "로그인 실패";
        Debug.LogError(error.Error);

        switch (error.Error)
        {
            case PlayFabErrorCode.InvalidParams:
                GameManager.instance.OpenConfirm("Confirm your email or password", null, false);
                break;
            case PlayFabErrorCode.AccountNotFound:
                GameManager.instance.OpenConfirm("Account not found", null, false);
                break;
        }

        PlayerPrefs.DeleteKey("PlayFabSessionToken");

        GameManager.instance.OnOffLoadingPanel(false);
    }
}
