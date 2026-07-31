using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;
using System.Text;
using System.Linq;
using System;
using MoonSharp.Interpreter;
using LuaCoroutine = MoonSharp.Interpreter.Coroutine;
using System.Collections;


namespace DullVersion {
    public class ComputerController : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private ManipulatorController currentManipulatorController; //TEMP
        [SerializeField] private ManipulatorController[] manipulatorControllers;

        const string GRAB_OPERATION = "Grab";
        const string RELEASE_OPERATION = "Release";
        const string GRAB_WITH_WAIT_OPERATION = "GrabConfident";


        const string WAIT_OPERATION = "Wait";
        const string ROTATE_JOINT_OPERATION = "RotateJoint";
        const string WAIT_ITEM_OPERATION = "WaitFor";

        private Dictionary<ManipulatorController, LuaRunner> _manipultorRunnners = new Dictionary<ManipulatorController, LuaRunner>();

        

        private string[] operationsWithoutArgs = new string[] 
        { 
            GRAB_OPERATION,
            RELEASE_OPERATION,
            GRAB_WITH_WAIT_OPERATION
        };
        private string[] operationsWithArgs = new string[]
        {
            ROTATE_JOINT_OPERATION,
            WAIT_OPERATION,
            WAIT_ITEM_OPERATION
        };

        private Dictionary<string, Func<Operation>> operationTable = new Dictionary<string, Func<Operation>>()
        {
            { GRAB_OPERATION, () => new WristGrabOperation() },
            { RELEASE_OPERATION, () => new WristReleaseOperation() },
            { GRAB_WITH_WAIT_OPERATION,  () => new GrabWaitAItemOperation() },
            { WAIT_OPERATION, () => new WaitOperation() },
            { ROTATE_JOINT_OPERATION, () => new JointRotationOperation() },
            { WAIT_ITEM_OPERATION, () => new WaitItemOperation() }
        };
        
        public void SubmitedInputField()
        {
            currentManipulatorController.SetAndStartScript(_inputField.text);
            
            //string pattern = @"(?<op>[A-Za-z_]\w*)\((?<args>[^()]*)\)";
            //Regex regex = new Regex(pattern, RegexOptions.Compiled);
            //foreach (string raw in raws)
            //{
            //    Match match = regex.Match(raw);
            //    if (!match.Success) continue;

            //    string operationName = match.Groups["op"].Value;
            //    string argsString = match.Groups["args"].Value;

            //    if (operationsWithoutArgs.Contains(operationName))
            //    {
            //        if (!operationTable.TryGetValue(operationName, out Func<Operation> operationFunc)) continue;

            //        manipulatorController.AddOperation(operationFunc());

            //    }
            //    else if (operationsWithArgs.Contains(operationName))
            //    {
            //        if (!operationTable.TryGetValue(operationName, out Func<Operation> operationFunc)) continue;

            //        string[] args = string.IsNullOrWhiteSpace(argsString) ? new string[0] :
            //            argsString.Split(',').Select(x => x.Trim()).ToArray();
            //        Operation operation = operationFunc();


            //        if (!operation.TryInit(args))
            //        {
            //            Debug.LogError($"Аргументы метода {operationName} не корректны");
            //            continue;
            //        }
            //        manipulatorController.AddOperation(operation);
            //        /*
            //        switch(operationName)
            //        {
            //            case ROTATE_JOINT_OPERATION:
            //                manipulatorController.RotateJoint(int.Parse(args[0]), float.Parse(args[1]));
            //                break;
            //        }*/
            //    }
            //    else
            //    {
            //        Debug.LogError($"Метода {operationName} не существует");
            //    }
            //}


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