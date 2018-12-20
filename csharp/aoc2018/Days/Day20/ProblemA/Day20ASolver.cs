using System;
using System.Collections.Generic;
using System.ComponentModel;
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
	public class Day20ASolver
	{

		
		
		public static void Solve()
		{


			string inputPath = FileUtils.GetProjectFilePath("Days/Day20/ProblemA/input.txt");
			string input = File.ReadAllText(inputPath).Trim();

			
			

			input = input.Substring(1, input.Length - 2);
			Log.WriteLine("Generating all paths for " + input);
			
			List<string> allPaths = new List<string>();
			string inProgress = "";

			try
			{
				AllPaths(String.Empty, input, allPaths);
			}
			catch (Exception ex)
			{
				Log.WriteLine("Caught exception " + ex.Message + " at " + ex.StackTrace.Substring(0, 1000));
				return;
			}
			
			
			allPaths.Sort(CompareStringsByLength);
			
			Log.WriteLine("Found " + allPaths.Count + " total paths - longest path was " + allPaths[0].Length);

			
			
			foreach (var path in allPaths)
			{
				//Log.WriteLine(path);	
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

		}

		private static int CompareStringsByLength(string x, string y)
		{
			return y.Length.CompareTo(x.Length);
		}


		private static void AllPaths(string soFar, string input, List<string> results)
		{
			if (input.Length == 0)
			{
				results.Add(soFar);
				return;
			}
			char next = input[0];
			if (next == '(')
			{
				int remainingCount = 1;
				int pivot = -1;
				for (int i = 1; i < input.Length; i++)
				{
					if (input[i] == ')')
					{
						remainingCount--;
					}
					if (input[i] == '(')
					{
						remainingCount++;
					}

					if (remainingCount == 0)
					{
						pivot = i;
						break;
					}
					
				}

				if (remainingCount > 0)
				{
					throw new Exception("Failed to parse branches");
				}

				string rawbranches = input.Substring(1, pivot - 1);
				//Log.WriteLine("Pivot point at " + pivot + " raw branches are " + rawbranches);

				int depth = 0;

				string branchInProgress = String.Empty;
				bool awaitingBranch = false;
				List<string> subtrees = new List<string>();
				
				for (int i = 0; i < rawbranches.Length; i++)
				{
					var c = rawbranches[i];
					if (depth == 0 && c == '|')
					{
						subtrees.Add(branchInProgress);
						branchInProgress = string.Empty;
						awaitingBranch = true;
						continue;
					}
					if (c == '(')
					{
						depth++;
					}
					else if (c == ')')
					{
						depth--;
					}
					else
					{
						awaitingBranch = false;
					}

					branchInProgress = branchInProgress + c;

				}

				if (awaitingBranch)
				{
					//Log.WriteLine("Empty branch!");
					subtrees.Clear();
					subtrees.Add(String.Empty);
					
				}
				else if(!String.IsNullOrEmpty(branchInProgress))
				{
					subtrees.Add(branchInProgress);	
				}
				
				

				foreach (var branch in subtrees)
				{
					string rest = branch + input.Substring(pivot + 1);
					//Log.WriteLine("Branching " + rest);
					AllPaths(soFar, rest, results);
					
				}
			}
			else
			{
				string newString = soFar + next;
				
				if (input.Length > 1)
				{
					string rest = input.Substring(1);
					AllPaths(newString, rest, results);
				}
				else
				{
					results.Add(newString);
				}
			}
		}



	}
}