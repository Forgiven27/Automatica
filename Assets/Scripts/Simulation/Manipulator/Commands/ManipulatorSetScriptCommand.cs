using Simulator;
using System.Collections.Generic;
using UnityEngine;

public struct ManipulatorSetScriptCommand : ISimulatorCommand
{
    public uint manipulatorID;
    public string scriptText;
    
    public void Execute(CommandContext context)
    {
        if (ScriptTool.TryConvertTextToString(scriptText, out Queue<ManipulatorCommandBuffer> scriptConverted))
        {
            context.manipulatorSim.SetScript(manipulatorID, scriptConverted, scriptText);
        }
        
    }
}
