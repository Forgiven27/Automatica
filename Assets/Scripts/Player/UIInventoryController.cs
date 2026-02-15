using Cysharp.Threading.Tasks;
using Simulator;
using System.Collections.Generic;
using UnityEngine;

public class UIInventoryController : MonoBehaviour
{
    enum Button
    {
        FirstS,
        SecondS,
        ThirdS,
        ForthS,
        FifthS,
    }
    
    private readonly float m_buttonCooldown = 0.3f;
    private Dictionary<Button, bool> buttonsActiveState;

    CommonPlacer m_CommonPlacer;
    SplinePlacer m_SplinePlacer;
    public BuildingInfo manipulator;

    public FactoryDescription factoryDescription; //DELETE

    private void Start()
    {
        buttonsActiveState = new Dictionary<Button, bool>() {
            {Button.FirstS, true},
            {Button.SecondS, true},
            {Button.ThirdS, true},
            {Button.ForthS, true},
            {Button.FifthS, true},
        };
        m_CommonPlacer = GetComponent<CommonPlacer>();
        m_SplinePlacer = GetComponent<SplinePlacer>();
    }
    
    public async void FirstSClick()
    {
        Button currentButton = Button.FirstS;
        if (buttonsActiveState[currentButton])
        {
            m_CommonPlacer.CreateBuilding(manipulator);
            buttonsActiveState[currentButton] = false;
            await TimerStandard(currentButton);
        }
    }
    public async void SecondSClick()
    {
        Button currentButton = Button.SecondS;
        if (buttonsActiveState[currentButton])
        {
            m_SplinePlacer.CreateBuilding(true);
            buttonsActiveState[currentButton] = false;
            await TimerStandard(currentButton);
        }
    }


    public async void ThirdSClick()
    {
        Button currentButton = Button.ThirdS;
        if (buttonsActiveState[currentButton])
        {

            Ray rayMouse = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(rayMouse, out RaycastHit hitInfo, 100, 1 << LayerMask.NameToLayer("BuildingSurface")))
            {
                /*
                Vector3 newPosition = new Vector3(hitInfo.point.x + (hitInfo.point.x % step < step / 2 ? -hitInfo.point.x % step : step - hitInfo.point.x % step),
                    hitInfo.point.y,
                    hitInfo.point.z + (hitInfo.point.z % step < step / 2 ? -hitInfo.point.z % step : step - hitInfo.point.z % step))
                    + worldGrid.null_position;
                */

                Debug.Log("Отправлен запрос");
                FactoryCreateCommand c = new FactoryCreateCommand()
                {
                    factoryType = FactoryType.ExportImport10,
                    factoryDescription = factoryDescription,
                    position = hitInfo.point,
                    rotation = Quaternion.identity
                };
                SimulationAPI.Request<FactoryCreateCommand>(c);
            }
            buttonsActiveState[currentButton] = false;
            await TimerStandard(currentButton);
        }
    }
    public async void ForthSClick()
    {
        Button currentButton = Button.ForthS;
        if (buttonsActiveState[currentButton])
        {
            Ray rayMouse = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(rayMouse, out RaycastHit hitInfo, 100, 1 << LayerMask.NameToLayer("BuildingSurface")))
            {
                /*
                Vector3 newPosition = new Vector3(hitInfo.point.x + (hitInfo.point.x % step < step / 2 ? -hitInfo.point.x % step : step - hitInfo.point.x % step),
                    hitInfo.point.y,
                    hitInfo.point.z + (hitInfo.point.z % step < step / 2 ? -hitInfo.point.z % step : step - hitInfo.point.z % step))
                    + worldGrid.null_position;
                */

                Debug.Log("Отправлен запрос");
                var c = new ConveyorCreateCommand()
                {
                    startPosition = hitInfo.point,
                    endPosition = hitInfo.point + new Vector3(0,0,10),
                    stepsOfContainer = 10,
                };
                SimulationAPI.Request<ConveyorCreateCommand>(c);
            }
            buttonsActiveState[currentButton] = false;
            await TimerStandard(currentButton);
        }
    }
    public async void FifthSClick()
    {
        Button currentButton = Button.FifthS;
        if (buttonsActiveState[currentButton])
        {
            
            buttonsActiveState[currentButton] = false;
            await TimerStandard(currentButton);
        }
    }
    private async UniTask TimerStandard(Button button)
    {
        await UniTask.WaitForSeconds(m_buttonCooldown);
        buttonsActiveState[button] = true;
    }


    




    

}
