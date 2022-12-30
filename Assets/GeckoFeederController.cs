using System.Collections;
using System.Collections.Generic;
using OVRTouchSample;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class GeckoFeederController : MonoBehaviour
{
    [SerializeField] GeckoTargeter geckoTarget;
    [SerializeField] Transform rightHand;
    [SerializeField] Transform leftHand;
    private Transform pointTarget;

    private void Update()
    {
        Debug.DrawRay(rightHand.position, rightHand.TransformDirection(Vector3.forward), Color.yellow);
        Debug.DrawRay(leftHand.position, leftHand.TransformDirection(Vector3.forward), Color.yellow);
    }

    public void PointToTarget(InputAction.CallbackContext context)
    {
        Debug.Log("POINTING POINTING POINTING");
        RaycastHit hit;
        if (Physics.Raycast(rightHand.position, rightHand.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
        {
            Debug.Log("hit!");
            if (pointTarget == null)
            {
                pointTarget = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
            }
            pointTarget.position = hit.transform.position;
            geckoTarget.SetTarget(pointTarget);
        };
    }
}
