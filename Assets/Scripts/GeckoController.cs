using System;
using System.Collections;
using UnityEngine;
using static Utils;

public class GeckoController : MonoBehaviour 
{
    // The target we are going to track
    [SerializeField] Transform target;
    // A reference to the gecko's neck
    [SerializeField] Transform headBone;
    [SerializeField] float speed = 1.0f;
    [SerializeField] float maxTurnAngle = 60.0f;

    // eyes
    [SerializeField] Transform leftEyeBone;
    [SerializeField] Transform rightEyeBone;

    [SerializeField] float eyeTrackingSpeed = 1.0f;
    [SerializeField] float leftEyeMaxYRotation = 45f;
    [SerializeField] float leftEyeMinYRotation = -45f;
    [SerializeField] float rightEyeMaxYRotation = 45f;
    [SerializeField] float rightEyeMinYRotation = -45f;

    // Leg Stepping
    [SerializeField] LegStepper frontLeftLegStepper;
    [SerializeField] LegStepper frontRightLegStepper;
    [SerializeField] LegStepper backLeftLegStepper;
    [SerializeField] LegStepper backRightLegStepper;

    // How fast we can turn and move full throttle
    [SerializeField] float turnSpeed;
    [SerializeField] float moveSpeed;
    // How fast we will reach the above speeds
    [SerializeField] float turnAcceleration;
    [SerializeField] float moveAcceleration;
    // Try to stay in this range from the target
    [SerializeField] float minDistToTarget;
    [SerializeField] float maxDistToTarget;
    // If we are above this angle from the target, start turning
    [SerializeField] float maxAngToTarget;

    // World space velocity
    Vector3 currentVelocity;
    // We are only doing a rotation around the up axis, so we only use a float here
    float currentAngularVelocity;

    private void Awake()
    {
        StartCoroutine(LegUpdateCoroutine());
    }

    IEnumerator LegUpdateCoroutine()
    {
      // Run continuously
      while (true)
      {
        // Try moving one diagonal pair of legs
        do
        {
          frontLeftLegStepper.TryMove();
          backRightLegStepper.TryMove();
          // Wait a frame
          yield return null;
          
          // Stay in this loop while either leg is moving.
          // If only one leg in the pair is moving, the calls to TryMove() will let
          // the other leg move if it wants to.
        } while (backRightLegStepper.Moving || frontLeftLegStepper.Moving);

        // Do the same thing for the other diagonal pair
        do
        {
          frontRightLegStepper.TryMove();
          backLeftLegStepper.TryMove();
          yield return null;
        } while (backLeftLegStepper.Moving || frontRightLegStepper.Moving);
      }
    }

    void RootMotionUpdate()
    {
        // Get the direction toward our target
        Vector3 towardTarget = target.position - transform.position;
        // Vector toward target on the local XZ plane
        Vector3 towardTargetProjected = Vector3.ProjectOnPlane(towardTarget, transform.up);
        // Get the angle from the gecko's forward direction to the direction toward toward our target
        // Here we get the signed angle around the up vector so we know which direction to turn in
        float angToTarget = Vector3.SignedAngle(transform.forward, towardTargetProjected, transform.up);

        float targetAngularVelocity = 0;

        // If we are within the max angle (i.e. approximately facing the target)
        // leave the target angular velocity at zero
        if (Mathf.Abs(angToTarget) > maxAngToTarget)
        {
            // Angles in Unity are clockwise, so a positive angle here means to our right
            if (angToTarget > 0)
            {
                targetAngularVelocity = turnSpeed;
            }
            // Invert angular speed if target is to our left
            else
            {
                targetAngularVelocity = -turnSpeed;
            }
        }


        //// To be placed in the RootMotionUpdate method below the rotation code ////

        Vector3 targetVelocity = Vector3.zero;

        // Don't move if we're facing away from the target, just rotate in place
        if (Mathf.Abs(angToTarget) < 90)
        {
          float distToTarget = Vector3PlaneDistance(headBone.position, target.position, Vector3.up);
          // If we're too far away, approach the target
          if (distToTarget > maxDistToTarget)
          {
            targetVelocity = moveSpeed * towardTargetProjected.normalized;
          }
          // If we're too close, reverse the direction and move away
          else if (distToTarget < minDistToTarget)
          {
            targetVelocity = moveSpeed * -towardTargetProjected.normalized * 5;
          }
        }

        currentVelocity = Vector3.Lerp(
          currentVelocity,
          targetVelocity,
          1 - Mathf.Exp(-moveAcceleration * Time.deltaTime)
        );

        // Apply the velocity
        transform.position += currentVelocity * Time.deltaTime;

        // Use our smoothing function to gradually change the velocity
        currentAngularVelocity = Mathf.Lerp(
          currentAngularVelocity,
          targetAngularVelocity,
          1 - Mathf.Exp(-turnAcceleration * Time.deltaTime)
        );

        // Rotate the transform around the Y axis in world space, 
        // making sure to multiply by delta time to get a consistent angular velocity
        transform.Rotate(0, Time.deltaTime * currentAngularVelocity, 0, Space.World);
    }


    // We will put all our animation code in LateUpdate.
    // This allows other systems to update the environment first, 
    // allowing the animation system to adapt to it before the frame is drawn.
    void LateUpdate()
    {
        RootMotionUpdate();
        HeadTrackingUpdate();
        EyeTrackingUpdate();
    }

    void EyeTrackingUpdate()
    {
        // Note: We use head position here just because the gecko doesn't
        // look so great when cross eyed. To make it relative to the eye 
        // itself, subtract the eye's position instead of the head's.
        Quaternion targetEyeRotation = Quaternion.LookRotation(
          target.position - headBone.position, // toward target
          transform.up
        );

        leftEyeBone.rotation = Quaternion.Slerp(
          leftEyeBone.rotation,
          targetEyeRotation,
          1 - Mathf.Exp(-eyeTrackingSpeed * Time.deltaTime)
        );

        rightEyeBone.rotation = Quaternion.Slerp(
          rightEyeBone.rotation,
          targetEyeRotation,
          1 - Mathf.Exp(-eyeTrackingSpeed * Time.deltaTime)
        );
        float leftEyeCurrentYRotation = leftEyeBone.localEulerAngles.y;
        float rightEyeCurrentYRotation = rightEyeBone.localEulerAngles.y;

        // Move the rotation to a -180 ~ 180 range
        if (leftEyeCurrentYRotation > 180)
        {
            leftEyeCurrentYRotation -= 360;
        }
        if (rightEyeCurrentYRotation > 180) 
        {
            rightEyeCurrentYRotation -= 360;
        }

        // Clamp the Y axis rotation
        float leftEyeClampedYRotation = Mathf.Clamp(
            leftEyeCurrentYRotation,
            leftEyeMinYRotation,
            leftEyeMaxYRotation
        );
        float rightEyeClampedYRotation = Mathf.Clamp(
            rightEyeCurrentYRotation,
            rightEyeMinYRotation,
            rightEyeMaxYRotation
        );

        // Apply the clamped Y rotation without changing the X and Z rotations
        leftEyeBone.localEulerAngles = new Vector3(
            leftEyeBone.localEulerAngles.x,
            leftEyeClampedYRotation,
            leftEyeBone.localEulerAngles.z
        );
        rightEyeBone.localEulerAngles = new Vector3(
            rightEyeBone.localEulerAngles.x,
            rightEyeClampedYRotation,
            rightEyeBone.localEulerAngles.z
        );
    }

    void HeadTrackingUpdate() {
      // Store the current head rotation since we will be resetting it
      Quaternion currentLocalRotation = headBone.localRotation;
      // Reset the head rotation so our world to local space transformation will use the head's zero rotation. 
      // Note: Quaternion.Identity is the quaternion equivalent of "zero"
      headBone.localRotation = Quaternion.identity;

      Vector3 targetWorldLookDir = target.position - headBone.position;
      Vector3 targetLocalLookDir = headBone.InverseTransformDirection(targetWorldLookDir);

      // Apply angle limit
      targetLocalLookDir = Vector3.RotateTowards(
        Vector3.forward,
        targetLocalLookDir,
        Mathf.Deg2Rad * maxTurnAngle, // Note we multiply by Mathf.Deg2Rad here to convert degrees to radians
        0 // We don't care about the length here, so we leave it at zero
      );

      // Get the local rotation by using LookRotation on a local directional vector
      Quaternion targetLocalRotation = Quaternion.LookRotation(targetLocalLookDir, Vector3.up);

      headBone.localRotation = Quaternion.Slerp(
          currentLocalRotation,
          targetLocalRotation,
          1 - Mathf.Exp(-speed * Time.deltaTime)
      );
    
  }
}
