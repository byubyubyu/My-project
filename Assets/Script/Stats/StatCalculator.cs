using UnityEngine;

public class StatCalculator : MonoBehaviour
{
    private StatsHolder holder;
    private StatModifier modifier;

    void Awake()
    {
        holder = GetComponent<StatsHolder>();
        modifier = GetComponent<StatModifier>();
    }

    public float GetStat(StatType type)
    {
        float baseValue = holder != null && holder.Stats != null
            ? holder.Stats.GetStat(type) : 0f;
        float bonus = modifier != null ? modifier.GetTotal(type) : 0f;
        return baseValue + bonus;
    }
}