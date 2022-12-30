using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeckoTargeter : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] GeckoController_Full gecko;
    private Transform currentTarget;

    void Awake()
    {
        gecko.SetTarget(player);
    }

    public void SetTarget(Transform other)
    {
        currentTarget = other;
    }

    void Update()
    {
        if (currentTarget == null)
        {
            gecko.SetTarget(player);
        }
        else
        {
            gecko.SetTarget(currentTarget);
        }
    }

    public void clear()
    {
        currentTarget = null;
    }
}
