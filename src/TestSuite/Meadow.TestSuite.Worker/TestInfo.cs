using System.Reflection;

namespace Meadow.TestSuite
{
    public class TestInfo
    {
        public string AssemblyName { get; set; }
        public string TypeName { get; set; }
        public string TestName { get; set; }
        public MethodInfo TestMethod { get; set; }
        public ConstructorInfo TestConstructor { get; set; }
        public PropertyInfo DeviceProperty { get; set; }

        public string ID
        {
            get => $"{AssemblyName}.{TypeName}.{TestName}";
        }
    }
}