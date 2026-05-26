using UnityEngine;

[CreateAssetMenu(fileName = "MinionStats", menuName = "Minion/Stats")]
public class MinionStats : ScriptableObject, IStats
{
    [SerializeField] private string type = "Minion";
    [SerializeField] private float maxHP = 100f;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float attackInterval = 1f;
    [SerializeField] private float repairAmount = 0f;
    [SerializeField] private float buildPower = 0f;  // 0なら建設不可

    public float GetStat(StatType type)
    {
        switch (type)
        {
            case StatType.HP: return maxHP;
            case StatType.Speed: return speed;
            case StatType.AttackDamage: return attackDamage;
            case StatType.AttackRange: return attackRange;
            case StatType.DetectionRange: return detectionRange;
            case StatType.AttackInterval: return attackInterval;
            case StatType.RepairAmount: return repairAmount;
            case StatType.BuildPower: return buildPower;
            default: return 0f;
        }
    }
}