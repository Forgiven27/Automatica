using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class UIPlayerAnimations : MonoBehaviour
{
    VisualElement m_rootElement;
    InputController m_inputController;


    void Start()
    {
        m_rootElement = GetComponent<UIDocument>().rootVisualElement;
        m_inputController = GetComponentInParent<InputController>();
        ConnectUIAnimation();
    }

    void ConnectUIAnimation()
    {
        if (m_rootElement == null || m_inputController == null) return;
        VisualElement veFisrtS = m_rootElement.Q<VisualElement>("ve_slot1");
        VisualElement veSecondS = m_rootElement.Q<VisualElement>("ve_slot2");
        VisualElement veThirdS = m_rootElement.Q<VisualElement>("ve_slot3");
        VisualElement veForthS = m_rootElement.Q<VisualElement>("ve_slot4");
        VisualElement veFifthS = m_rootElement.Q<VisualElement>("ve_slot5");

        m_inputController.OnFirstS_Tap += (async() => await ClickAnimation(veFisrtS));
        m_inputController.OnSecondS_Tap += (async () => await ClickAnimation(veSecondS));
        m_inputController.OnThirdS_Tap += (async () => await ClickAnimation(veThirdS));
        m_inputController.OnForthS_Tap += (async () => await ClickAnimation(veForthS));
        m_inputController.OnFifthS_Tap += (async () => await ClickAnimation(veFifthS));

    }

    async UniTask ClickAnimation(VisualElement ve)
    {
        if (ve == null) return;
        ve.AddToClassList("inventory-cell__pressed");
        await UniTask.Delay(500);
        ve.RemoveFromClassList("inventory-cell__pressed");
        await UniTask.Delay(250);
    }


    
}
