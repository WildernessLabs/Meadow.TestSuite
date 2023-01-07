namespace Meadow.Validation
{
    public class PushButtonBA : PushButtonBase
    {
        public override bool RunTest(PinPair pair)
        {
            return RunTest(pair, false);
        }
    }
}