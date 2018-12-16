using Log=System.Console;
namespace AdventOfCode.OpCodes
{
    public class OpCodeBanr : IOpCode
    {
        public bool Execute(int[] arguments, int[] registerState)
        {
            
            int registerA = arguments[1];
            int registerB = arguments[2];
            int targetRegister = arguments[3];
            
            //Log.WriteLine("Banr " + registerA + " " + registerB + " " + targetRegister);
            
            if (registerA < registerState.Length && registerA >= 0 && registerB < registerState.Length && registerB >= 0)
            {
                int valA = registerState[registerA];
                int valB = registerState[registerB];
                
                //Log.WriteLine("Storing " + (valA & valB) + "(which is the bitwise of " + valA + "," + valB + ") into register " + targetRegister);
                
                registerState[targetRegister] = valA & valB;
                return true;
            }
            else
            {
                return false;
            }
        }

        public string Name
        {
            get { return "banr"; }
        }
    }
}