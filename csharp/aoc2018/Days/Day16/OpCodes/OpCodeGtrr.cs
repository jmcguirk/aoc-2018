using Log=System.Console;
namespace AdventOfCode.OpCodes
{
    public class OpCodeGtrr : IOpCode
    {
        public bool Execute(int[] arguments, int[] registerState)
        {
            
            int registerA = arguments[1];
            int registerB = arguments[2];
            int targetRegister = arguments[3];
            
            //Log.WriteLine("Gtrr " + registerA + " " + registerB + " " + targetRegister);
            
            if (registerA < registerState.Length && registerA >= 0 && registerB < registerState.Length && registerB >= 0)
            {
                int valA = registerState[registerA];
                int valB = registerState[registerB];
                if (valA > valB)
                {
                    registerState[targetRegister] = 1;
                }
                else
                {
                    registerState[targetRegister] = 0;
                }
                
                return true;
            }
            else
            {
                return false;
            }
        }

        public string Name
        {
            get { return "gtrr"; }
        }
    }
}