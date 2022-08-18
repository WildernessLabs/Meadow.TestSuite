namespace Validation
{
    public class PushButtonAB : PushButtonBase
    {
        public override bool RunTest(PinPair pair)
        {
            return RunTest(pair, true);
        }
    }
}