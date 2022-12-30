using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerLocomotor : MonoBehaviour, ILocomotor
{
    [SerializeField] private float moveSpeed = 5;
    private Vector3 targetVector;

    SmoothDamp.Vector3 currentVelocity;

    public Vector3 velocity { get { return currentVelocity.currentValue; } }
    public float speed { get { return currentVelocity.currentValue.magnitude; } }
    private void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        targetVector = new Vector3(movementVector.x, 0, movementVector.y);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetVelocity = moveSpeed * targetVector.normalized;
        currentVelocity.Step(targetVelocity, 1);
        transform.position += currentVelocity.currentValue * Time.deltaTime;
    }
}
