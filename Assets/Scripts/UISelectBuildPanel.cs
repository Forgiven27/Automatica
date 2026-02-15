using UnityEngine;
using System.Collections.Generic;
using System;

public class UISelectBuildPanel : UIPanel
{
    [SerializeField]private UIBuildingCard _uIBuildingCard;
    [SerializeField] private RectTransform _cardsContainer; 
    private BuildButton _buildInfo;
    private List<UIBuildingCard> cards = new();
    event Action onInfoChanged;



    public void SetBuildings(BuildButton buildButton) 
    { 
        _buildInfo = buildButton;
        onInfoChanged?.Invoke();
        UpdateVisual();
    }


    void OnEnable()
    {
        onInfoChanged += UpdateVisual;
    }

    void UpdateVisual()
    {
        foreach (var card in cards)
        {
            card.GetComponent<UIBuildingCard>().OnClick -= SetCurrentBuilding;
            Destroy(card);
        }
        cards.Clear();

        for (var i = 0; i < _buildInfo.buildings.Count; i++)
        {
            var card = Instantiate(_uIBuildingCard, _cardsContainer);
            var uicard = card.GetComponent<UIBuildingCard>();
            uicard.SetBuilding(_buildInfo.buildings[i]);
            uicard.OnClick += SetCurrentBuilding;
            cards.Add(card);
        }
    }

    private void SetCurrentBuilding(Building building)
    {
        _buildInfo.currentBuilding = building;
    }


    private void OnDisable()
    {
        onInfoChanged -= UpdateVisual;
    }
}
