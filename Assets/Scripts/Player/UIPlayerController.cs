using UnityEngine;

public class UIPlayerController : MonoBehaviour
{
    InputController m_InputController;
    UIInventoryController m_InventoryController;
    CommonPlacer m_CommonPlacer;
    SplinePlacer m_SplinePlacer;


    void Awake()
    {
        m_InputController = GetComponent<InputController>();
        m_InventoryController = GetComponent<UIInventoryController>();
        m_CommonPlacer = GetComponent<CommonPlacer>();
        m_SplinePlacer = GetComponent<SplinePlacer>();
    }
    private void OnEnable()
    {
        m_InputController.OnFirstS_Tap += m_InventoryController.FirstSClick;
        m_InputController.OnSecondS_Tap += m_InventoryController.SecondSClick;
        m_InputController.OnThirdS_Tap += m_InventoryController.ThirdSClick;
        m_InputController.OnForthS_Tap += m_InventoryController.ForthSClick;
        m_InputController.OnFifthS_Tap += m_InventoryController.FifthSClick;

        m_InputController.OnRotateR_Tap += m_CommonPlacer.RotateRightClick;
        m_InputController.OnRotateL_Tap += m_CommonPlacer.RotateLeftClick;
        m_InputController.OnPlaceBuilding_Tap += m_CommonPlacer.PlaceBuildingClick;

        m_InputController.OnPlaceBuilding_Tap += m_SplinePlacer.PlaceBuildingClick;
        m_InputController.OnSwitchTypeStart_Tap += m_SplinePlacer.SwitchTypeStartClick;
        m_InputController.OnSwitchTypeEnd_Tap += m_SplinePlacer.SwitchTypeEndClick;
    }
    private void OnDisable()
    {
        m_InputController.OnFirstS_Tap -= m_InventoryController.FirstSClick;
        m_InputController.OnSecondS_Tap -= m_InventoryController.SecondSClick;
        m_InputController.OnThirdS_Tap -= m_InventoryController.ThirdSClick;
        m_InputController.OnForthS_Tap -= m_InventoryController.ForthSClick;
        m_InputController.OnFifthS_Tap -= m_InventoryController.FifthSClick;

        m_InputController.OnRotateR_Tap -= m_CommonPlacer.RotateRightClick;
        m_InputController.OnRotateL_Tap -= m_CommonPlacer.RotateLeftClick;
        m_InputController.OnPlaceBuilding_Tap -= m_CommonPlacer.PlaceBuildingClick;

        m_InputController.OnPlaceBuilding_Tap -= m_SplinePlacer.PlaceBuildingClick;
        m_InputController.OnSwitchTypeStart_Tap -= m_SplinePlacer.SwitchTypeStartClick;
        m_InputController.OnSwitchTypeEnd_Tap -= m_SplinePlacer.SwitchTypeEndClick;
    }






}
