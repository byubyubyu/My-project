using System;
using UnityEngine;

public interface IBarracks
{
    void ProduceMinion(GameObject prefab, Transform[] waypoints);
}