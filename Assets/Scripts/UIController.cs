using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Action OnRoadPlacement, OnHousePlacement, OnSpecialPlacement, OnBigStructurePlacement;
    public Button PlaceRoadButton, PlaceHouseButton, PlaceSpecialButton, PlaceBigStructureButton;

    public Color OutlineColor;
    private List<Button> buttonList;

    private void Start()
    {
        buttonList = new List<Button> { PlaceRoadButton, PlaceHouseButton, PlaceSpecialButton, 
            PlaceBigStructureButton };

        PlaceRoadButton.onClick.AddListener(() => 
        {
            ResetButtonColor();
            ModifyOutline(PlaceRoadButton);
            OnRoadPlacement?.Invoke();
        });

        PlaceHouseButton.onClick.AddListener(() =>
        {
            ResetButtonColor();
            ModifyOutline(PlaceHouseButton);
            OnHousePlacement?.Invoke();
        });

        PlaceSpecialButton.onClick.AddListener(() =>
        {
            ResetButtonColor();
            ModifyOutline(PlaceSpecialButton);
            OnSpecialPlacement?.Invoke();
        });

        PlaceBigStructureButton.onClick.AddListener(() =>
        {
            ResetButtonColor();
            ModifyOutline(PlaceBigStructureButton);
            OnBigStructurePlacement?.Invoke();
        });
    }

    private void ModifyOutline(Button button)
    {
        var outline = button.GetComponent<Outline>();
        outline.effectColor = OutlineColor;
        outline.enabled = true;
    }

    private void ResetButtonColor()
    {
        foreach (Button button in buttonList)
            button.GetComponent<Outline>().enabled = false;
    }
}
