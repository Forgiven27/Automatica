using UnityEngine;
using UnityEngine.InputSystem;

public class UIPlayerController : MonoBehaviour
{

    [SerializeField] private UISelectBuildPanel _panelSelectBuild;
    [SerializeField] private UIBuildBoard _buildBoard;

    public UISelectBuildPanel GetSelectBuildPanel => _panelSelectBuild;
    public UIBuildBoard GetBuildBoard => _buildBoard;


    public void InitUI(BuildBoard buildBoard, InputSystem_Actions inputActions)
    {
        string bindingFirstS = inputActions.Player.FirstSlot.bindings[0].effectivePath;
        string bindingSecondS = inputActions.Player.SecondSlot.bindings[0].effectivePath;
        string bindingThirdS = inputActions.Player.ThirdSlot.bindings[0].effectivePath;
        string bindingForthS = inputActions.Player.ForthSlot.bindings[0].effectivePath;
        string bindingFifthS = inputActions.Player.FifthSlot.bindings[0].effectivePath;

        string hintFirstButton = InputControlPath.ToHumanReadableString(bindingFirstS, InputControlPath.HumanReadableStringOptions.OmitDevice);
        string hintSecondButton = InputControlPath.ToHumanReadableString(bindingSecondS, InputControlPath.HumanReadableStringOptions.OmitDevice);
        string hintThirdButton = InputControlPath.ToHumanReadableString(bindingThirdS, InputControlPath.HumanReadableStringOptions.OmitDevice);
        string hintForthButton = InputControlPath.ToHumanReadableString(bindingForthS, InputControlPath.HumanReadableStringOptions.OmitDevice);
        string hintFifthButton = InputControlPath.ToHumanReadableString(bindingFifthS, InputControlPath.HumanReadableStringOptions.OmitDevice);

        buildBoard.firstCell.currentBuilding = buildBoard.firstCell.currentBuilding.buildingObject == null ? buildBoard.firstCell.buildings[0] :
            buildBoard.firstCell.currentBuilding;
        buildBoard.secondCell.currentBuilding = buildBoard.secondCell.currentBuilding.buildingObject == null ? buildBoard.secondCell.buildings[0] :
            buildBoard.secondCell.currentBuilding;


        _buildBoard.InitButton(buildBoard.firstCell.currentBuilding, hintFirstButton);
        _buildBoard.InitButton(buildBoard.secondCell.currentBuilding, hintSecondButton);
        _buildBoard.InitButton(buildBoard.thirdCell.currentBuilding, hintThirdButton);
        _buildBoard.InitButton(buildBoard.forthCell.currentBuilding, hintForthButton);
        _buildBoard.InitButton(buildBoard.fifthCell.currentBuilding, hintFifthButton);


        _buildBoard.gameObject.SetActive(true);
    }
}
