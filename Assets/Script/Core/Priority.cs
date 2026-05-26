using UnityEngine;

public class Priority : MonoBehaviour, IPriority
{
    [SerializeField] private int priorityValue = 0;
    public int PriorityLevel => priorityValue;
}