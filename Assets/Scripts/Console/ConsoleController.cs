using UnityEngine;
using UnityEngine.UIElements;

public class ConsoleController : MonoBehaviour
{
    public UIDocument consoleDocument;
    VisualElement root;
    PanelSettings consolePanelSettings;
    Button interactionButton;
    bool IsInteractionEnable;
    void Start()
    {
        consolePanelSettings = consoleDocument.panelSettings;

        root = GetComponent<UIDocument>().rootVisualElement;

        interactionButton = root.Q<Button>();
        interactionButton.SetEnabled(false);
        IsInteractionEnable = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interactionButton.SetEnabled(true);
            IsInteractionEnable=true;
            print("Player Enter");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interactionButton.SetEnabled(false);
            IsInteractionEnable = false;
            print("Player Exit");
        }
    }

    private void Update()
    {
        if (IsInteractionEnable) {
            if (Input.GetKeyDown(KeyCode.E))
            {
                print("Pressed E");
            }
        }
    }


}
