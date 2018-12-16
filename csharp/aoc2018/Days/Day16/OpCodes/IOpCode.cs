using System;

namespace AdventOfCode.OpCodes
{
    public interface IOpCode
    {
        bool Execute(int[] arguments, int[] registerState);
        
        string Name { get; }
    }
}