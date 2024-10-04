using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolableObject : MonoBehaviour
{
    private PoolingManager cPool;
    [SerializeField, ReadOnlyInspector]
    private int iIndex; 

    public void InitObject(PoolingManager _cPool, int _iIndex)
    {
        cPool = _cPool;
        iIndex = _iIndex;
    }

    public void ReturnObj()
    {
        transform.position = Vector3.zero;
        transform.localScale = Vector3.one;
        gameObject.SetActive(false);
        transform.SetParent(cPool.listHolder[iIndex]);
    }
}
