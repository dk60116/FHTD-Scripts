using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserFramePaenl : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI levelText, nicknameText, expText;
    [SerializeField]
    private Image profileImage, frameImage, expFill;

    public void UpdateProfile(int _level, int _exp, string _nickname, string _profileImage, string _frameImage)
    {
        levelText.text = _level.ToString();
        nicknameText.text = _nickname;
        expText.text = $"{_exp} / {100}";
        expFill.fillAmount = _exp / 100f;
        profileImage.sprite = SocialManager.instance.GetProfilelIconSpriteWithId(_profileImage);
        frameImage.sprite = SocialManager.instance.GetProfileFrameSpriteWithId(_frameImage);
        MainSceneManager.instance.profilePanel.SetCurrentImage(_profileImage, _frameImage);
    }

    public void Reset()
    {
        MainSceneManager.instance.profilePanel.SetCurrentImage("310100", "320100");
    }
}
