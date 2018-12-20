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
	public class Day20BSolver
	{



		private const int _maxX = 10000;
		private const int _maxY = 10000;
		private const int _offset = _maxX / 2;
		private static int[] _bestTiles = new int[_maxX * _maxY];
		
		public static void Solve()
		{


			string inputPath = FileUtils.GetProjectFilePath("Days/Day20/ProblemB/input.txt");
			string input = File.ReadAllText(inputPath).Trim();

			for (int i = 0; i < _bestTiles.Length; i++)
			{
				_bestTiles[i] = Int32.MaxValue;
			}
			

			input = input.Substring(1, input.Length - 2);
			Log.WriteLine("Generating all paths for " + input);
			
			List<string> allPaths = new List<string>();
			string inProgress = "";

			try
			{
				AllPaths(String.Empty, 0, 0, input, allPaths);
			}
			catch (Exception ex)
			{
				Log.WriteLine("Caught exception " + ex.Message + " at " + ex.StackTrace.Substring(0, 1000));
				return;
			}
			
			
			//allPaths.Sort(CompareStringsByLength);
			
			//Log.WriteLine("Found " + allPaths.Count + " total paths - longest path was " + allPaths[0].Length);



			int tx;
			int ty;
			int pathable = 0;
			int minLen = 1000;
			for(int i =0; i < _bestTiles.Length; i++)
			{
				int len = _bestTiles[i];
				if (len < Int32.MaxValue && len >= minLen)
				{
					pathable++;
				}
				
			}
			Log.WriteLine("Found " + pathable + " rooms with a minumum length of " + minLen);
			
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

		public static int GetTileIndex(int x, int y)
		{
			return ((x+_offset) * _maxX) + (_offset + y);
		}

		private static void AllPaths(string soFar, int x, int y, string input, List<string> results)
		{
			if (input.Length == 0)
			{
				RecordPath(x, y, soFar.Length);
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
					
					foreach (var branch in subtrees)
					{
						AllPaths(soFar, x, y, branch, results);
					}
					
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
					AllPaths(soFar, x, y, rest, results);
					
				}
			}
			else
			{
				if (next == 'W')
				{
					x++;
				} else if (next == 'E')
				{
					x--;
				} 
				else if (next == 'N')
				{
					y++;
				}
				else if (next == 'S')
				{
					y--;
				}
				string newString = soFar + next;
				RecordPath(x, y, newString.Length);
				if (input.Length > 1)
				{
					string rest = input.Substring(1);
					AllPaths(newString, x, y, rest, results);
				}
				else
				{
					results.Add(newString);
				}
			}
			
		}


		private static void RecordPath(int tX, int tY, int length)
		{
			//Log.WriteLine("Recording path to " + tX + " " + tY + " " + length);
			int tileIndex = GetTileIndex(tX, tY);
			int v = _bestTiles[tileIndex];
			if (length < v)
			{
				if (v < Int32.MaxValue)
				{
					Log.WriteLine("DUPE");
				}
				_bestTiles[tileIndex] = length;
			}
		}

		private static void IndexToTile(int tileIndex, out int tileX, out int tileY){
			tileY = (tileIndex % _maxX) - _offset;
			tileX = ((tileIndex - tileY) / _maxX) - _offset;
		}

	}
}