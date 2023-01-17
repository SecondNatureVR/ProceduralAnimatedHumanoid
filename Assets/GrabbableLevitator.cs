using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine;

public class GrabbableLevitator : MonoBehaviour
{
    [SerializeField] public float height = 3;
    [SerializeField] public float duration = 4;
    [SerializeField] public float radius = 6;
    [SerializeField][Range(0, 10)] public float smoothing = 3f;

    void LateUpdate()
    {
        transform.position.Set(transform.position.x, 0, transform.position.z);
        foreach(var obj in FindObjectsOfType<OVRGrabbable>())
        {
            var ot = obj.transform;
            var rb = obj.GetComponent<Rigidbody>();
            bool isNearFloor = ot.position.y <= (1.05f * height);
            float dist = Vector3.ProjectOnPlane(transform.position - ot.position, Vector3.up).magnitude;
            bool isSleep = rb.velocity.magnitude < 3.0f;
            if (!obj.isGrabbed && isSleep && isNearFloor && dist < radius)
            {
                // Calculate what time in animation we are based on current height
                float currentDuration = Mathf.InverseLerp(0, height, Mathf.Max(0, ot.position.y)) * duration;
                float nextDuration = Mathf.Clamp(currentDuration + Time.deltaTime, currentDuration, duration);
                //  Calcuate next height value with smoothing
                float destHeight = Mathf.Lerp(0, height, Mathf.Pow(nextDuration / duration, Mathf.Lerp(0.88f, 1, smoothing/10)));
                ot.position = new Vector3(ot.position.x, destHeight, ot.position.z);
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }
}
