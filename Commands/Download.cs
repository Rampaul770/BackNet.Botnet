﻿using System;
using System.Collections.Generic;
using System.IO;
using Shared;

namespace Commands
{
    internal class Download : ICommand
    {
        public string name { get; } = "download";

        public string description { get; } = "Download a file from the server";

        public string syntaxHelper { get; } = "download [remoteFileName] [localFileName]";

        public bool isLocal { get; } = false;

        public List<string> validArguments { get; } = new List<string>()
        {
            "? ?"
        };


        public bool PreProcessCommand(List<string> args)
        {
            throw new NotImplementedException();
        }

        public void ClientMethod(List<string> args)
        {
            var initResult = CommandsManager.networkManager.ReadLine();
            if (initResult != "OK")
            {
                ColorTools.WriteCommandError(initResult == "NotFound" ? "The remote file doesn't exist" : "An IO exception occured");
                return;
            }

            var path = args[1];
            ColorTools.WriteCommandMessage($"Starting download of file '{args[0]}' from the server");
            
            var dataLength = int.Parse(CommandsManager.networkManager.ReadLine());

            try
            {
                using (var fs = new FileStream(path, FileMode.Create))
                {
                    CommandsManager.networkManager.NetworkStreamToStream(fs, dataLength);
                }

                ColorTools.WriteCommandSuccess("File successfully downloaded from the server");
            }
            catch (Exception)
            {
                // Delete the partially created file
                File.Delete(path);
                ColorTools.WriteCommandError("An error occured");
            }
        }

        public void ServerMethod(List<string> args)
        {
            if (!File.Exists(args[0]))
            {
                CommandsManager.networkManager.WriteLine("NotFound");
                return;
            }

            try
            {
                using (var readStream = new FileStream(args[0], FileMode.Open))
                {
                    CommandsManager.networkManager.WriteLine("OK");

                    // Send the data length first
                    CommandsManager.networkManager.WriteLine(new FileInfo(args[0]).Length.ToString());
                    CommandsManager.networkManager.StreamToNetworkStream(readStream);
                }
            }
            catch (IOException)
            {
                CommandsManager.networkManager.WriteLine("IOException");
            }
        }
    }
}
