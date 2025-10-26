using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;
using UnityEngine.UIElements;
public class UIInputHintController : MonoBehaviour
{
    enum ActionMaps
    {
        Player,
        UI
    }
    InputSystem_Actions m_InputActions;
    VisualElement m_rootElement;
    void Start()
    {
        m_InputActions = new();
        m_InputActions.Enable();

        m_rootElement = GetComponent<UIDocument>().rootVisualElement;
        UpdateHints();
    }

    public void UpdateHints()
    {
        if (m_rootElement == null) return;
        Label labelFirstS = m_rootElement.Q<VisualElement>("ve_slot1").Q<Label>("hint_label");
        Label labelSecondS = m_rootElement.Q<VisualElement>("ve_slot2").Q<Label>("hint_label");
        Label labelThirdS = m_rootElement.Q<VisualElement>("ve_slot3").Q<Label>("hint_label");
        Label labelForthS = m_rootElement.Q<VisualElement>("ve_slot4").Q<Label>("hint_label");
        Label labelFifthS = m_rootElement.Q<VisualElement>("ve_slot5").Q<Label>("hint_label");
        string bindingFirstS = m_InputActions.Player.FirstSlot.bindings[0].effectivePath;
        string bindingSecondS = m_InputActions.Player.SecondSlot.bindings[0].effectivePath;
        string bindingThirdS = m_InputActions.Player.ThirdSlot.bindings[0].effectivePath;
        string bindingForthS = m_InputActions.Player.ForthSlot.bindings[0].effectivePath;
        string bindingFifthS = m_InputActions.Player.FifthSlot.bindings[0].effectivePath;
        SetControlHintText(labelFirstS, bindingFirstS);
        SetControlHintText(labelSecondS, bindingSecondS);
        SetControlHintText(labelThirdS, bindingThirdS);
        SetControlHintText(labelForthS, bindingForthS);
        SetControlHintText(labelFifthS, bindingFifthS);

    }
    void SetControlHintText(Label label, string pathBinding)
    {
        if (label != null)
        {
            label.text = InputControlPath.ToHumanReadableString(pathBinding, InputControlPath.HumanReadableStringOptions.OmitDevice);
        }
        else
        {
            Debug.LogWarning("Label is not found");
        }
    }

    void Update()
    {
            
    }
}
