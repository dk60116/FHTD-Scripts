using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class CramisLaserComponent : MonoBehaviour
{
    [SerializeField, ReadOnlyInspector]
    private Transform tfSylinder;

    private Vector3 v3StartPoint, v3EndPoint;

    void Update()
    {
        v3StartPoint = transform.parent.localPosition;
    }

    [ContextMenu("Init")]
    private void Init()
    {
        tfSylinder = transform.GetChild(0);
    }
}

