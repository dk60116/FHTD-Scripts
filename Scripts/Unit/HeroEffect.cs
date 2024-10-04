using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HeroEffect : MonoBehaviour
{
    public PoolableObject cPoolObj;

    void Awake()
    {
        cPoolObj = GetComponent<PoolableObject>();
    }
}
