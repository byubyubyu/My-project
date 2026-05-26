using System;
using UnityEngine;

public class MinionBrain : MonoBehaviour
{
    private IMinionAction[] actions;
    private MinionMove mover;
    private int activeActionCount = 0;

    void Start()
    {
        mover = GetComponent<MinionMove>();
        actions = GetComponents<IMinionAction>();

        foreach (IMinionAction action in actions)
        {
            action.OnActionStart += OnActionStart;
            action.OnActionEnd += OnActionEnd;
            action.OnChaseTarget += OnChaseTarget;
        }
    }

    void OnActionStart()
    {
        activeActionCount++;
        IMinionAction highest = GetHighestPriorityAction();
        if (highest != null && highest.StopsMovement)
        {
            mover.StopMove();
        }
    }

    void OnActionEnd()
    {
        activeActionCount--;
        if (activeActionCount <= 0)
        {
            activeActionCount = 0;
            mover.ResumeMove();
        }
    }

    void OnChaseTarget(Vector3 targetPos)
    {
        UnityEngine.Debug.Log("OnChaseTarget: " + targetPos);
        mover.ChaseTo(targetPos);
    }

    IMinionAction GetHighestPriorityAction()
    {
        IMinionAction highest = null;
        int highestPriority = int.MaxValue;

        foreach (IMinionAction action in actions)
        {
            if (action.PriorityLevel < highestPriority)
            {
                highestPriority = action.PriorityLevel;
                highest = action;
            }
        }
        return highest;
    }

    void OnDestroy()
    {
        foreach (IMinionAction action in actions)
        {
            action.OnActionStart -= OnActionStart;
            action.OnActionEnd -= OnActionEnd;
            action.OnChaseTarget -= OnChaseTarget;
        }
    }
}