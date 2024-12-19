using System;
using System.Threading.Tasks;

namespace SoakTests;

public interface ISoakTest
{
    /// <summary>
    /// Setup the conditions for the test.
    /// </summary>
    void Initialize(SoakTestSettings config);

    /// <summary>
    /// Execute the test synchronously.
    /// </summary>
    Task Execute();

    /// <summary>
    /// Tear down the test restoring the system to a state where other tests can be run.
    /// </summary>
    void Teardown();
}
