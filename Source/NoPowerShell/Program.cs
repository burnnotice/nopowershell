﻿using System;
using System.Collections.Generic;
using NoPowerShell.Commands;
using NoPowerShell.HelperClasses;

/*
Author: @_bitsadmin
Website: https://github.com/bitsadmin
License: BSD 3-Clause
*/

namespace NoPowerShell
{
    class Program
    {
        static int Main(string[] args)
        {
            // Using reflection determine available commands
            Dictionary<Type, CaseInsensitiveList> availableCommands = ReflectionHelper.GetCommands();
            List<PSCommand> userCommands = null;

            // If no arguments are provided to the executable, show help
            if (args.Length == 0)
            {
                Console.WriteLine("== NoPowerShell v1.0 ==\r\nUrl: Website: https://github.com/bitsadmin\r\nUsage: NoPowerShell.exe [Command] [Parameters] | [Command2] [Parameters2] etc.\r\n");
                userCommands = new List<PSCommand>(1) { new GetCommandCommand(null) };
            }
            // Parse pipes in commandline arguments and commands within pipes
            else
            {
                userCommands = PipeParser.ParseArguments(args, availableCommands);
            }

            // Add output to console if no explicit output is provided
            Type lastCommand = userCommands[userCommands.Count - 1].GetType();
            bool justOutput = false;
            if (lastCommand != typeof(FormatListCommand) && lastCommand != typeof(FormatTableCommand))
                justOutput = true;

            CommandResult result = null;
            try
            {
                // Execute commands in pipeline
                foreach (PSCommand command in userCommands)
                {
                    result = command.Execute(result);
                }
            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
                return -1;
            }

            // Output to screen
            if (justOutput)
                ResultPrinter.OutputResults(result);

            return 0;
        }
    }
}
