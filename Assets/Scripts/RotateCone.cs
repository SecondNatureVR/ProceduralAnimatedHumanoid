using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCone : MonoBehaviour
{
    [SerializeField] public Transform player;

    /// <summary> isolate the y-Component of a rotation </summary>
    private Quaternion yRotation(Quaternion q)
    {
        float theta = Mathf.Atan2(q.y, q.w);

        // quaternion representing rotation about the y axis
        return new Quaternion(0, Mathf.Sin(theta), 0, Mathf.Cos(theta));
    }

    private void Update()
    {
        Vector3 awayDir = transform.position - player.position;
        Quaternion awayRotation = Quaternion.LookRotation(awayDir);
        transform.rotation = yRotation(awayRotation);
    }
}
