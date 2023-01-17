using System.Collections; using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GeckoTargeter : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GeckoController_Full gecko;
    private GameObject currentTarget;
    private GeckoState currentState = GeckoState.RETURN;

    private enum GeckoState {
        FETCH, RETURN, IDLE
    }

    void Awake()
    {
        SetTarget(player);
    }

    public void SetTarget(GameObject other)
    {
        if (other == null)
            other = player;
        currentTarget = other;
        gecko.SetTarget(other.transform);
        if (other.CompareTag("Food"))
            gecko.SetMaxDistToTarget(1f);
        else
            gecko.ResetMaxDistToTarget();
    }

    public bool hasTarget { get { return currentTarget != null; } }

    public void FindTarget()
    {
        var food = FindFood();
        if (food != null) {
            currentState = GeckoState.FETCH;
            SetTarget(food);
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

    void Update()
    {
        FindTarget();
        if (gecko.isInTargetRange)
        {
            switch (currentState) {
                case GeckoState.FETCH:
                    GameObject.Destroy(currentTarget);
                    clear();
                    break;
                case GeckoState.RETURN:
                    break;
                default:
                    break;
            }
        }
    }

    public void clear()
    {
        SetTarget(null);
        currentState = GeckoState.RETURN;
    }
}
