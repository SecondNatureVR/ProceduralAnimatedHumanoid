using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeckoModel : MonoBehaviour
{
    [SerializeField] public GeckoStats stats;

    // TODO: GameObject -> useful notion of edible gecko food
    public void Eat(GameObject g)
    {
        stats.foodEaten += 1;
    }
}
