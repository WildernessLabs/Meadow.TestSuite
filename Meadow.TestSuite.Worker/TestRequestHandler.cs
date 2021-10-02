using Meadow.Foundation.Web.Maple.Server;
using Meadow.Foundation.Web.Maple.Server.Routing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace MeadowApp
{
    public class TestRequestHandler : RequestHandlerBase
    {
        public override bool IsReusable => true;

        // TODO: pull from config
        public const int FileBufferSize = 4096;

        [HttpGet("/")]
        public IActionResult GetInfo()
        {
            return new JsonResult(
                new
                {
                    Name = "Meadow TestSuite",
                    Version = Assembly.GetEntryAssembly().GetName().Version.ToString(3)
                });
        }

        [HttpGet("tests")]
        public void GetTests()
        {
            Console.WriteLine("GET Tests");
        }

        [HttpGet("assemblies")]
        public IActionResult GetAssemblies()
        {
            var list = new List<TestAssemblyInfo>();

            Console.WriteLine("GET Assemblies");

            return new JsonResult(list);
        }

        [HttpPut("assemblies/{name}")]
        public IActionResult PutAssembly(string name)
        {
            Console.WriteLine($"PUT assembly '{name}'");

            var destination = Worker.Config.TestAssemblyFolder;
            var path = Path.Combine(destination, name);

            if(!Path.HasExtension(path))
            {
                path += ".dll";
            }

            var fi = new FileInfo(path);
            Console.WriteLine($"File '{fi.FullName}'");

            if (fi.Exists)
            {
                Console.WriteLine($"Deleting existing assembly");
                fi.Delete();
            }

            var buffer = new byte[FileBufferSize];

            var size = 0;

            using (var reader = new BinaryReader(Context.Request.InputStream))
            using (var writer = fi.OpenWrite())
            {
                int read = 0;
                do
                {
                    read = reader.Read(buffer, 0, buffer.Length);
                    writer.Write(buffer, 0, read);
                    size += read;
                } while (read > 0);
            }

            Console.WriteLine($"File size is {size} bytes");

            // TODO: respond with name and version

            // TODO: tell the framework to load this assembly

            return new OkResult();
        }
    }

    public class TestAssemblyInfo
    {
        public string Name { get; set; }
        public string Version { get; set; }
    }
}