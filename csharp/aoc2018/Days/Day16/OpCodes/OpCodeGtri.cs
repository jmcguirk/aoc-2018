using Log=System.Console;
namespace AdventOfCode.OpCodes
{
    public class OpCodeGtri : IOpCode
    {
        public bool Execute(int[] arguments, int[] registerState)
        {
            
            int registerA = arguments[1];
            int valB = arguments[2];
            int targetRegister = arguments[3];
            
            //Log.WriteLine("Gtri " + registerA + " " + valB + " " + targetRegister);
            
            if (registerA < registerState.Length && registerA >= 0)
            {
                int valA = registerState[registerA];
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
            get { return "gtri"; }
        }
    }
}