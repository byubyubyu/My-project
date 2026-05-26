using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int width = 50;
    [SerializeField] private int height = 50;
    [SerializeField] private float cellSize = 2f;

    public int Width => width;
    public int Height => height;
    public float CellSize => cellSize;

    public Vector3 GridToWorld(Vector2Int gridPos)
    {
        return transform.position
            + new Vector3(gridPos.x * cellSize, 0f, gridPos.y * cellSize);
    }

    public bool IsInBounds(Vector2Int gridPos)
    {
        return gridPos.x >= 0 && gridPos.x < width
            && gridPos.y >= 0 && gridPos.y < height;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        for (int x = 0; x < width; x++)
            for (int z = 0; z < height; z++)
            {
                Vector3 center = GridToWorld(new Vector2Int(x, z));
                Gizmos.DrawWireCube(center, new Vector3(cellSize, 0.1f, cellSize));
            }
    }
}