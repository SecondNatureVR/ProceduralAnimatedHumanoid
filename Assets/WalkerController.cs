using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WalkerController : MonoBehaviour
{
    [SerializeField] ControllerLocomotor locomotor;
    [SerializeField] Animator animator;

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("isWalking", locomotor.speed > 0);
    }
}
