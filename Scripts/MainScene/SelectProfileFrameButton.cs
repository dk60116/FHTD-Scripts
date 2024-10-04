using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectProfileFrameButton : MonoBehaviour
{
    public string itemId;
    [SerializeField]
    private Button btn;

    public void SetFrame()
    {
        btn.image.sprite = SocialManager.instance.GetProfileFrameSpriteWithId(itemId);

        btn.onClick.AddListener(() => SocialManager.instance.ChangeProfileImage(itemId, "Frame"));
    }
}
