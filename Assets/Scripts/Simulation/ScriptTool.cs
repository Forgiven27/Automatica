using Simulator;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
public static class ScriptTool
{
    public static bool TryConvertTextToString(string text, out Queue<ManipulatorCommandBuffer> commands)
    {
        commands = new Queue<ManipulatorCommandBuffer>();

        string pattern = @"(\w+)\s*\(([^)]*)\)";
        MatchCollection matches = Regex.Matches(text, pattern);
        bool flag;
        foreach (Match match in matches)
        {
            string commandName = match.Groups[1].Value;
            string argsString = match.Groups[2].Value;

            flag = false;

            // Разделяем аргументы по запятым
            string[] args = argsString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < args.Length; i++)
            {
                args[i] = args[i].Trim(); 
            }
            var command = new ManipulatorCommandBuffer();
            
            switch (commandName)
            {
                case "Sleep":
                    command.sleep = Convert.ToInt32(args[0]);
                    flag = true;
                    break;
                case "SetAngle":
                    int i = Convert.ToInt32(args[0]);
                    float angle = float.Parse(args[1]);
                    command.TargetJointAngles[i] = angle;
                    flag = true;
                    break;
                case "SetAngleBase":
                    command.TargetBaseYaw = float.Parse(args[0]);
                    flag = true;
                    break;
                case "Grab":
                    command.RequestGrab = true;
                    flag = true;
                    break;
                case "Release":
                    command.RequestRelease = true;
                    flag = true;
                    break;

            }

            if (flag) commands.Enqueue(command);
        }
        if (commands.Count > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
            
    }
}
