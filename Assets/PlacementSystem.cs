using System;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private GridTemplate gridTemplate;
    [SerializeField] private GameObject cityHallPrefab;
    [SerializeField] private Vector2Int cityHallGridPosition = Vector2Int.zero;
    [SerializeField] private CityHallStats cityHallStats;

    public event Action<BuildingType, GameObject> OnBuildingPlaced;
    public event Action OnCityHallBuilt;

    private float buildProgress = 0f;
    private bool isBuilding = false;
    private Team builderTeam = Team.None;
    private CityHall currentCityHall = null;

    void Start()
    {
        if (gridTemplate != null)
            PlaceAll();
    }

    // ── 初期配置 ─────────────────────────────────────────────

    public void PlaceAll()
    {
        foreach (var entry in gridTemplate.entries)
            PlaceBuilding(entry);
    }

    private void PlaceBuilding(GridTemplate.BuildingEntry entry)
    {
        if (!gridManager.IsInBounds(entry.gridPosition))
        {
            Debug.LogWarning($"[PlacementSystem] {entry.buildingType} が範囲外: {entry.gridPosition}");
            return;
        }

        Vector3 worldPos = gridManager.GridToWorld(entry.gridPosition);
        GameObject building = Instantiate(entry.prefab, worldPos, Quaternion.identity, transform);
        building.name = $"{entry.buildingType}_{entry.gridPosition}";

        if (entry.buildingType == BuildingType.CityHall)
            RegisterCityHall(building);

        OnBuildingPlaced?.Invoke(entry.buildingType, building);
    }

    // ── 動的建設（MinionBuildから） ───────────────────────────

    public bool HasCityHall() => currentCityHall != null;

    public void StartBuild(Team team)
    {
        if (isBuilding) return;
        isBuilding = true;
        builderTeam = team;
        buildProgress = 0f;
        Debug.Log($"[PlacementSystem] 建設開始 Team:{team}");
    }

    public void AddBuildProgress(float power)
    {
        if (!isBuilding) return;

        buildProgress += power * Time.deltaTime;

        float required = cityHallStats != null ? cityHallStats.BuildRequired : 100f;
        Debug.Log($"[PlacementSystem] 建設進捗:{buildProgress:F1} / {required:F1}");

        if (buildProgress >= required)
            CompleteBuild();
    }

    private void CompleteBuild()
    {
        isBuilding = false;
        Debug.Log($"[PlacementSystem] 建設完了！CityHallをInstantiate Team:{builderTeam}");

        if (cityHallPrefab == null)
        {
            Debug.LogWarning("[PlacementSystem] cityHallPrefabがアサインされていない");
            return;
        }
        if (!gridManager.IsInBounds(cityHallGridPosition)) return;

        Vector3 worldPos = gridManager.GridToWorld(cityHallGridPosition);
        GameObject building = Instantiate(cityHallPrefab, worldPos, Quaternion.identity, transform);
        building.name = $"CityHall_{cityHallGridPosition}";

        TeamMember tm = building.GetComponent<TeamMember>();
        if (tm != null)
        {
            tm.SetTeam(builderTeam);
            Debug.Log($"[PlacementSystem] CityHallのTeam設定:{tm.Team}");
        }
        else
        {
            Debug.LogWarning("[PlacementSystem] CityHallにTeamMemberがない！");
        }

        RegisterCityHall(building);

        OnCityHallBuilt?.Invoke();
    }

    private void RegisterCityHall(GameObject building)
    {
        currentCityHall = building.GetComponent<CityHall>();
        if (currentCityHall != null)
            currentCityHall.OnDestroyed += OnCityHallDestroyed;
    }

    private void OnCityHallDestroyed(Team attackerTeam)
    {
        Debug.Log($"[PlacementSystem] CityHall破壊 attackerTeam:{attackerTeam}");
        currentCityHall = null;
        buildProgress = 0f;
        isBuilding = false;
    }
}