namespace Meadow.TestSuite
{
    public class TestCommand
    {
        public CommandType CommandType { get; set; }
        public string Result { get; set; }

        public virtual void BeforeExecute()
        {
        }
        
        public virtual void Execute()
        {
        }

        public virtual void AfterExecute()
        {
        }

        public virtual void ProcessResult(ICommandSerializer serializer, byte[] resultData)
        {

        }
    }
}