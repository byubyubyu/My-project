using UnityEngine;

public class StatsHolder : MonoBehaviour
{
    [SerializeField] private ScriptableObject stats;
    public IStats Stats => stats as IStats;
}