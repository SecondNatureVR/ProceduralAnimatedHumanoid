using UnityEditor.Rendering;
using UnityEngine;

public class FaceAtIK : MonoBehaviour 
{
  [SerializeField] Transform target;
  [SerializeField] Transform sourceBone;
  [SerializeField] float speed = 1.0f;
  [SerializeField] float maxTurnAngle = 1.0f;
    
  
  void LateUpdate()
  {
      Quaternion currentLocalRotation = sourceBone.localRotation;
      sourceBone.localRotation = Quaternion.identity;

      Vector3 targetWorldLookDir = target.position - sourceBone.position;
      Vector3 targetLocalLookDir = sourceBone.InverseTransformDirection(targetWorldLookDir);

      // Apply angle limit
      targetLocalLookDir = Vector3.RotateTowards(
        Vector3.forward,
        targetLocalLookDir,
        Mathf.Deg2Rad * maxTurnAngle, // Note we multiply by Mathf.Deg2Rad here to convert degrees to radians
        0 // We don't care about the length here, so we leave it at zero
      );

      // Get the local rotation by using LookRotation on a local directional vector
      Quaternion targetLocalRotation = Quaternion.LookRotation(targetLocalLookDir, Vector3.up);

      sourceBone.localRotation = Quaternion.Slerp(
          currentLocalRotation,
          targetLocalRotation,
          1 - Mathf.Exp(-speed * Time.deltaTime)
      );
    
  }
}
