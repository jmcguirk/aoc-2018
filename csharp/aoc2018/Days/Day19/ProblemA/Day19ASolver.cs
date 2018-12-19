using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using AdventOfCode.OpCodes;
using Log=System.Console;
namespace AdventOfCode
{
	public class Day19ASolver
	{

		public static Dictionary<string, IOpCode> _allOpCodes = new Dictionary<string, IOpCode>();
		
		public static void Solve()
		{
			int sum = 0;

			string inputPath = FileUtils.GetProjectFilePath("Days/Day19/ProblemA/input.txt");
			string[] allLines = File.ReadAllLines(inputPath);

			List<Instruction> instructions = new List<Instruction>();
			int ip = -1;
			for(int i = 0; i < allLines.Length; i++)
			{
				string line = allLines[i].Trim();
				if (line.StartsWith("#ip"))
				{
					ip = Int32.Parse(line.Split(' ')[1]);
				}
				else
				{
					instructions.Add(Instruction.Parse(line));
				}
			}
			
			
			BuildOpCodes();			

			int[] registers = new int[6];
			
			Log.WriteLine("Beginning a program of " + instructions.Count + " length. Instruction pointer is at " + ip);

			
			int instructionCount = 0;
			while (true)
			{
				
				int ipVal = registers[ip];
				if (ipVal < 0 || ipVal > instructions.Count)
				{
					break;
				}
				Instruction next = instructions[ipVal];
				var op = _allOpCodes[next.OpCode];
				if (!op.Execute(next.Arguments, registers))
				{
					Log.WriteLine("Failed to execute line " + ipVal);
				}
				instructionCount++;
				registers[ip]++;
			}
			
			/*
			int programStartPoint = sets * 4;
			for (int i = programStartPoint; i < allLines.Length; i++)
			{
				string line = allLines[i].Trim();
				if (!string.IsNullOrEmpty(line))
				{
					int[] commandArgs = InstructionTestSet.ParseCommand(line);
					string opCode = _opCodeIds[commandArgs[0]][0];
					var op = _allOpCodes[opCode];
					if (!op.Execute(commandArgs, registers))
					{
						Log.WriteLine("Failed to execute line " + i);
					}
				}
			}*/
			Log.WriteLine("Program finished, register state is " + DumpRegisters(registers) + " executed " + instructionCount);
		}


		private static void BuildOpCodes()
		{
			AddOpCode(new OpCodeAddr());
			AddOpCode(new OpCodeAddi());
			AddOpCode(new OpCodeMulr());
			AddOpCode(new OpCodeMuli());
			AddOpCode(new OpCodeBanr());
			AddOpCode(new OpCodeBani());
			AddOpCode(new OpCodeBorr());
			AddOpCode(new OpCodeBori());
			AddOpCode(new OpCodeSetr());
			AddOpCode(new OpCodeSeti());
			AddOpCode(new OpCodeGtir());
			AddOpCode(new OpCodeGtri());
			AddOpCode(new OpCodeGtrr());
			AddOpCode(new OpCodeEqir());
			AddOpCode(new OpCodeEqri());
			AddOpCode(new OpCodeEqrr());
		}

		private static void AddOpCode(IOpCode code)
		{
			_allOpCodes.Add(code.Name, code);
		}

		private static string DumpRegisters(int[] register)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("[");
			for (int i = 0; i < register.Length; i++)
			{
				if (i > 0)
				{
					sb.Append(", ");
				}
				sb.Append(register[i]);
			}
			sb.Append("]");
			return sb.ToString();
		}


		public class Instruction
		{
			public string OpCode;
			public int[] Arguments;

			public static Instruction Parse(string line)
			{
				Instruction res = new Instruction();
				string[] parts = line.Trim().Split(' ');
				res.OpCode = parts[0];
				res.Arguments = new int[4]; // We offset by 1 since original problem formulation had opcode in arg array 0
				res.Arguments[1] = Int32.Parse(parts[1]);
				res.Arguments[2] = Int32.Parse(parts[2]);
				res.Arguments[3] = Int32.Parse(parts[3]);
				return res;
			}
		}

	}
}