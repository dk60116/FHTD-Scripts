using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingProjectile : HeroEffect
{
    protected Transform tfTarget;
    protected PoolableObject cPoolable;
    protected float fSpeed;

    void Start()
    {
        cPoolable = GetComponent<PoolableObject>();
    }

    void Update()
    {
        if (tfTarget != null)
        {
            transform.position = Vector3.Lerp(transform.position, tfTarget.position + Vector3.up * 0.5f, fSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, tfTarget.position + Vector3.up * 0.5f) <= 0.1f)
                gameObject.GetComponent<PoolableObject>().ReturnObj();
        }
    }

    public void SetTarget(Transform _tfTarget, float _fSpeed)
    {
        tfTarget = _tfTarget;
        fSpeed = _fSpeed;
    }

    public float speed { get => fSpeed; set => fSpeed = value; }
}
