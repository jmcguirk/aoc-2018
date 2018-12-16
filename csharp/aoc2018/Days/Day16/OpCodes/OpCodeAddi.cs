using Log=System.Console;
namespace AdventOfCode.OpCodes
{
    public class OpCodeAddi : IOpCode
    {
        public bool Execute(int[] arguments, int[] registerState)
        {
            
            int registerA = arguments[1];
            int valB = arguments[2];
            int targetRegister = arguments[3];
            
            //Log.WriteLine("Addi " + registerA + " " + valB + " " + targetRegister);
            
            if (registerA < registerState.Length && registerA >= 0)
            {
                int valA = registerState[registerA];
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
            get { return "addi"; }
        }
    }
}