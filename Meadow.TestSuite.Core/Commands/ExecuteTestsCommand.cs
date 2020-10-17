﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Meadow.TestSuite
{
    public class ExecuteTestsCommand : TestCommand
    {
        public string[] TestNames { get; set; }

        public ExecuteTestsCommand(IEnumerable<string> testNames)
            : this()
        {
            TestNames = testNames.ToArray();
        }

        public ExecuteTestsCommand()
        {
            CommandType = CommandType.ExecuteTests;
        }

        public override void Execute(IWorker worker)
        {
            if (TestNames == null)
            {
                Result = "TestNames is null";
            }
            else if (TestNames.Length == 0)
            {
                Result = "TestNames is empty";
            }
            else
            {
                var list = new List<string>();

                foreach (var t in TestNames)
                {
                    Console.WriteLine($"Running {t}");
                    list.Add($"Running {t}");
                    worker.ExecuteTest(t);
                }

                Result = list.ToArray();
            }
        }
    }
}