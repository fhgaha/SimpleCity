using SVS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StructureManager : MonoBehaviour
{
    public StructurePrefabWeighted[] HousesPrefabs, SpecialPrefabs, BigStructurePrefabs;
    public PlacementManager PlacementManager;

    private float[] houseWeights, specialWeights, bigStructureWeights;

    private void Start()
    {
        houseWeights = HousesPrefabs.Select(prefabStats => prefabStats.Weight).ToArray();
        specialWeights = SpecialPrefabs.Select(prefabStats => prefabStats.Weight).ToArray();
        bigStructureWeights = BigStructurePrefabs.Select(prefabStats => prefabStats.Weight).ToArray();
    }

    public void PlaceHouse(Vector3Int position)
    {
        if (CheckPositionBeforePlacement(position))
        {
            int randomIndex = GetRandomWeightedIndex(houseWeights);
            PlacementManager.PlaceObjectOnMap(position, HousesPrefabs[randomIndex].Prefab, CellType.Structure);
            AudioPlayer.instance.PlayPlacementSound();
        }
    }

    internal void PlaceBigStructure(Vector3Int position)
    {
        int width = 2, height = 2;
        if (CheckBigStructure(position, width, height))
        {
            int randomIndex = GetRandomWeightedIndex(bigStructureWeights);
            PlacementManager.PlaceObjectOnMap(position, BigStructurePrefabs[randomIndex].Prefab, CellType.Structure,
                width, height);
            AudioPlayer.instance.PlayPlacementSound();
        }
    }

    private bool CheckBigStructure(Vector3Int position, int width, int height)
    {
        bool nearRoad = false;
        for (int x = 0; x < width; x++)
            for (int z = 0; z < height; z++)
            {
                var newPosition = position + new Vector3Int(x, 0, z);
                
                if (!DefaultCheck(newPosition)) return false;
                if (!nearRoad)
                    nearRoad = RoadCheck(newPosition);
            }

        return nearRoad;
    }

    public void PlaceSpecial(Vector3Int position)   //should refactor
    {
        if (CheckPositionBeforePlacement(position))
        {
            int randomIndex = GetRandomWeightedIndex(specialWeights);
            PlacementManager.PlaceObjectOnMap(position, SpecialPrefabs[randomIndex].Prefab, CellType.Structure);
            AudioPlayer.instance.PlayPlacementSound();
        }
    }

    private int GetRandomWeightedIndex(float[] weights)
    {
        float randomValue = UnityEngine.Random.Range(0, weights.Sum());

        float tempSum = 0;
        for (int i = 0; i < weights.Length; i++)
        {
            if (randomValue >= tempSum && randomValue < tempSum + weights[i])
                return i;

            tempSum += weights[i];
        }
        return 0;
    }

    private bool CheckPositionBeforePlacement(Vector3Int position)
    {
        if (!DefaultCheck(position)) return false;
        if (!RoadCheck(position)) return false;


        return true;
    }

    private bool RoadCheck(Vector3Int position)
    {
        if (PlacementManager.GetNeighboursOfTypeFor(position, CellType.Road).Count <= 0)
        {
            Debug.Log("must be placed near a road");
            return false;
        }
        return true;
    }

    private bool DefaultCheck(Vector3Int position)
    {
        if (!PlacementManager.CheckIfPositionInBound(position))
        {
            Debug.Log("position out of bounds");
            return false;
        }
        if (!PlacementManager.CheckIfPositionIsFree(position))
        {
            Debug.Log("position is not empty");
            return false;
        }
        return true;
    }
}

[Serializable]
public struct StructurePrefabWeighted
{
    public GameObject Prefab;

    [Range(0, 1)]
    public float Weight;    //weight determines which structures will be seen more often
}
