using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Animator))]
public class FoodButton : MonoBehaviour
{
    [SerializeField] FoodDispenser dispenser;
    private Animator animator;
    private bool triggered = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!triggered)
        {
            dispenser.Dispense();
            animator.Play("ButtonPress");
        }
    }
    private void OnTriggerExit()
    {
        triggered = false;
    }
}
