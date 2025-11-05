using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using System.Collections.Generic;

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
    
    private readonly float m_buttonCooldown = 0.1f;
    private Dictionary<Button, bool> buttonsActiveState;

    CommonPlacer m_CommonPlacer;
    SplinePlacer m_SplinePlacer;
    public Building manipulator;



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
            m_SplinePlacer.CreateBuildingByIndex(0, true);
            buttonsActiveState[currentButton] = false;
            await TimerStandard(currentButton);
        }
    }


    public async void ThirdSClick()
    {
        Button currentButton = Button.ThirdS;
        if (buttonsActiveState[currentButton])
        {
            
            buttonsActiveState[currentButton] = false;
            await TimerStandard(currentButton);
        }
    }
    public async void ForthSClick()
    {
        Button currentButton = Button.ForthS;
        if (buttonsActiveState[currentButton])
        {
            
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
