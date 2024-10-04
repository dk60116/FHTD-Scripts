using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EasyBackground : MonoBehaviour
{
    [SerializeField]
    private Button background;
    [SerializeField]
    private bool autoEsc;

    void OnEnable()
    {
        background.gameObject.SetActive(true);
        background.onClick.RemoveAllListeners();

        if (autoEsc)
            background.onClick.AddListener(()=> gameObject.SetActive(false));
    }

    void OnDisable()
    {
        background.gameObject.SetActive(false);
    }
}
