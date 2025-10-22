using UnityEngine;

public class UIPlayerController : MonoBehaviour
{
    InputController m_InputController;
    UIInventoryController m_InventoryController;
    public GridController GridController;


    void Awake()
    {
        m_InputController = GetComponent<InputController>();
        m_InventoryController = GetComponent<UIInventoryController>();
    }
    private void OnEnable()
    {
        m_InputController.OnFirstS_Tap += m_InventoryController.FirstSClick;
        m_InputController.OnSecondS_Tap += m_InventoryController.SecondSClick;
        m_InputController.OnThirdS_Tap += m_InventoryController.ThirdSClick;
        m_InputController.OnForthS_Tap += m_InventoryController.ForthSClick;
        m_InputController.OnFifthS_Tap += m_InventoryController.FifthSClick;

        m_InputController.OnRotateR_Tap += GridController.RotateRightClick;
        m_InputController.OnRotateL_Tap += GridController.RotateLeftClick;
        m_InputController.OnPlaceBuilding_Tap += GridController.PlaceBuildingClick;
    }
    private void OnDisable()
    {
        m_InputController.OnFirstS_Tap -= m_InventoryController.FirstSClick;
        m_InputController.OnSecondS_Tap -= m_InventoryController.SecondSClick;
        m_InputController.OnThirdS_Tap -= m_InventoryController.ThirdSClick;
        m_InputController.OnForthS_Tap -= m_InventoryController.ForthSClick;
        m_InputController.OnFifthS_Tap -= m_InventoryController.FifthSClick;

        m_InputController.OnRotateR_Tap -= GridController.RotateRightClick;
        m_InputController.OnRotateL_Tap -= GridController.RotateLeftClick;
        m_InputController.OnPlaceBuilding_Tap -= GridController.PlaceBuildingClick;
    }






}
