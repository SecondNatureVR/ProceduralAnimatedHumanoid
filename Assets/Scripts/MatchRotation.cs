using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchRotation : MonoBehaviour
{
    [SerializeField] private Transform source;
    [SerializeField] private Transform target;

    // Update is called once per frame
    void LateUpdate()
    {
        source.rotation = target.rotation; 
    }
}
