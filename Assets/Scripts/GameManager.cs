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

    private void Start()
    {
        InputManager.OnMouseClick += HandleMouseClick;
    }

    private void HandleMouseClick(Vector3Int position)
    {
        Debug.Log(position);
        RoadManager.PlaceRoad(position);
    }

    private void Update()
    {
        CameraMovement.MoveCamera(new Vector3(InputManager.CameraMovementVector.x, 0,
            InputManager.CameraMovementVector.y));
    }
}
