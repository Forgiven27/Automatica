using UnityEngine;
using System.Collections.Generic;

public class UIBuildBoard : UIPanel
{
    [SerializeField]private UIBuildButton _buildButton;
    [SerializeField] private RectTransform _container;
    
    private Dictionary<Building,UIBuildButton> _uIBuildButtons = new();
    
    public void InitButton(Building building, string hintButtonText)
    {
        var button = Instantiate(_buildButton.gameObject, _container);
        var uiBuildButton = button.GetComponent<UIBuildButton>();
        uiBuildButton.InitButton(building.sprite, hintButtonText);
        _uIBuildButtons.Add(building, uiBuildButton);
    }

    public void UpdateButton(Building building)
    {
        if (_uIBuildButtons.TryGetValue(building, out var button))
        {
            button.UpdateSprite(building.sprite);
        }
    }

    public void UpdateButton(Building building, string hintButtonText)
    {
        if (_uIBuildButtons.TryGetValue(building, out var button))
        {
            button.InitButton(building.sprite, hintButtonText);
        }
    }

    public void DestroyButtons()
    {
        foreach ((var key, var value) in _uIBuildButtons)
        {
            Destroy(_uIBuildButtons[key].gameObject);
        }
        _uIBuildButtons.Clear();
    }
}
