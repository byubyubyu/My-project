using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;
using Debug = UnityEngine.Debug;

/// <summary>
/// Minionを生産・送り出す建物。
/// PathOrderで「どのPathに何匹送るか」を管理する。
/// 命令はGameAIからSetOrder()で受け取る。デバッグ用にInspectorで直接設定も可能。
/// PathIndexが存在しない場合はそのOrderをスキップする。
/// </summary>
public class Barracks : MonoBehaviour, IBarracks
{
    [Serializable]
    public class PathOrder
    {
        public int pathIndex;  // WaypointHolderのPath番号
        public int count;      // このPathに送るMinion数
    }

    [Header("Orders")]
    [SerializeField] private PathOrder[] orders;

    private Team currentTeam;
    private TeamMember teamMember;
    private CityHall cityHall;
    private WaypointHolder waypointHolder;

    private GameObject[] minionPrefabs;
    private float spawnInterval;
    private float spawnDelay;

    private List<PathOrder> currentOrders = new List<PathOrder>();
    private Coroutine spawnCoroutine;

    void Start()
    {
        waypointHolder = GetComponentInParent<WaypointHolder>();

        teamMember = GetComponent<TeamMember>();
        if (teamMember != null)
        {
            currentTeam = teamMember.Team;
            teamMember.OnTeamChanged += OnTeamChanged;
        }

        cityHall = GetComponentInParent<CityHall>();
        if (cityHall != null)
            cityHall.OnDestroyed += OnCityHallDestroyed;

        if (orders != null && orders.Length > 0)
            SetOrder(orders);
    }

    // ── スポーン設定（GameManagerから注入） ──────────────────

    public void SetSpawnSettings(GameObject[] prefabs, float interval, float delay)
    {
        minionPrefabs = prefabs;
        spawnInterval = interval;
        spawnDelay = delay;

        // 既に命令があれば再スタート
        if (currentOrders.Count > 0)
            StartSpawning();
    }

    // ── 命令（GameAIから） ────────────────────────────────────

    /// <summary>どのPathに何匹送るかを設定してスポーン開始。</summary>
    public void SetOrder(PathOrder[] orders)
    {
        currentOrders = new List<PathOrder>(orders);
        StartSpawning();
    }

    // ── スポーンループ ────────────────────────────────────────

    private void StartSpawning()
    {
        StopSpawning();
        if (minionPrefabs == null || minionPrefabs.Length == 0) return;
        if (currentOrders == null || currentOrders.Count == 0) return;
        spawnCoroutine = StartCoroutine(SpawnLoop());
    }

    private void StopSpawning()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    IEnumerator SpawnLoop()
    {
        yield return new WaitForSeconds(0.1f);
        while (true)
        {
            yield return StartCoroutine(SpawnWave());
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    IEnumerator SpawnWave()
    {
        foreach (PathOrder order in currentOrders)
        {
            // Pathが存在しない場合はこのOrderをスキップ
            Transform[] waypoints = waypointHolder != null
                ? waypointHolder.GetWaypoints(order.pathIndex)
                : null;

            if (waypoints == null)
            {
                Debug.LogWarning($"[Barracks] PathIndex {order.pathIndex} が存在しないためスキップ");
                continue;
            }

            for (int i = 0; i < order.count; i++)
            {
                SpawnOne(waypoints);
                yield return new WaitForSeconds(spawnDelay);
            }
        }
    }

    void SpawnOne(Transform[] waypoints)
    {
        if (minionPrefabs == null || minionPrefabs.Length == 0) return;

        GameObject prefab = minionPrefabs[Random.Range(0, minionPrefabs.Length)];

        Vector3 spawnPos = new Vector3(transform.position.x, 1f, transform.position.z);
        NavMeshHit hit;
        if (NavMesh.SamplePosition(spawnPos, out hit, 20f, NavMesh.AllAreas))
            spawnPos = hit.position;
        else
            return;

        GameObject minion = Instantiate(prefab, spawnPos, Quaternion.identity);

        TeamMember tm = minion.GetComponent<TeamMember>();
        if (tm != null) tm.SetTeam(currentTeam);

        MinionMove mm = minion.GetComponent<MinionMove>();
        if (mm != null)
            mm.SetWaypoints(waypoints);
    }

    // ── IBarracks ────────────────────────────────────────────

    public void ProduceMinion(GameObject prefab, Transform[] waypoints)
    {
        if (prefab == null) return;

        Vector3 spawnPos = new Vector3(transform.position.x, 1f, transform.position.z);
        NavMeshHit hit;
        if (NavMesh.SamplePosition(spawnPos, out hit, 20f, NavMesh.AllAreas))
            spawnPos = hit.position;
        else
            return;

        GameObject minion = Instantiate(prefab, spawnPos, Quaternion.identity);

        TeamMember tm = minion.GetComponent<TeamMember>();
        if (tm != null) tm.SetTeam(currentTeam);

        MinionMove mm = minion.GetComponent<MinionMove>();
        if (mm != null && waypoints != null)
            mm.SetWaypoints(waypoints);
    }

    // ── イベント ─────────────────────────────────────────────

    private void OnCityHallDestroyed(Team attackerTeam)
    {
        Destroy(gameObject);
    }

    private void OnTeamChanged(Team newTeam)
    {
        currentTeam = newTeam;
    }

    void OnDestroy()
    {
        if (teamMember != null)
            teamMember.OnTeamChanged -= OnTeamChanged;

        if (cityHall != null)
            cityHall.OnDestroyed -= OnCityHallDestroyed;

        StopSpawning();
    }
}