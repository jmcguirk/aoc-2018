using Log=System.Console;
namespace AdventOfCode.OpCodes
{
    public class OpCodeAddr : IOpCode
    {
        public bool Execute(int[] arguments, int[] registerState)
        {
            
            int registerA = arguments[1];
            int registerB = arguments[2];
            int targetRegister = arguments[3];
            
            //Log.WriteLine("Addr " + registerA + " " + registerB + " " + targetRegister);
            
            if (registerA < registerState.Length && registerA >= 0 && registerB < registerState.Length && registerB >= 0)
            {
                int valA = registerState[registerA];
                int valB = registerState[registerB];
                registerState[targetRegister] = valA + valB;
                return true;
            }
            else
            {
                return false;
            }
        }

        public string Name
        {
            get { return "addr"; }
        }
    }
}