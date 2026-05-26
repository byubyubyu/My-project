using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MinionSpawner : MonoBehaviour
{
    private GameObject[] minionPrefabs;
    private Team currentTeam = Team.Blue;
    private float spawnInterval = 15f;
    private int waveCount = 5;
    private float spawnDelay = 0.5f;
    private WaypointHolder waypointHolder;
    private Coroutine spawnCoroutine;

    void Awake()
    {
        waypointHolder = GetComponent<WaypointHolder>();
    }

    void Start()
    {
        TeamMember tm = GetComponent<TeamMember>();
        if (tm != null)
        {
            currentTeam = tm.Team;
            tm.OnTeamChanged += ChangeTeam;
        }
    }

    public void SetStats(BaseStats stats)
    {
        spawnInterval = stats.SpawnInterval;
        waveCount = stats.WaveCount;
        spawnDelay = stats.SpawnDelay;
        minionPrefabs = stats.MinionPrefabs;

        StartSpawning();
    }

    void StartSpawning()
    {
        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);
        spawnCoroutine = StartCoroutine(SpawnLoop());
    }

    void StopSpawning()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return StartCoroutine(SpawnWave()); // 最初はすぐSpawn
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    IEnumerator SpawnWave()
    {
        for (int i = 0; i < waveCount; i++)
        {
            SpawnOne();
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    void SpawnOne()
    {
        if (minionPrefabs == null || minionPrefabs.Length == 0) return;

        GameObject prefab = minionPrefabs[UnityEngine.Random.Range(0, minionPrefabs.Length)];

        Vector3 spawnPos = new Vector3(transform.position.x, 1f, transform.position.z);
        NavMeshHit hit;
        if (NavMesh.SamplePosition(spawnPos, out hit, 20f, NavMesh.AllAreas))
        {
            spawnPos = hit.position;
        }
        else
        {
            return;
        }

        GameObject minion = Instantiate(prefab, spawnPos, Quaternion.identity);

        TeamMember tm = minion.GetComponent<TeamMember>();
        if (tm != null)
        {
            tm.SetTeam(currentTeam);
        }

        MinionMove mm = minion.GetComponent<MinionMove>();
        if (mm != null && waypointHolder != null)
        {
            Transform[] waypoints = waypointHolder.GetWaypoints();
            mm.SetWaypoints(waypoints);
        }
    }

    public void SetTeamBeforeStart(Team team)
    {
        currentTeam = team;
    }

    public void ChangeTeam(Team team)
    {
        currentTeam = team;
    }

    void OnDestroy()
    {
        TeamMember tm = GetComponent<TeamMember>();
        if (tm != null)
        {
            tm.OnTeamChanged -= ChangeTeam;
        }
        StopSpawning();
    }
}