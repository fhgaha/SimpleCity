using SVS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public CameraMovement CameraMovement;
    public RoadManager RoadManager;
    public InputManager InputManager;
    public UIController UiController;
    public StructureManager StructureManager;

    private void Start()
    {
        UiController.OnRoadPlacement += RoadPlacementHandler;
        UiController.OnHousePlacement += HousePlacementHandler;
        UiController.OnSpecialPlacement += SpecialPlacementHandler;
    }

    private void SpecialPlacementHandler()
    {
        ClearInputActions();
        InputManager.OnMouseClick += StructureManager.PlaceSpecial;
    }

    private void HousePlacementHandler()
    {
        ClearInputActions();
        InputManager.OnMouseClick += StructureManager.PlaceHouse;
    }

    private void RoadPlacementHandler()
    {
        ClearInputActions();

        InputManager.OnMouseClick += RoadManager.PlaceRoad;
        InputManager.OnMouseHold += RoadManager.PlaceRoad;
        InputManager.OnMouseUp += RoadManager.FinishPlacingRoad;
    }

    private void ClearInputActions()
    {
        InputManager.OnMouseClick = null;
        InputManager.OnMouseHold = null;
        InputManager.OnMouseUp = null;
    }

    private void Update()
    {
        CameraMovement.MoveCamera(new Vector3(InputManager.CameraMovementVector.x, 0,
            InputManager.CameraMovementVector.y));
    }
}
