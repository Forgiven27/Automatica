using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIBuildingRaw : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameTextField;
    [SerializeField] private TextMeshProUGUI _idTextField;
    [SerializeField] private ClickHandler _clickHandler;

    public Action<uint> OnClicked;

    private void OnEnable()
    {
        _clickHandler.OnClick += (
            (evtData) => { Debug.Log("Клик обработался"); OnClicked?.Invoke(uint.Parse(_idTextField.text)); });
    }

    private void OnDisable()
    {
        _clickHandler.OnClick -= (
            (evtData) => { Debug.Log("Клик обработался"); OnClicked?.Invoke(uint.Parse(_idTextField.text)); });
    }

    public void Init(string name, string id)
    {
        _nameTextField.text = name;
        _idTextField.text = id;
    }

    public void SetName(string name)
    {
        _nameTextField.text = name;
    }

    public void SetId(string id)
    {
        _idTextField.text = id;
    }



    

}
