using System.Collections.Generic;
using Simulator;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIEntitiesList : MonoBehaviour
{
    [SerializeField] private ScrollRect _scroll;
    [SerializeField] private GameObject _scrollElement;
    [SerializeField] private Button _buttonUpdate;
    [SerializeField] private Button _buttonUploadScript;
    [SerializeField] private TMP_InputField _scriptInputField;
    [SerializeField] private TextMeshProUGUI _currentIdTextField;

    private List<GameObject> elements = new List<GameObject>();
    uint currentID;
    private void OnEnable()
    {
        UpdateEntitiesList();
        if(currentID == 0)
        {
            _scriptInputField.readOnly = true;
        }
    }


    public void UpdateEntitiesList()
    {
        Debug.Log("Обновление списка");
        foreach (var element in elements)
        {
            element.GetComponent<UIBuildingRaw>().OnClicked -= UpdateScriptText;
            Destroy(element);
        }
        elements.Clear();
        uint[] ids = SimulationAPI.GetAllManipulator();

        foreach (uint id in ids)
        {
            GameObject element = Instantiate(_scrollElement, _scroll.content);

            var buildingRaw = element.GetComponent<UIBuildingRaw>();
            buildingRaw.Init("Manipulator", id.ToString());
            buildingRaw.OnClicked += UpdateScriptText;
            elements.Add(element);
        }
    }

    void UpdateScriptText(uint manipulatorID)
    {
        currentID = manipulatorID;
        _currentIdTextField.text = currentID.ToString();
        _scriptInputField.text = SimulationAPI.GetScriptText(manipulatorID);
        _scriptInputField.readOnly = false;
    }

    public void LoadScript()
    {
        string text = _scriptInputField.text;
        Debug.Log("Загрузка скрипта");
        var c = new ManipulatorSetScriptCommand()
        {
            manipulatorID = currentID,
            scriptText = text
        };
        SimulationAPI.Request<ManipulatorSetScriptCommand>(c);
    }

    private void OnDisable()
    {
        _buttonUpdate.onClick.RemoveListener(UpdateEntitiesList);
    }

}
