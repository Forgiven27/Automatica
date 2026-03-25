using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace DullVersion {
    public class ComputerController : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private ManipulatorController manipulatorController;

        private void OnEnable()
        {
            _inputField.onSubmit.AddListener(SubmitedInputField);
        }
        private void OnDisable()
        {
            _inputField.onSubmit.RemoveListener(SubmitedInputField);
        }
        void Start()
        {
            
        }

        void SubmitedInputField(string text)
        {
            string[] splited = text.Split();
            float arg = 0;
            int index = 0;
            if (splited.Length > 1)
            {
                 arg = float.Parse(splited[1]);
            }
            if (splited.Length > 2)
            {
                index = int.Parse(splited[2]);
            }
            switch (splited[0].ToLowerInvariant())
            {
                case "rotate":
                    manipulatorController.RotateJoint(index, arg);
                    break;
                case "grab":
                    manipulatorController.Grab();
                    break;
                case "release":
                    manipulatorController.Release();
                    break;
                case "ironhigh":
                    ItemFilter filter = new ItemFilter()
                    {
                        qualityWhiteList = new List<ItemQuality>() { ItemQuality.HighQuality},
                        itemsWhiteList = new List<Item>() { Item.Iron}
                    };
                    manipulatorController.SetFilter(filter);
                    break;
                case "grabwait":
                    manipulatorController.EnableScaner();
                    break;

            }
        }

      
    }
}