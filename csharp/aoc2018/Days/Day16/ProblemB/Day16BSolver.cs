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
	public class Day16BSolver
	{

		public static Dictionary<string, IOpCode> _allOpCodes = new Dictionary<string, IOpCode>();
		
		public static Dictionary<int, List<string>> _opCodeIds = new Dictionary<int, List<string>>();

		public static void Solve()
		{
			int sum = 0;

			string inputPath = FileUtils.GetProjectFilePath("Days/Day16/ProblemB/input.txt");
			string[] allLines = File.ReadAllLines(inputPath);

			List<InstructionTestSet> tests = new List<InstructionTestSet>();
			int sets = 0;
			for(int i = 0; i < allLines.Length; i++)
			{
				string line = allLines[i];
				if (line.StartsWith("Before"))
				{
					tests.Add(InstructionTestSet.Parse(allLines[i], allLines[i+1], allLines[i+2], sets));
					sets++;
				}
			}
			
			
			BuildOpCodes();			
			//Log.WriteLine("Stage 1 - Figure out the minimum set of commands supported by the data");
			foreach (var set in tests)
			{
				PerformTest(set);
				int instructionId = set.Command[0];
				List<string> existing;
				if(_opCodeIds.TryGetValue(instructionId, out existing))
				{
					List<string> cp = new List<string>();
					foreach(var str in existing)
					{
						if (set.MatchingOpCodes.Contains(str)) // Check to see if our hypothesis is still supported
						{
							cp.Add(str);
						}
					}
					_opCodeIds[instructionId] = cp;
				}
				else
				{
					existing = new List<string>(); // This is the first time we have encountered this command, include all possible options
					existing.AddRange(set.MatchingOpCodes);
					_opCodeIds[instructionId] = existing;
				}
			}

			
			//DumpOpCodeState();
			
			//Log.WriteLine("Stage 2 - Repeatedly remove unique commands until we have everything disambiguated");
			HashSet<string> removalList = new HashSet<string>();
			while (OpCodesAreAmbiguous())
			{
				string toRemove = null;

				foreach (var kvp in _opCodeIds)
				{
					if (kvp.Value.Count == 1 && !removalList.Contains(kvp.Value[0]))
					{
						toRemove = kvp.Value[0];
						removalList.Add(kvp.Value[0]);
						break;
					}
				}
				if (toRemove == null)
				{
					break;
				}

				foreach (var kvp in _opCodeIds)
				{
					if (kvp.Value.Count > 1 && kvp.Value.Contains(toRemove))
					{
						kvp.Value.Remove(toRemove);
					}
				}
			}
			
			//Log.WriteLine("Finished figuring out commands");
			//DumpOpCodeState();

			int[] registers = new int[4];

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
			}
			Log.WriteLine("Program finished, register state is " + DumpRegisters(registers));
		}

		private static void DumpOpCodeState()
		{
			foreach (var kvp in _opCodeIds)
			{
				StringBuilder sb = new StringBuilder();
				sb.Append(kvp.Key + " = ");
				for (int i = 0; i < kvp.Value.Count; i++)
				{
					if (i > 0)
					{
						sb.Append(",");
					}
					sb.Append(kvp.Value[i]);
				}
				Log.WriteLine(sb.ToString());
			}
		}
		
		private static bool OpCodesAreAmbiguous()
		{
			foreach (var kvp in _opCodeIds)
			{
				if (kvp.Value.Count > 1)
				{
					return true;
				}	
			}
			return false;
		}

		private static void PerformTest(InstructionTestSet set)
		{
			//Log.WriteLine("Beginning test for " + set.TestNumber);
			int[] registers;
			int[] command;
			
			foreach (var kvp in _allOpCodes)
			{
				var opcode = kvp.Value;
				
				// Step 1, create a clean state as desired by the test
				registers = new int[4];
				Array.Copy(set.InitialRegisterState, registers, registers.Length);
				command = new int[4]; // Hypothetically, this should not be required - but we track it for debug reasons
				Array.Copy(set.Command, command, command.Length);
				
				// Step 2, run the command and check that it could make sense i.e. doesn't attempt to address outside the register range
				if (opcode.Execute(command, registers))
				{
					for (int i = 0; i < command.Length; i++)
					{
						if (set.Command[i] != command[i])
						{
							Log.WriteLine("Command arguments were mutated for command " + opcode.Name);
						}
					}
					
					// Step 3, check the results of the commands
					bool allMatch = true;
					for (int i = 0; i < registers.Length; i++)
					{
						if (set.FinalRegisterState[i] != registers[i])
						{
							allMatch = false;
							break;
						}
					}

					if (allMatch)
					{
						//Log.WriteLine("Test Number " + set.TestNumber + " - " + opcode.Name + " matches!");
						set.MatchingOpCodes.Add(opcode.Name);
						//Log.WriteLine(set.DebugLog());
						//Log.WriteLine(DumpRegisters(registers));
					}
					else
					{						
						//Log.WriteLine("Test Number " + set.TestNumber + " - " + opcode.Name + " did not match");
						//Log.WriteLine(set.DebugLog());
						//Log.WriteLine(DumpRegisters(registers));
					}
				}
				else
				{
					Log.WriteLine("Test Number " + set.TestNumber + " - " + opcode.Name + " did not execute");
				}
			}
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

		public class InstructionTestSet
		{
			public int[] InitialRegisterState;

			public int[] FinalRegisterState;

			public int[] Command;

			public List<string> MatchingOpCodes = new List<string>();

			public int TestNumber;

			public int NumMatchingOpCodes
			{
				get { return MatchingOpCodes.Count; }
			}

			public static InstructionTestSet Parse(string first, string middle, string last, int testNumber)
			{
				InstructionTestSet set = new InstructionTestSet();

				set.InitialRegisterState = ParseRegisterState(first);
				set.FinalRegisterState = ParseRegisterState(last);
				set.Command = ParseCommand(middle);
				
				set.TestNumber = testNumber;
				return set;
			}

			private static int[] ParseRegisterState(string rawInput)
			{
				string line = rawInput.Trim();
				line = line.Substring(line.IndexOf('[') + 1); // Dump the first part
				line = line.Substring(0, line.Length - 1); // Trim the end bracket
				string[] parts = line.Split(',');
				var res = new int[4];
				for (int i = 0; i < res.Length; i++)
				{
					res[i] = Int32.Parse(parts[i]);
				}

				return res;
			}

			public string DebugLog()
			{
				StringBuilder sb = new StringBuilder();
				sb.Append("Before: [");
				for (int i = 0; i < InitialRegisterState.Length; i++)
				{
					if (i > 0)
					{
						sb.Append(", ");
					}
					sb.Append(InitialRegisterState[i]);
				}
				sb.Append("]");
				sb.Append(Environment.NewLine);

				for(int i = 0; i < Command.Length; i++){
					if (i > 0)
					{
						sb.Append(" ");
					}
					sb.Append(Command[i]);
				}
				sb.Append(Environment.NewLine);
				sb.Append("After:  [");
				for (int i = 0; i < FinalRegisterState.Length; i++)
				{
					if (i > 0)
					{
						sb.Append(", ");
					}
					sb.Append(FinalRegisterState[i]);
				}
				sb.Append("]");
				sb.Append(Environment.NewLine);
				return sb.ToString();
			}

			public static int[] ParseCommand(string rawInput)
			{
				string line = rawInput.Trim();
				string[] parts = line.Split(' ');
				var res = new int[4];
				for (int i = 0; i < res.Length; i++)
				{
					res[i] = Int32.Parse(parts[i]);
				}

				return res;
			}
		}
	}
}