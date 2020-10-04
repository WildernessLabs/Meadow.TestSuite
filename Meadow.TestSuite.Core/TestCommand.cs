﻿using ProtoBuf;

namespace Meadow.TestsSuite
{
    public class UplinkFileCommand : TestCommand
    {
        public UplinkFileCommand()
        {
            CommandType = CommandType.UplinkFile;
        }

        public string Destination { get; set; }
        public string FileData { get; set; }
    }

    public class TestCommand
    {
        public CommandType CommandType { get; set; }
    }
}