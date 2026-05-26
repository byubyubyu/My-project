using UnityEngine;

/// <summary>
/// Baseが持つ全Pathを管理する。
/// BarracksなどはこのHolderからPathを取得して使う。
/// Pathの追加・管理はInspectorで行う。
/// </summary>
public class WaypointHolder : MonoBehaviour
{
    [SerializeField] private Transform[] waypointPaths;

    public int PathCount => waypointPaths != null ? waypointPaths.Length : 0;

    /// <summary>指定indexのPathのWaypoint配列を返す。</summary>
    public Transform[] GetWaypoints(int pathIndex = 0)
    {
        if (waypointPaths == null || pathIndex >= waypointPaths.Length) return null;

        Transform path = waypointPaths[pathIndex];
        if (path == null) return null;

        Transform[] waypoints = new Transform[path.childCount];
        for (int i = 0; i < path.childCount; i++)
            waypoints[i] = path.GetChild(i);

        return waypoints;
    }

    /// <summary>全PathのWaypoint配列を返す。</summary>
    public Transform[][] GetAllWaypoints()
    {
        if (waypointPaths == null) return null;

        Transform[][] result = new Transform[waypointPaths.Length][];
        for (int i = 0; i < waypointPaths.Length; i++)
            result[i] = GetWaypoints(i);

        return result;
    }

    /// <summary>GameAIやBarracksからPathのTransformを直接取得したい場合。</summary>
    public Transform GetPath(int pathIndex)
    {
        if (waypointPaths == null || pathIndex >= waypointPaths.Length) return null;
        return waypointPaths[pathIndex];
    }
}