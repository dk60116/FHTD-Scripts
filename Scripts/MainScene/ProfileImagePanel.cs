using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class ProfileImagePanel : MonoBehaviour
{
    [SerializeField]
    private List<Toggle> toggles;
    [SerializeField]
    private List<ScrollRect> scrolls;
    [SerializeField]
    private List<SelectProfileIconButton> profileIconBtnList;
    [SerializeField]
    private List<SelectProfileFrameButton> profileFrameBtnList;
    [SerializeField]
    private Image selectedImage, selectedFrame;

    void Awake()
    {
        for (int i = 0; i < toggles.Count; i++)
        {
            int _i = i;
            toggles[i].onValueChanged.AddListener((bool _on) => ToggleHandler(_on, _i));
        }
    }

    void OnEnable()
    {
        foreach (var profile in profileIconBtnList)
        {
            profile.SetIcon();
            profile.gameObject.SetActive(SocialManager.instance.playerInfo.haveProfileIcon.Contains(profile.itemId));
        }

        foreach (var profile in profileFrameBtnList)
        {
            profile.SetFrame();
            profile.gameObject.SetActive(SocialManager.instance.playerInfo.haveProfileFrame.Contains(profile.itemId));
        }

        toggles[0].isOn = true;

        Invoke(nameof(EnableHandler), Time.deltaTime);
    }

    private void EnableHandler()
    {
        toggles[0].isOn = true;
    }

    private void ToggleHandler(bool _on, int _index)
    {
        if (_on)
        {
            foreach (var item in scrolls)
                item.gameObject.SetActive(false);

            scrolls[_index].gameObject.SetActive(true);
        }
    }

    [ContextMenu("Init")]
    private void Init()
    {
        profileIconBtnList = new List<SelectProfileIconButton>();
        profileFrameBtnList = new List<SelectProfileFrameButton>();

        for (int i = 0; i < scrolls[0].content.childCount; i++)
            profileIconBtnList.Add(scrolls[0].content.GetChild(i).GetComponent<SelectProfileIconButton>());
        for (int i = 0; i < scrolls[1].content.childCount; i++)
            profileFrameBtnList.Add(scrolls[1].content.GetChild(i).GetComponent<SelectProfileFrameButton>());
    }

    public void SetCurrentImage(string _iconId, string _frameId)
    {
        selectedImage.sprite = SocialManager.instance.GetProfilelIconSpriteWithId(_iconId);
        selectedFrame.sprite = SocialManager.instance.GetProfileFrameSpriteWithId(_frameId);
    }
}
