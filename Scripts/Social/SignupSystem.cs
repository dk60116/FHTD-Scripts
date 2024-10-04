using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SignupSystem : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField emailInput, pwInput, pwCInput;
    [SerializeField]
    private GameObject checkText, checkMark, pwCheckMark;
    [SerializeField]
    private Button emailCheckBtn;
    [SerializeField, ReadOnlyInspector]
    private bool isCheckedEmail, isCheckPassword, isCheckPasswordC;

    void Awake()
    {
        emailInput.onValueChanged.AddListener((string _text) => ChangeEmailText(_text));
        pwInput.onValueChanged.AddListener((string _text) => ChangePWText(_text));
        pwCInput.onValueChanged.AddListener((string _text) => ChangePWCText(_text));
    }

    void OnEnable()
    {
        isCheckedEmail = false;
        isCheckPassword = false;
        isCheckPasswordC = false;
        emailInput.text = string.Empty;
        pwInput.text = string.Empty;
        pwCInput.text = string.Empty;
        checkText.SetActive(true);
        checkMark.SetActive(false);
        pwCheckMark.SetActive(false);
        emailCheckBtn.interactable = false;
    }

    private void ChangeEmailText(string _text)
    {
        emailCheckBtn.interactable = _text.Length > 5;
    }

    private void ChangePWText(string _text)
    {
        bool _value;

        _value = _text.Length > 6 && !_text.Contains("%");

        isCheckPassword = _value;

        if (_value && pwCInput.text == _text)
        {
            isCheckPasswordC = _value;
            pwCheckMark.SetActive(true);
        }
        else
            pwCheckMark.SetActive(false);
    }

    private void ChangePWCText(string _text)
    {
        bool _value;

        _value = isCheckPassword && pwInput.text == _text;

        isCheckPasswordC = _value;
        pwCheckMark.SetActive(_value);
    }

    public void CheckDuplicationID()
    {
        var request = new LoginWithEmailAddressRequest { Email = emailInput.text, Password = $"!@#$%^{Random.Range(0, 999)}" };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
    }

    void OnLoginSuccess(LoginResult result)
    {
        print("로그인 성공");
    }

    void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError(error.Error);

        switch (error.Error)
        {
            case PlayFabErrorCode.InvalidParams:
                isCheckedEmail = false;
                GameManager.instance.OpenConfirm("Not in the correct format.", null, false);
                break;
            case PlayFabErrorCode.AccountNotFound:
                Debug.LogError("사용 가능한 이메일");
                isCheckedEmail = true;
                checkText.SetActive(false);
                checkMark.SetActive(true);
                break;
            case PlayFabErrorCode.InvalidEmailOrPassword:
                isCheckedEmail = false;
                GameManager.instance.OpenConfirm("An email address that is already in use.", null, false);
                break;
        }
    }

    public void RegisterBtn()
    {
        if (!isCheckedEmail)
        {
            GameManager.instance.OpenConfirm("Please Check your email", null, false);
            return;
        }

        if (!isCheckPassword || !isCheckPasswordC)
        {
            GameManager.instance.OpenConfirm("Please Confirm passwords", null, false);
            return;
        }

        var request = new RegisterPlayFabUserRequest { Email = emailInput.text, Password = pwInput.text, Username = $"USER{GenerateRandomHex(8)}" };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterFailure);
    }

    void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        Debug.LogError("회원가입 성공");
        GameManager.instance.OpenConfirm("Success signup! ", null, false);
    }

    void OnRegisterFailure(PlayFabError error)
    {
        Debug.LogError("회원가입 실패");
        Debug.LogError(error.Error);

        switch (error.Error)
        {
            case PlayFabErrorCode.UsernameNotAvailable:
                RegisterBtn();
                break;
        }
    }

    string GenerateRandomHex(int length)
    {
        string hex = "";
        for (int i = 0; i < length; i++)
        {
            hex += "0123456789ABCDEF"[UnityEngine.Random.Range(0, 16)];
        }
        return hex;
    }
}
