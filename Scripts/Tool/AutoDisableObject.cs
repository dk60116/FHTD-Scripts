using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDisableObject : MonoBehaviour
{
    [SerializeField]
    private float fTime = 1f;
    [SerializeField]
    private bool bDestroy = false;

    void OnEnable()
    {
        Invoke(nameof(Disable), fTime);
    }

    private void Disable()
    {
        gameObject.SetActive(false);

        if (bDestroy)
            Destroy(gameObject);
    }
}
