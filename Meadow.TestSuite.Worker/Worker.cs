﻿using Meadow.Devices;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TestSuite.Unit.tests")]

namespace MeadowApp
{
    public class Worker
    {
        public F7Micro Device { get; }

        public Worker(F7Micro device)
        {
            Device = device;
        }
    }
}