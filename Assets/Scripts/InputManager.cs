using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    public Action<Vector3Int> OnMouseClick, OnMouseHold;
    public Action OnMouseUp;
    private Vector2 cameraMovementVector;

    [SerializeField]
    Camera mainCamera;

    public LayerMask GroundMask;

    public Vector2 CameraMovementVector
    {
        get { return cameraMovementVector; }
    }

    private void Update()
    {
        CheckClickDownEvent();
        CheckClickUpEvent();
        CheckClickHoldEvent();
        CheckArrowInput();
    }

    private Vector3Int? RaycastGround()
    {
        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, GroundMask))
        {
            Vector3Int positionHit = Vector3Int.RoundToInt(hit.point);
            return positionHit;
        }
        return null;
    }

    private void CheckArrowInput()
    {
        ///Input.GetAxis("Horizontal") - reads keys A and D
        ///Input.GetAxis("Vertical") - reads keys W and S
        cameraMovementVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    private void CheckClickHoldEvent()
    {
        ///Check if mouse pointer not over any object of type UI
        ///Input.GetMouseButton(0) - LMB
        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            var position = RaycastGround();
            if (position != null)
                OnMouseHold?.Invoke(position.Value);
        }
    }

    private void CheckClickUpEvent()
    {
        if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
            OnMouseUp?.Invoke();
    }

    private void CheckClickDownEvent()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            var position = RaycastGround();
            if (position != null)
                OnMouseClick?.Invoke(position.Value);
        }
    }
}
