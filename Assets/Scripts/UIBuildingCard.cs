using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UIBuildingCard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _headerText;
    [SerializeField] private Image _imagePreview;
    [SerializeField] private UICardBehavior _inputHandler;
    private Building currentBuilding;

    public event Action<Building> OnClick; 

    public void SetHeaderText(string text)
    {
        _headerText.text = text;
    }

    public void SetImagePreview(Sprite sprite)
    {
        _imagePreview.sprite = sprite;
    }

    public void SetBuilding(Building building)
    {
        currentBuilding = building;
        SetHeaderText(building.header);
        SetImagePreview(building.sprite);
    }

    private void OnEnable()
    {
        _inputHandler.onMouseEnter += ActiveUIState;
        _inputHandler.onMouseExit += DeactiveUIState;
        _inputHandler.onMouseClick += OnMouseClick;
    }

    private void ActiveUIState()
    {
        
    }

    private void DeactiveUIState()
    {

    }

    private void OnMouseClick()
    {
        if (currentBuilding != null)
        {
            OnClick.Invoke(currentBuilding);
        }
    }
    private void OnDisable()
    {
        _inputHandler.onMouseEnter -= ActiveUIState;
        _inputHandler.onMouseExit -= DeactiveUIState;
        _inputHandler.onMouseClick -= OnMouseClick;
    }


}
