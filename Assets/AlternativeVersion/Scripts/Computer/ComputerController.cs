using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;
using System.Text;

namespace DullVersion {
    public class ComputerController : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private ManipulatorController manipulatorController;

        public void SubmitedInputField()
        {
            string text = _inputField.text;
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

            string pattern = @"(\w+)\s*\(([^)]*)\)";
            Regex regex = new Regex(pattern, RegexOptions.Multiline);
            MatchCollection matches = regex.Matches(text);
            StringBuilder sb = new StringBuilder();
            foreach (Match match in matches)
            {
                foreach (string s in match.Groups)
                {
                    Debug.Log(s);
                }
                Debug.Log("____\n");
            }
            /*
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

            }*/
        }

      
    }
}