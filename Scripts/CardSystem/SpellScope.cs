using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellScope : MonoBehaviour
{
    private SpriteRenderer renSprite;
    private Material mMat;
    private Vector3 v3OrgSize;

    void Awake()
    {
        renSprite = GetComponent<SpriteRenderer>();
        mMat = renSprite.material;
        v3OrgSize = transform.localScale;
    }

    public void SetScale(float _fScale)
    {
        transform.localScale = v3OrgSize * _fScale;
    }

    public void SetColor(Color _cColor)
    {
        mMat.color = _cColor;
    }
}
