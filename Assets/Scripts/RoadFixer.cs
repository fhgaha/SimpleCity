using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoadFixer : MonoBehaviour
{
    public GameObject DeadEnd, RoadStraight, Corner, ThreeWay, FourWay;

    public void FixRoadAtPosition(PlacementManager placementManager, Vector3Int temporaryPosition)
    {
        ///[right, up, left, down]
        var result = placementManager.GetNeighbourTypesFor(temporaryPosition);

        int roadCount = 0;
        roadCount = result.Where(x => x == CellType.Road).Count();

        switch (roadCount)
        {
            case 0:
            case 1:
                CreateDeadEnd(placementManager, result, temporaryPosition);
                break;
            case 2:
                if (CreateStraightRoad(placementManager, result, temporaryPosition)) return;
                CreateCorner(placementManager, result, temporaryPosition);
                break;
            case 3:
                Create3Way(placementManager, result, temporaryPosition);
                break;
            default:
                Create4Way(placementManager, result, temporaryPosition);
                break;
        }
    }

    private void Create4Way(PlacementManager placementManager, CellType[] result, Vector3Int position)
    {
        placementManager.ModifyStructureModel(position, FourWay, Quaternion.identity);
    }

    ///[left, up, right, down]
    private void Create3Way(PlacementManager placementManager, CellType[] result, Vector3Int position)
    {
        if (result[1] == CellType.Road && result[2] == CellType.Road && result[3] == CellType.Road)
            placementManager.ModifyStructureModel(position, ThreeWay, Quaternion.identity);
        else if (result[2] == CellType.Road && result[3] == CellType.Road && result[0] == CellType.Road)
            placementManager.ModifyStructureModel(position, ThreeWay, Quaternion.Euler(0, 90, 0));
        else if (result[3] == CellType.Road && result[0] == CellType.Road && result[1] == CellType.Road)
            placementManager.ModifyStructureModel(position, ThreeWay, Quaternion.Euler(0, 180, 0));
        else if (result[0] == CellType.Road && result[1] == CellType.Road && result[2] == CellType.Road)
            placementManager.ModifyStructureModel(position, ThreeWay, Quaternion.Euler(0, 270, 0));
    }

    ///[left, up, right, down]
    private void CreateCorner(PlacementManager placementManager, CellType[] result, Vector3Int position)
    {
        if (result[1] == CellType.Road && result[2] == CellType.Road)
            placementManager.ModifyStructureModel(position, Corner, Quaternion.Euler(0, 90, 0));
        else if (result[2] == CellType.Road && result[3] == CellType.Road)
            placementManager.ModifyStructureModel(position, Corner, Quaternion.Euler(0, 180, 0));
        else if (result[3] == CellType.Road && result[0] == CellType.Road)
            placementManager.ModifyStructureModel(position, Corner, Quaternion.Euler(0, 270, 0));
        else if (result[0] == CellType.Road && result[1] == CellType.Road)
            placementManager.ModifyStructureModel(position, Corner, Quaternion.identity);
    }

    ///[left, up, right, down]
    private bool CreateStraightRoad(PlacementManager placementManager, CellType[] result, Vector3Int position)
    {
        if (result[0] == CellType.Road && result[2] == CellType.Road)
        {
            placementManager.ModifyStructureModel(position, RoadStraight, Quaternion.identity);
            return true;
        }
        else if (result[1] == CellType.Road && result[3] == CellType.Road)
        {
            placementManager.ModifyStructureModel(position, RoadStraight, Quaternion.Euler(0, 90, 0));
            return true;
        }
        return false;
    }

    ///[left, up, right, down]
    private void CreateDeadEnd(PlacementManager placementManager, CellType[] result, Vector3Int position)
    {
        if (result[1] == CellType.Road)
            placementManager.ModifyStructureModel(position, DeadEnd, Quaternion.Euler(0, 270, 0));
        else if (result[2] == CellType.Road)
            placementManager.ModifyStructureModel(position, DeadEnd, Quaternion.identity);
        else if (result[3] == CellType.Road)
            placementManager.ModifyStructureModel(position, DeadEnd, Quaternion.Euler(0, 90, 0));
        else if (result[0] == CellType.Road)
            placementManager.ModifyStructureModel(position, DeadEnd, Quaternion.Euler(0, 180, 0));
    }
}
