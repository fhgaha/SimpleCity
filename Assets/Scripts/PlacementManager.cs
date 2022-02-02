using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    public int Width, Height;
    private Grid placementGrid;

    private Dictionary<Vector3Int, StructureModel> temporaryRoadObjects = new Dictionary<Vector3Int,
        StructureModel>();

    private void Start()
    {
        placementGrid = new Grid(Width, Height);
    }

    internal CellType[] GetNeighbourTypesFor(Vector3Int position)
    {
        return placementGrid.GetAllAdjacentCellTypes(position.x, position.z);
    }

    internal bool CheckIfPositionInBound(Vector3Int position)
    {
        return position.x >= 0 && position.x < Width && position.z >= 0 && position.z < Height;
    }

    internal bool CheckIfPositionIsFree(Vector3Int position)
    {
        return CheckIfPositionIsOfType(position, CellType.Empty);
    }

    private bool CheckIfPositionIsOfType(Vector3Int position, CellType type)
    {
        return placementGrid[position.x, position.z] == type;
    }

    internal void PlaceTemporaryStructure(Vector3Int position, GameObject structurePrefab, CellType type)
    {
        placementGrid[position.x, position.z] = type;
        StructureModel structure = CreateNewStructureModel(position, structurePrefab, type);
        temporaryRoadObjects.Add(position, structure);
    }

    internal List<Vector3Int> GetNeighboursOfTypeFor(Vector3Int position, CellType type)
    {
        var neighbourVertices = placementGrid.GetAdjacentCellsOfType(position.x, position.z, type);
        var neighbours = new List<Vector3Int>();
        foreach (var point in neighbourVertices)
            neighbours.Add(new Vector3Int(point.X, 0, point.Y));

        return neighbours;
    }

    private StructureModel CreateNewStructureModel(Vector3Int position, GameObject structurePrefab, CellType type)
    {
        GameObject structure = new GameObject(type.ToString());
        structure.transform.SetParent(transform);
        structure.transform.localPosition = position;
        var structureModel = structure.AddComponent<StructureModel>();
        structureModel.CreateModel(structurePrefab);
        return structureModel;
    }

    public void ModifyStructureModel(Vector3Int position, GameObject newModel, Quaternion rotation)
    {
        if (temporaryRoadObjects.ContainsKey(position))
            temporaryRoadObjects[position].SwapModel(newModel, rotation);
    }
}
