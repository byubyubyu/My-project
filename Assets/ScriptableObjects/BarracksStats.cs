using UnityEngine;

[CreateAssetMenu(fileName = "BarracksStats", menuName = "Stats/BarracksStats")]
public class BarracksStats : ScriptableObject, IStats
{
    [SerializeField] private float hp = 100f;
    [SerializeField] private GameObject[] minionPrefabs;
    [SerializeField] private float spawnInterval = 15f;
    [SerializeField] private float spawnDelay = 0.5f;

    public float HP => hp;
    public GameObject[] MinionPrefabs => minionPrefabs;
    public float SpawnInterval => spawnInterval;
    public float SpawnDelay => spawnDelay;

    public float GetStat(StatType type)
    {
        switch (type)
        {
            case StatType.HP: return hp;
            default: return 0f;
        }
    }
}