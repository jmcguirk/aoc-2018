using Log=System.Console;
namespace AdventOfCode.OpCodes
{
    public class OpCodeSeti : IOpCode
    {
        public bool Execute(int[] arguments, int[] registerState)
        {
            
            int valA = arguments[1];
            int targetRegister = arguments[3];
            
            //Log.WriteLine("Seti " + valA + " " + " " + targetRegister);
            
            registerState[targetRegister] = valA;
            return true;
        }

        public string Name
        {
            get { return "seti"; }
        }
    }
}