using Meadow.Foundation.Web.Maple.Server;
using Meadow.Foundation.Web.Maple.Server.Routing;
using System;
using System.Collections.Generic;
using System.IO;

namespace MeadowApp
{
    public class AssembliesHandler : RequestHandlerBase
    {

        // TODO: pull from config
        public const int FileBufferSize = 4096;

        [HttpGet]
        public IActionResult GetAssemblies()
        {
            Console.WriteLine("GET Assemblies");

            // TODO: include versions            

            return new JsonResult(MeadowApp.Worker.Registry.GetAssemblies());
        }

        [HttpPut("{name}")]
        public IActionResult PutAssembly(string name)
        {
            MeadowApp.Worker.Logger?.Info($"PUT assembly '{name}'");

            var destination = MeadowApp.Worker.Config.TestAssemblyFolder;
            if (!Directory.Exists(destination))
            {
                MeadowApp.Worker.Logger?.Info($"Creating test assembly folder '{destination}'");
                Directory.CreateDirectory(destination);
            }

            var path = Path.Combine(destination, name);

            if(!Path.HasExtension(path))
            {
                path += ".dll";
            }

            var fi = new FileInfo(path);
            MeadowApp.Worker.Logger?.Info($"File '{fi.FullName}'");

            if (fi.Exists)
            {
                MeadowApp.Worker.Logger?.Info($"Deleting existing assembly");
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

            MeadowApp.Worker.Logger?.Info($"File size is {size} bytes");

            // tell the framework to load this assembly
            MeadowApp.Worker.Registry.RegisterAssembly(path);

            // TODO: respond with name and version

            return new OkResult();
        }
    }
}