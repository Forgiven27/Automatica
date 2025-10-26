using UnityEngine;
using UnityEngine.UIElements;

public class UIStartSceneController : MonoBehaviour
{

    VisualElement m_RootElement;
    public VisualTreeAsset m_controlSwitcher;

    void Start()
    {
        m_RootElement = GetComponent<UIDocument>().rootVisualElement;

    }




    // Update is called once per frame
    void Update()
    {
        
    }
}
