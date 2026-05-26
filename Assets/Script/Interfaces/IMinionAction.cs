using System;
using UnityEngine;

public interface IMinionAction : IPriority
{
    event Action OnActionStart;
    event Action OnActionEnd;
    event Action<Vector3> OnChaseTarget;
    bool StopsMovement { get; }
}