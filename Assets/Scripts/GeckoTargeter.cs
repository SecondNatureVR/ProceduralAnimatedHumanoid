using UnityEngine;
using System.Linq;

public class GeckoTargeter : MonoBehaviour
{
    public GameObject player;
    // TODO: traverse children for reference
    public GameObject geckoMouth;
    private GeckoController_Full gecko;
    private GeckoModel geckoModel;
    private OVRGrabber grabber;
    private GameObject grabbedTarget;
    private GameObject currentTarget;
    private GeckoState currentState = GeckoState.RETURN;
    // TODO: refactor mouth/eat boundaries and max distance logic
    private bool IsNearPlayer {
        get
        {
            return Vector3.Distance(transform.position, player.transform.position) <= gecko.defaultMaxDistToTarget;
        }
    }

    public enum GeckoState {
        FETCH, RETURN, IDLE
    }

    void Start()
    {
        gecko = GetComponent<GeckoController_Full>();
        geckoModel = GetComponent<GeckoModel>();
        grabber = geckoMouth.GetComponentInChildren<OVRGrabber>();
        SetTarget(player);
    }

    public void SetTarget(GameObject other)
    {
        if (other == null)
            other = player;
        currentTarget = other;
        gecko.SetTarget(other.transform);
        // TODO: refactor mouth/eat boundaries and max distance logic
        if (other.CompareTag("Food"))
            gecko.SetMaxDistToTarget(0.5f);
        else if (other.CompareTag("Stick"))
            gecko.SetMaxDistToTarget(1f);
        else
            gecko.ResetMaxDistToTarget();
    }

    public bool HasTarget { get { return currentTarget != null; } }

    public void FindTarget()
    {
        var food = FindFood();
        var stick = FindStick();
        if (food != null || stick != null) {
            currentState = GeckoState.FETCH;
            SetTarget(food == null ? stick : food);
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
                    if (MouthHasFood()) {
                        geckoModel.Eat(currentTarget);
                        GameObject.Destroy(currentTarget);
                    }
                    else if (!IsNearPlayer)
                        GrabTarget();
                    Clear();
                    break;
                case GeckoState.RETURN:
                    ReleaseGrab();
                    break;
                default:
                    break;
            }
        }
    }
    // TODO: refactor mouth/eat boundaries and max distance logic
    public bool MouthHasFood()
    {
        return currentTarget.CompareTag("Food")
        && Vector3.Distance(currentTarget.transform.position, geckoMouth.transform.position) < 4f;
    }

    void GrabTarget()
    {
        var target = currentTarget.GetComponent<OVRGrabbable>();
        if (!target.isGrabbed)
        {
            grabbedTarget = currentTarget;
            grabber.ForceGrab();
        }
    }

    void ReleaseGrab()
    {
        if (grabbedTarget != null) {
            var grabbable = grabbedTarget.GetComponent<OVRGrabbable>();
            grabber.ForceRelease(grabbable);
            grabbable.GetComponent<Rigidbody>().isKinematic = false;
            grabbedTarget.transform.parent = null;
            grabbedTarget = null;
        }
    }

    public void Clear()
    {
        SetTarget(null);
        currentState = GeckoState.RETURN;
    }
}
