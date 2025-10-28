using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System;
using UnityEditor;

public class UIStartSceneController : MonoBehaviour
{

    VisualElement m_RootElement;
    VisualElement m_AdditionalWindowContainer;
    readonly string OPEN_WINDOW_CLASS = "window--open";
    readonly string CLOSE_WINDOW_CLASS = "window--close";
    InputSystem_Actions m_InputActions;
    bool m_IsInputListen = false;
    void Start()
    {
        m_InputActions = new InputSystem_Actions();
        m_InputActions.Enable();

        m_RootElement = GetComponent<UIDocument>().rootVisualElement;
        InitStartMenuButtons();
    }

    void InitStartMenuButtons()
    {
        if (m_RootElement == null) return;
        Button startButton = m_RootElement.Q<Button>("start");
        Button settingsButton = m_RootElement.Q<Button>("settings");
        Button exitButton = m_RootElement.Q<Button>("exit");

        m_AdditionalWindowContainer = m_RootElement.Q<VisualElement>("additional-menu");
        m_AdditionalWindowContainer.RemoveFromClassList(OPEN_WINDOW_CLASS);
        m_AdditionalWindowContainer.AddToClassList(CLOSE_WINDOW_CLASS);

        startButton.clicked += StartButton_clicked;
        settingsButton.clicked += SettingsButton_clicked;
        exitButton.clicked += ExitButton_clicked;
    }

    private async void StartButton_clicked()
    {
        await SceneManager.LoadSceneAsync(1);
    }

    private void SettingsButton_clicked()
    {
        m_AdditionalWindowContainer.RemoveFromClassList(CLOSE_WINDOW_CLASS);
        m_AdditionalWindowContainer.AddToClassList(OPEN_WINDOW_CLASS);

        Tab controlTab = m_AdditionalWindowContainer.Q<Tab>("control__tab");
        Tab soundTab = m_AdditionalWindowContainer.Q<Tab>("sound__tab");
        Tab videoTab = m_AdditionalWindowContainer.Q<Tab>("video__tab");

        InitControlTab(controlTab);
    }

    void InitControlTab(Tab controlTab)
    {
        List<VisualElement> containers = controlTab.Query<VisualElement>(className: "logic__container").ToList();
        Debug.LogWarning("Количество контейнеров " + containers.Count);
        foreach (VisualElement container in containers)
        {
            switch (container.name)
            {
                case "building__container":
                    InitBuildingControl(container);
                    break;
                case "move__container":
                    InitMoveControl(container);
                    break;
                default:
                    Debug.LogWarning("Имя контейнера в табе "+container.name);
                    break;
            }
        }
    }
    void InitBuildingControl(VisualElement container)
    {
        
        List<VisualElement> rebinders = container.Query<VisualElement>(null, "rebinder").ToList();
        foreach(var rebinder in rebinders)
        {
            switch (rebinder.name.Remove(0,10))
            {
                case "first-slot":
                    Debug.Log("FirstSlot switch ");
                    Button button = rebinder.Q<Button>();
                    InputAction inputAction = m_InputActions.Player.FirstSlot;
                    SetSwitchControlName(button, inputAction);
                    button.clicked += (() => SwitchControl_clicked(button, inputAction));
                    break;
                default:
                    
                    break;
            }
        }
        
    }
    void InitMoveControl(VisualElement container)
    {

    }

    void SetSwitchControlName(Button button, InputAction inputAction)
    {
        string actionName = InputControlPath.ToHumanReadableString(inputAction.bindings[0].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
        
        if (button.text == actionName) return;
        else button.text = actionName;
    }

    void SwitchControl_clicked(Button button,InputAction inputAction)
    {
        if (m_IsInputListen) return;
        Debug.Log("SwitchControl_clicked");
        string actionName = InputControlPath.ToHumanReadableString(inputAction.bindings[0].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
        inputAction.Disable();
        var rebindingOperation = inputAction.PerformInteractiveRebinding()
                                            .WithControlsExcluding("<Mouse>/position")
                                            .WithControlsExcluding("<Mouse>/delta")
                                            .WithControlsHavingToMatchPath("<Keyboard>")
                                            .WithCancelingThrough("<Keyboard>/escape")
                                            .OnComplete(_ =>
                                            {
                                                Debug.Log("CompeleteBinding");
                                                
                                                inputAction.ApplyBindingOverride(_.action.bindings[0]);
                                                SetSwitchControlName(button, inputAction);
                                                //m_InputActions.SaveBindingOverridesAsJson();
                                                
                                                
                                                _.Dispose();
                                                inputAction.Enable();
                                            });
        rebindingOperation.Start();
        //m_IsInputListen = true;

    }



    
    void Update()
    {
        /*
        if (m_IsInputListen)
        {
            Debug.Log(Input.inputString);
        }*/
    }

    private void ExitButton_clicked()
    {
        Debug.Log("Выход из игры...пока не реализован");
    }
}
