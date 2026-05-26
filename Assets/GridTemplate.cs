using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GridTemplate", menuName = "Map/GridTemplate")]
public class GridTemplate : ScriptableObject
{
    [Serializable]
    public class BuildingEntry
    {
        public BuildingType buildingType;
        public Vector2Int gridPosition;
        public GameObject prefab;
    }

    public List<BuildingEntry> entries = new();
}