using Meadow.Devices;

namespace Meadow.Validation
{
    public class ProjectLabTestDevice : F7TestDevice
    {
        public ProjectLab ProjectLab { get; }

        public ProjectLabTestDevice(IMeadowDevice device, ProjectLab projectLab)
        : base(device)
        {
            ProjectLab = projectLab;
        }
    }
}