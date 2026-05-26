using UnityEngine;

[CreateAssetMenu(fileName = "BaseStats", menuName = "Base/Stats")]
public class BaseStats : ScriptableObject, IStats
{
    [Header("ステータス")]
    [SerializeField] private float maxHP = 500f;
    [SerializeField] private float speed = 0f;
    [SerializeField] private float attackDamage = 0f;
    [SerializeField] private float attackRange = 0f;
    [SerializeField] private float detectionRange = 0f;
    [SerializeField] private float attackInterval = 0f;
    [SerializeField] private float repairAmount = 0f;

    [Header("モデル")]
    [SerializeField] private GameObject activeModel;
    [SerializeField] private GameObject destroyedModel;
    [SerializeField] private GameObject rebuildingModel;

    [Header("Spawner設定")]
    [SerializeField] private Vector3[] spawnerPositions;
    [SerializeField] private GameObject[] minionPrefabs;
    [SerializeField] private float spawnInterval = 15f;
    [SerializeField] private int waveCount = 5;
    [SerializeField] private float spawnDelay = 0.5f;

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
            default: return 0f;
        }
    }

    public GameObject ActiveModel => activeModel;
    public GameObject DestroyedModel => destroyedModel;
    public GameObject RebuildingModel => rebuildingModel;
    public Vector3[] SpawnerPositions => spawnerPositions;
    public GameObject[] MinionPrefabs => minionPrefabs;
    public float SpawnInterval => spawnInterval;
    public int WaveCount => waveCount;
    public float SpawnDelay => spawnDelay;
}