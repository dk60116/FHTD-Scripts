using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ScreenType { Normal, Wide, UltraWide, Quade}

public class PlayerCamera : MonoBehaviour
{
    [SerializeField]
    private float fHeight;
    [SerializeField, ReadOnlyInspector]
    private float fFinalHeight;
    private float fOffset;
    public float fSet;

    [SerializeField, ReadOnlyInspector]
    private ScreenType eScreenType;


    private void Reset()
    {
        fHeight = 3.8f;
        fSet = -0.415f;
        Init();
    }

    void Awake()
    {
        Init();
    }

    void FixedUpdate()
    {
        Init();
    }

    private void CheckScreenType()
    {
        if (Screen.width > Screen.height * 2.2f && Screen.width <= Screen.height * 2.5f)
            eScreenType = ScreenType.Wide;
        else if (Screen.width > Screen.height * 2.5f)
            eScreenType = ScreenType.UltraWide;
        else if (Mathf.Abs(Screen.width - Screen.height) < 200f)
            eScreenType = ScreenType.Quade;
        else
            eScreenType = ScreenType.Normal;

        switch (eScreenType)
        {
            case ScreenType.Normal:
                fFinalHeight = fHeight;
                break;
            case ScreenType.Wide:
                fFinalHeight = fHeight * 1.15f;
                break;
            case ScreenType.UltraWide:
                fFinalHeight = fHeight * (Screen.width / Screen.height) * 0.6f;
                break;
            case ScreenType.Quade:
                fFinalHeight = fHeight * 0.8f;
                break;
            default:
                break;
        }

        fFinalHeight *= FindObjectOfType<AStar>().sizeX;
    }

    private void Init()
    {
        CheckScreenType();

        fOffset = fFinalHeight * fSet * ((float)Screen.height / (float)Screen.width);
        transform.position = new Vector3(0, (float)Screen.height / (float)Screen.width * fFinalHeight, fOffset);
    }
}
