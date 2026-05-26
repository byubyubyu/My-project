using UnityEngine;

[CreateAssetMenu(fileName = "CityHallStats", menuName = "Stats/CityHallStats")]
public class CityHallStats : ScriptableObject, IStats
{
    [SerializeField] private float hp = 500f;
    [SerializeField] private float buildRequired = 100f;

    public float HP => hp;
    public float BuildRequired => buildRequired;

    public float GetStat(StatType type)
    {
        switch (type)
        {
            case StatType.HP: return hp;
            default: return 0f;
        }
    }
}