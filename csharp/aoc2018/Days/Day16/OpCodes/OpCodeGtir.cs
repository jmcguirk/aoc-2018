using Log=System.Console;
namespace AdventOfCode.OpCodes
{
    public class OpCodeGtir : IOpCode
    {
        public bool Execute(int[] arguments, int[] registerState)
        {
            
            int valA = arguments[1];
            int registerB = arguments[2];
            int targetRegister = arguments[3];
            
            //Log.WriteLine("Gtir " + valA + " " + registerB + " " + targetRegister);
            
            if (registerB < registerState.Length && registerB >= 0)
            {
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
            get { return "gtir"; }
        }
    }
}