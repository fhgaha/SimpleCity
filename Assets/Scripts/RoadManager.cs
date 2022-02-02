using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadManager : MonoBehaviour
{
    public PlacementManager PlacementManager;
    public List<Vector3Int> TemporaryPlacementPositions = new List<Vector3Int>();
    public List<Vector3Int> RoadPositionsToRecheck = new List<Vector3Int>();
    public GameObject RoadStraight;
    public RoadFixer RoadFixer;

    private void Start()
    {
        RoadFixer = GetComponent<RoadFixer>();
    }

    public void PlaceRoad(Vector3Int position)
    {
        if (!PlacementManager.CheckIfPositionInBound(position)) return;
        if (!PlacementManager.CheckIfPositionIsFree(position)) return;

        TemporaryPlacementPositions.Clear();
        TemporaryPlacementPositions.Add(position);
        PlacementManager.PlaceTemporaryStructure(position, RoadStraight, CellType.Road);
        FixRoadPrefabs();
    }

    private void FixRoadPrefabs()
    {
        foreach (var temporaryPosition in TemporaryPlacementPositions)
        {
            ///fix last placed road
            RoadFixer.FixRoadAtPosition(PlacementManager, temporaryPosition);

            ///this for fixing neighbour roads
            var neighbours = PlacementManager.GetNeighboursOfTypeFor(temporaryPosition, CellType.Road);
            foreach (var roadPosition in neighbours)
                RoadPositionsToRecheck.Add(roadPosition);
        }

        ///fix neighbour roads
        foreach (var positionToFix in RoadPositionsToRecheck)
            RoadFixer.FixRoadAtPosition(PlacementManager, positionToFix);
    }
}
