using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Networking;

public class MainSceneManager : MonoBehaviour
{
    static public MainSceneManager instance;

    [SerializeField]
    private GameObject blackPanel;
    public RectTransform loginPanel;
    public LoginSystem loginSys;
    public SocialPanel socialPanel;
    public ProfileImagePanel profilePanel;
    public CardDeckPanel cardDeckPanel;
    public UserFramePaenl userFrame;
    public PlayerMessageSystem playerMessage;
    public RectTransform nicknamePanel;
    public MainShopPanel mainShopPanel;
    public OpenCardSystem openCardSystem;
    [SerializeField, ReadOnlyInspector]
    public bool isCompleteLoadShop;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        profilePanel.gameObject.SetActive(false);
        cardDeckPanel.gameObject.SetActive(false);
        blackPanel.gameObject.SetActive(true);
        loginPanel.gameObject.SetActive(true);
        playerMessage.gameObject.SetActive(false);
    }

    [ContextMenu("Init")]
    public void Init()
    {
        loginSys = FindObjectOfType<LoginSystem>(true);
        socialPanel = FindObjectOfType<SocialPanel>(true);
        profilePanel = FindObjectOfType<ProfileImagePanel>(true);
        cardDeckPanel = FindObjectOfType<CardDeckPanel>(true);
        userFrame = FindObjectOfType<UserFramePaenl>(true);
        playerMessage = FindObjectOfType<PlayerMessageSystem>(true);
        mainShopPanel = FindObjectOfType<MainShopPanel>(true);
        openCardSystem = FindObjectOfType<OpenCardSystem>(true);
    }

    public void OnOffBlackPanel(bool _on)
    {
        blackPanel.SetActive(_on);
    }

    public void OpenShop()
    {
        SocialManager.instance.UpdateMainShopContents();
    }

    public void ShopImageDownload(string _url, RawImage _image)
    {
        StartCoroutine(DownloadImage(_url, _image));
    }

    IEnumerator DownloadImage(string _url, RawImage _image)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(_url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            _image.texture = texture;
            isCompleteLoadShop = true;
        }
        else
            Debug.LogError("Failed to download image: " + request.error);
    }
}
