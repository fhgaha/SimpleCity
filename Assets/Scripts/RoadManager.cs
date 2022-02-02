using SVS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadManager : MonoBehaviour
{
    public PlacementManager PlacementManager;
    public List<Vector3Int> TemporaryPlacementPositions = new List<Vector3Int>();
    public List<Vector3Int> RoadPositionsToRecheck = new List<Vector3Int>();

    private Vector3Int startPosition;
    private bool placementMode = false;

    public RoadFixer RoadFixer;

    private void Start()
    {
        RoadFixer = GetComponent<RoadFixer>();
    }

    public void PlaceRoad(Vector3Int position)
    {
        if (!PlacementManager.CheckIfPositionInBound(position)) return;
        if (!PlacementManager.CheckIfPositionIsFree(position)) return;
        if (!placementMode)
        {
            TemporaryPlacementPositions.Clear();
            RoadPositionsToRecheck.Clear();

            ///drawing a road the first time
            placementMode = true;
            startPosition = position;

            TemporaryPlacementPositions.Add(position);
            PlacementManager.PlaceTemporaryStructure(position, RoadFixer.DeadEnd, CellType.Road);
        }
        else
        {
            PlacementManager.RemoveAllTemporaryStructures();
            TemporaryPlacementPositions.Clear();

            foreach (var positionToFix in RoadPositionsToRecheck)
            {
                RoadFixer.FixRoadAtPosition(PlacementManager, positionToFix);
            }

            RoadPositionsToRecheck.Clear();

            TemporaryPlacementPositions = PlacementManager.GetPathBetween(startPosition, position);

            foreach (var temporaryPosition in TemporaryPlacementPositions)
            {
                if (!PlacementManager.CheckIfPositionIsFree(temporaryPosition)) continue;
                PlacementManager.PlaceTemporaryStructure(temporaryPosition, RoadFixer.DeadEnd, CellType.Road);
            }
        }

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
            {
                if (!RoadPositionsToRecheck.Contains(roadPosition))
                    RoadPositionsToRecheck.Add(roadPosition);
            }
        }

        ///fix neighbour roads
        foreach (var positionToFix in RoadPositionsToRecheck)
            RoadFixer.FixRoadAtPosition(PlacementManager, positionToFix);
    }

    public void FinishPlacingRoad()
    {
        placementMode = false;
        PlacementManager.AddTemporaryStructuresToStructureDictionary();
        if (TemporaryPlacementPositions.Count > 0)
        {
            AudioPlayer.instance.PlayPlacementSound();
        }
        TemporaryPlacementPositions.Clear();
        startPosition = Vector3Int.zero;
    }
}
