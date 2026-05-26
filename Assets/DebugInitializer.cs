using System;
using UnityEngine;
using Debug = UnityEngine.Debug;

/// <summary>
/// デバッグ・テスト用の初期設定。
/// 本来は土地に建物は建っていないが、テスト用に最初から建物を配置してチームを設定する。
/// 本番実装が進んだら外すだけでよい。
/// </summary>
public class DebugInitializer : MonoBehaviour
{
    [SerializeField] private BaseSetup[] baseSetups;

    [Serializable]
    public class BaseSetup
    {
        public PlacementSystem placementSystem;
        public Team team;
        public GameObject[] minionPrefabs;
        public float spawnInterval = 15f;
        public float spawnDelay = 0.5f;
    }

    void Awake()
    {
        foreach (var setup in baseSetups)
        {
            if (setup.placementSystem == null) continue;

            setup.placementSystem.OnBuildingPlaced += (type, building) =>
            {
                TeamMember tm = building.GetComponent<TeamMember>();
                if (tm != null) tm.SetTeam(setup.team);

                if (type == BuildingType.Barracks)
                {
                    Barracks barracks = building.GetComponent<Barracks>();
                    if (barracks != null)
                        barracks.SetSpawnSettings(setup.minionPrefabs, setup.spawnInterval, setup.spawnDelay);
                }
            };
        }
    }
}