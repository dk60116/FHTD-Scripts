using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class SocialPanel : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI masterIdText, googleNameText;
    [SerializeField]
    private RawImage profileImage;

    public void UpdateProfile(string _masterId, string _googleName, Texture2D _googleImage)
    {
        masterIdText.text = _masterId;
        googleNameText.text = _googleName;
        profileImage.texture = _googleImage;
    }
}
