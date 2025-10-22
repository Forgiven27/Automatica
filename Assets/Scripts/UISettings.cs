using System;
using Unity.Properties;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "UISettings", menuName = "Scriptable Objects/UISettings")]
public class UISettings : ScriptableObject
{
    public InputActionAsset inputActionsAsset;
    [SerializeField, DontCreateProperty] string myString;
    [SerializeField, CreateProperty]
    public string ButtonS1
    {
        get
        {
            return name;
        }
        set
        {
            name = value;
        }
    }
    string Mthod()
    {
        if (inputActionsAsset != null)
        {
            var actions = inputActionsAsset.FindActionMap("Player").actions;
            return "";
        }
        else
        {
            return "No InputActionAsset";
        }
    }
}
