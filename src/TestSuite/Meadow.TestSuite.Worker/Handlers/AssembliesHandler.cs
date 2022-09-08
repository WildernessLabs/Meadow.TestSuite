using Meadow.Foundation.Web.Maple;
using Meadow.Foundation.Web.Maple.Routing;
using System.IO;

namespace MeadowApp
{
    public class AssembliesHandler : RequestHandlerBase
    {
        public override bool IsReusable => true;

        // TODO: pull from config
        public const int FileBufferSize = 4096;

        [HttpGet]
        public IActionResult GetAssemblies()
        {
            MeadowApp.Worker.Logger?.Debug($"REST: AssembliesHandler.GetAssemblies()");

            // TODO: include versions            

            return new JsonResult(MeadowApp.Worker.Registry.GetAssemblies());
        }

        [HttpDelete("{name}")]
        public IActionResult DeleteFile(string name)
        {
            MeadowApp.Worker.Logger?.Debug($"REST: AssembliesHandler.DeleteFile({name})");

            var destination = MeadowApp.Worker.Config.TestAssemblyFolder;
            if (!Directory.Exists(destination))
            {
                MeadowApp.Worker.Logger?.Debug($"Test assembly folder '{destination}' not found");
                return this.NotFound();
            }

            var path = Path.Combine(destination, name);

            var fi = new FileInfo(path);

            if (!fi.Exists)
            {
                MeadowApp.Worker.Logger?.Debug($"File '{name}' not found");
                return this.NotFound();
            }

            MeadowApp.Worker.Logger?.Debug($"Deleting file");
            fi.Delete();

            return this.Ok();
        }

        [HttpPut("{name}")]
        public IActionResult PutAssembly(string name)
        {
            MeadowApp.Worker.Logger?.Debug($"REST: AssembliesHandler.PutAssembly({name})");

            var destination = MeadowApp.Worker.Config.TestAssemblyFolder;
            if (!Directory.Exists(destination))
            {
                MeadowApp.Worker.Logger?.Debug($"Creating test assembly folder '{destination}'");
                Directory.CreateDirectory(destination);
            }

            var path = Path.Combine(destination, name);

            if (!Path.HasExtension(path))
            {
                path += ".dll";
            }

            var fi = new FileInfo(path);
            MeadowApp.Worker.Logger?.Info($"File '{fi.FullName}'");

            if (fi.Exists)
            {
                MeadowApp.Worker.Logger?.Debug($"Deleting existing assembly");
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

            MeadowApp.Worker.Logger?.Debug($"File size is {size} bytes");

            // tell the framework to load this assembly
            MeadowApp.Worker.Registry.RegisterAssembly(path);

            // TODO: respond with name and version

            return new OkResult();
        }
    }
}