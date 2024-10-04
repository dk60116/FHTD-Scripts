using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectProfileIconButton : MonoBehaviour
{
    public string itemId;
    [SerializeField]
    private Button btn;

    public void SetIcon()
    {
        Debug.LogError(btn);
        btn.image.sprite = SocialManager.instance.GetProfilelIconSpriteWithId(itemId);

        btn.onClick.AddListener(() => SocialManager.instance.ChangeProfileImage(itemId, "Icon"));
    }
}
