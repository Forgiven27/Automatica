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

    public GridController gridController;
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
    }
    
    public async void FirstSClick()
    {
        Button currentButton = Button.FirstS;
        if (buttonsActiveState[currentButton])
        {
            gridController.CreateBuilding(manipulator);
            buttonsActiveState[currentButton] = false;
            await TimerStandard(currentButton);
        }
    }
    public async void SecondSClick()
    {
        Button currentButton = Button.SecondS;
        if (buttonsActiveState[currentButton])
        {
            
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
