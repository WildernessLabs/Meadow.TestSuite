using Meadow.Devices;

namespace Meadow.Validation
{
    public class ProjectLabTestDevice : MeadowTestDevice
    {
        public ProjectLab ProjectLab { get; }

        public ProjectLabTestDevice(IMeadowDevice device, ProjectLab projectLab)
        : base(device)
        {
            ProjectLab = projectLab;
        }
    }
}