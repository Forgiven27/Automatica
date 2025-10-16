using UnityEngine;

public class UIPlayerController : MonoBehaviour
{
    InputController m_InputController;
    UIInventoryController m_InventoryController;
    public GridController GridController;


    void Start()
    {
        m_InputController = GetComponent<InputController>();
        m_InventoryController = GetComponent<UIInventoryController>();
    }

    
    void Update()
    {
        m_InventoryController.SetInventoryButtonState(
            m_InputController.IsFirstSClick(),
            m_InputController.IsSecondSClick(),
            m_InputController.IsThirdSClick(),
            m_InputController.IsForthSClick(),
            m_InputController.IsFifthSClick()
            );
        GridController.SetControlState(m_InputController.IsPlaceBuilding(),
            m_InputController.IsRotateLeftClick(),
            m_InputController.IsRotateRightClick());
    }



}
