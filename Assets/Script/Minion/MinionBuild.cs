using System;
using UnityEngine;
using Debug = UnityEngine.Debug;

/// <summary>
/// 建設能力を持つMinionのアクション。
/// Baseに到達したらCityHallがなければ建設に参加する。
/// 敵がいる間は建設を中断し、いなくなったら再開する。
/// CityHallが建ったら自身をDestroyする。
/// </summary>
public class MinionBuild : MonoBehaviour, IMinionAction
{
    public event Action OnActionStart;
    public event Action OnActionEnd;
    public event Action<Vector3> OnChaseTarget;
    public bool StopsMovement => true;
    public int PriorityLevel => 3;

    [SerializeField] private float searchRadius = 30f;

    private float buildPower;
    private TeamMember teamMember;
    private MinionDetection detection;
    private MinionMove minionMove;

    private PlacementSystem targetPlacement = null;
    private bool isBuilding = false;
    private bool hasArrived = false;

    void Start()
    {
        teamMember = GetComponent<TeamMember>();
        detection = GetComponent<MinionDetection>();
        minionMove = GetComponent<MinionMove>();

        StatCalculator calculator = GetComponent<StatCalculator>();
        if (calculator != null)
            buildPower = calculator.GetStat(StatType.BuildPower);

        if (buildPower <= 0f)
        {
            Debug.Log($"[MinionBuild] {gameObject.name} BuildPowerが0のため無効化");
            enabled = false;
            return;
        }

        Debug.Log($"[MinionBuild] {gameObject.name} 初期化完了 BuildPower:{buildPower}");

        if (minionMove != null)
            minionMove.OnDestinationReached += OnDestinationReached;
    }

    void Update()
    {
        if (!hasArrived || targetPlacement == null) return;

        if (detection != null && detection.FindNearestEnemy() != null)
        {
            if (isBuilding)
            {
                isBuilding = false;
                OnActionEnd?.Invoke();
                Debug.Log($"[MinionBuild] {gameObject.name} 敵検知のため建設中断");
            }
            return;
        }

        if (!isBuilding)
        {
            isBuilding = true;
            OnActionStart?.Invoke();
            Debug.Log($"[MinionBuild] {gameObject.name} 建設開始");
        }

        targetPlacement.AddBuildProgress(buildPower);
    }

    private void OnDestinationReached()
    {
        hasArrived = true;
        Debug.Log($"[MinionBuild] {gameObject.name} 終点到達");

        PlacementSystem placement = FindNearestPlacement();
        if (placement == null)
        {
            Debug.LogWarning($"[MinionBuild] {gameObject.name} 近くにBaseが見つからない（searchRadius:{searchRadius}）");
            return;
        }

        if (placement.HasCityHall())
        {
            Debug.Log($"[MinionBuild] {gameObject.name} CityHallが既にあるためDestroy");
            Destroy(gameObject);
            return;
        }

        targetPlacement = placement;
        targetPlacement.StartBuild(teamMember.Team);
        targetPlacement.OnCityHallBuilt += OnCityHallBuilt;
        Debug.Log($"[MinionBuild] {gameObject.name} 建設参加 Team:{teamMember.Team}");
    }

    private PlacementSystem FindNearestPlacement()
    {
        PlacementSystem nearest = null;
        float minDistance = searchRadius;

        GameObject[] bases = GameObject.FindGameObjectsWithTag("Base");
        Debug.Log($"[MinionBuild] Baseタグのオブジェクト数:{bases.Length}");

        foreach (GameObject baseObj in bases)
        {
            float distance = Vector3.Distance(transform.position, baseObj.transform.position);
            if (distance > searchRadius) continue;

            PlacementSystem ps = baseObj.GetComponentInChildren<PlacementSystem>();
            if (ps == null) continue;

            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = ps;
            }
        }
        return nearest;
    }

    private void OnCityHallBuilt()
    {
        Debug.Log($"[MinionBuild] {gameObject.name} CityHall建設完了 Destroy");
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        if (minionMove != null)
            minionMove.OnDestinationReached -= OnDestinationReached;

        if (targetPlacement != null)
            targetPlacement.OnCityHallBuilt -= OnCityHallBuilt;
    }
}