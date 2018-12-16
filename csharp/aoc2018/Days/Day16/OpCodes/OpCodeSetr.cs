using Log=System.Console;
namespace AdventOfCode.OpCodes
{
    public class OpCodeSetr : IOpCode
    {
        public bool Execute(int[] arguments, int[] registerState)
        {
            
            int registerA = arguments[1];
            int targetRegister = arguments[3];
            
            //Log.WriteLine("Setr " + registerA + " " + " " + targetRegister);
            
            if (registerA < registerState.Length && registerA >= 0)
            {
                int valA = registerState[registerA];
                registerState[targetRegister] = valA;
                return true;
            }
            else
            {
                return false;
            }
        }

        public string Name
        {
            get { return "setr"; }
        }
    }
}