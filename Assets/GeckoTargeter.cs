using System.Collections; using System.Collections.Generic; using System.Linq;
using Oculus.Interaction.GrabAPI;
using Oculus.Interaction.HandGrab;
using Oculus.Platform.Models;
using UnityEngine;

public class GeckoTargeter : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GeckoController_Full gecko;
    [SerializeField] GameObject geckoMouth;
    private GameObject grabbedTarget;
    private GameObject currentTarget;
    private GeckoState currentState = GeckoState.RETURN;
    private OVRGrabber dummyOVRGrab;
    private bool isNearPlayer {
        get
        {
            return Vector3.Distance(transform.position, player.transform.position) <= gecko.defaultMaxDistToTarget;
        }
    }

    private enum GeckoState {
        FETCH, RETURN, IDLE
    }

    void Start()
    {
        dummyOVRGrab = new OVRGrabber();
        SetTarget(player);
    }

    public void SetTarget(GameObject other)
    {
        if (other == null)
            other = player;
        currentTarget = other;
        gecko.SetTarget(other.transform);
        if (other.CompareTag("Food"))
            gecko.SetMaxDistToTarget(0.5f);
        else if (other.CompareTag("Stick"))
            gecko.SetMaxDistToTarget(1f);
        else
            gecko.ResetMaxDistToTarget();
    }

    public bool hasTarget { get { return currentTarget != null; } }

    public void FindTarget()
    {
        var food = FindFood();
        var stick = FindStick();
        if (food != null || stick != null) {
            currentState = GeckoState.FETCH;
            SetTarget(food ?? stick);
            if (currentTarget == food)
            {
                ReleaseGrab();
            }
        } else {
            currentState = GeckoState.RETURN;
            SetTarget(player);
        }
    }

    private GameObject FindFood()
    {
        var food = GameObject.FindGameObjectsWithTag("Food");
        return food.Length > 0 ? food.OrderBy(f => Vector3.Distance(f.transform.position, transform.position)).First() : null;
    }
    private GameObject FindStick()
    {
        var stick = GameObject.FindGameObjectWithTag("Stick");
        if (stick != null && !stick.GetComponent<OVRGrabbable>().isGrabbed)
        {
            stick = Vector3.Distance(stick.transform.position, player.transform.position) > gecko.maxDistToTarget
                ? stick : null;
        }
        return stick;
    }

    void Update()
    {
        FindTarget();
        if (gecko.isInTargetRange)
        {
            switch (currentState) {
                case GeckoState.FETCH:
                    if (currentTarget.CompareTag("Food"))
                        GameObject.Destroy(currentTarget);
                    else if (!isNearPlayer)
                        GrabTarget();
                    clear();
                    break;
                case GeckoState.RETURN:
                    ReleaseGrab();
                    break;
                default:
                    break;
            }
        }
    }

    private void LateUpdate()
    {
        if (grabbedTarget != null)
        {
            Vector3 heldPos = geckoMouth.transform.position - geckoMouth.transform.forward * 0.5f;
            //grabbedTarget.transform.position = heldPos;
            var rb = grabbedTarget.GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    void GrabTarget()
    {
        var target = currentTarget.GetComponent<OVRGrabbable>();
        if (!target.isGrabbed)
        {
            grabbedTarget = currentTarget;
            target.GrabBegin(dummyOVRGrab, new Collider());
            Vector3 heldPos = geckoMouth.transform.position;
            grabbedTarget.transform.position = heldPos;
            grabbedTarget.transform.parent = geckoMouth.transform;
        }
    }

    void ReleaseGrab()
    {
        if (grabbedTarget != null) {
            var grabbable = grabbedTarget.GetComponent<OVRGrabbable>();
            dummyOVRGrab.ForceRelease(grabbable);
            grabbable.GetComponent<Rigidbody>().isKinematic = false;
            grabbedTarget.transform.parent = null;
            grabbedTarget = null;
        }
    }

    public void clear()
    {
        SetTarget(null);
        currentState = GeckoState.RETURN;
    }
}
