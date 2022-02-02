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
    private Dictionary<Vector3Int, StructureModel> structureDictionary = new Dictionary<Vector3Int,
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

    internal void PlaceObjectOnMap(Vector3Int position, GameObject structurePrefab, CellType type)
    {
        placementGrid[position.x, position.z] = type;
        StructureModel structure = CreateNewStructureModel(position, structurePrefab, type);
        structureDictionary.Add(position, structure);
        DectoyNatureAt(position);
    }

    private void DectoyNatureAt(Vector3Int position)
    {
        RaycastHit[] hits = Physics.BoxCastAll(position + new Vector3(0, 0.5f, 0), new Vector3(0.5f, 0.5f, 0.5f),
            transform.up, Quaternion.identity, 1f, 1 << LayerMask.NameToLayer("Nature"));

        foreach (var item in hits)
            Destroy(item.collider.gameObject);
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
        else if (structureDictionary.ContainsKey(position))
            structureDictionary[position].SwapModel(newModel, rotation);
    }
    
    internal List<Vector3Int> GetPathBetween(Vector3Int startPosition, Vector3Int endPosition)
    {
        var resultPath = GridSearch.AStarSearch(placementGrid, 
            new Point(startPosition.x, startPosition.z),
            new Point(endPosition.x, endPosition.z));

        List<Vector3Int> path = new List<Vector3Int>();
        foreach (var point in resultPath)
            path.Add(new Vector3Int(point.X, 0, point.Y));

        return path;
    }

    internal void RemoveAllTemporaryStructures()
    {
        foreach (var structure in temporaryRoadObjects.Values)
        {
            var position = Vector3Int.RoundToInt(structure.transform.position);
            placementGrid[position.x, position.z] = CellType.Empty;
            Destroy(structure.gameObject);
        }
        temporaryRoadObjects.Clear();
    }

    internal void AddTemporaryStructuresToStructureDictionary()
    {
        foreach (var structure in temporaryRoadObjects)
        {
            structureDictionary.Add(structure.Key, structure.Value);
            DectoyNatureAt(structure.Key);
        }
        temporaryRoadObjects.Clear();
    }
}
