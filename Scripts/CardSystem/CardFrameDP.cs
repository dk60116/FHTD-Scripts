using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class CardFrameDP : MonoBehaviour
{
    public CardFrame frame;

    [SerializeField]
    private Toggle toggle;
    [SerializeField]
    private Image frontImage, backImage;
    [SerializeField]
    private TextMeshProUGUI frameName;

    public void UpdateFrame(string _id)
    {
        frame = CardManager.instance.GetCardFrameWithID(_id);

        frontImage.sprite = frame.frontSprite;
        backImage.sprite = frame.backSprite;
        frameName.text = frame.itemName;

        toggle.onValueChanged.RemoveAllListeners();
        toggle.onValueChanged.AddListener((bool _on) => SetFrame(_on));

        gameObject.SetActive(SocialManager.instance.playerInfo.haveCardFrame.Contains(_id));
    }

    private void SetFrame(bool _on)
    {
        if (_on)
        {
            var request = new ExecuteCloudScriptRequest
            {
                FunctionName = "ChangeCardFrame",
                FunctionParameter = new
                {
                    id = frame.itemId
                },
                GeneratePlayStreamEvent = true,
            };

            PlayFabClientAPI.ExecuteCloudScript(request, OnChangeFrameSuccess, OnChangeFrameFailure);
        }
    }

    private void OnChangeFrameSuccess(ExecuteCloudScriptResult result)
    {
        Debug.LogError(result.FunctionResult);
        SocialManager.instance.playerInfo.equipCardFrame = result.FunctionResult.ToString();
        MainSceneManager.instance.cardDeckPanel.UpdateCardDisplay();
    }

    private void OnChangeFrameFailure(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }

    public void OnToggle()
    {
        toggle.isOn = true;
    }
}
