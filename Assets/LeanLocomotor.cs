using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeanLocomotor : MonoBehaviour
{
    [SerializeField] private Transform home;
    [SerializeField] private Transform target;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float deadZone = 0.5f;

    SmoothDamp.Vector3 currentVelocity;

    public Vector3 velocity { get { return currentVelocity.currentValue; } }
    public float speed { get { return currentVelocity.currentValue.magnitude; } }

    // Update is called once per frame
    void Update()
    {
        Vector3 leanTargetVector = target.position - home.position;
        if (leanTargetVector.magnitude > deadZone)
        {
            Vector3 targetVelocity = moveSpeed * Vector3.ProjectOnPlane(leanTargetVector, Vector3.up).normalized;
            currentVelocity.Step(targetVelocity, 1);
            transform.position += currentVelocity.currentValue * Time.deltaTime;
        }
    }
}
