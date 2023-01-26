using System;
using UnityEngine;

public interface ILocomotor 
{
    public Vector3 velocity { get; }
    public float speed { get; }
}
