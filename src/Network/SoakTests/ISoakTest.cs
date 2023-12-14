namespace SoakTests;

public interface ISoakTest
{
    /// <summary>
    /// Name of the test.
    /// </summary>
    /// <returns>Name of the test class</returns>
    string Name { get; }

    /// <summary>
    /// Setup the conditions for the test.
    /// </summary>
    void Initialize(SoakTestSettings config);

    /// <summary>
    /// Execute the test synchronously.
    /// </summary>
    void Execute();

    /// <summary>
    /// Tear down the test restoring the system to a state where other tests can be run.
    /// </summary>
    void Teardown();
}
