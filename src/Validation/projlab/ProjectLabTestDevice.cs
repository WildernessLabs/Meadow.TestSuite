using Meadow.Devices;

namespace Meadow.Validation
{
    public class ProjectLabTestDevice : MeadowTestDevice
    {
        public IProjectLabHardware ProjectLab { get; }

        public ProjectLabTestDevice(IMeadowDevice device, IProjectLabHardware projectLab)
        : base(device)
        {
            ProjectLab = projectLab;
        }
    }
}