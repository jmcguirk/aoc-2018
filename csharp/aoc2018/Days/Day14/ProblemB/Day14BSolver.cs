using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using Log=System.Console;
namespace AdventOfCode
{
	public class Day14BSolver
	{
		//public const int TargetRecipeCount = 18;
		
		public const string TargetRecipe = "919901";

		public const int InitialState = 37;

		public const int ChecksumLen = 10;

		public const int NumElves = 2;

		private static List<int> _gameboard = new List<int>();
		
		private static List<int> _scratch = new List<int>();
		
		private static int[] targetRecipe;

		private static StringBuilder _buffer = new StringBuilder();

		private static int[] _elfPositions;

		private static int _currGeneration;
		
		private static int _numRecipesCreated;

		public static void Solve()
		{
			using (var timer = new Timer("Day 14B Problem"))
			{
				AppendRecipeToGameboard(InitialState);
				_elfPositions = new int[NumElves];
				_elfPositions[0] = 0;
				_elfPositions[1] = 1;
				LogGameboard();

				List<int> target = new List<int>();

				for (int i = 0; i < TargetRecipe.Length; i++)
				{
					target.Add((int) Char.GetNumericValue(TargetRecipe[i]));
				}

				targetRecipe = target.ToArray();

				while (true)
				{
					GenerateNewRecipes();
					if (_gameboard.Count >= targetRecipe.Length)
					{
						// Check to see if our recipe is in the exact back first
						bool allMatch = true;
						for (int j = 0; j < targetRecipe.Length; j++)
						{
							if (targetRecipe[targetRecipe.Length - 1 - j] != _gameboard[_gameboard.Count - 1 - j])
							{
								allMatch = false;
								break;
							}
						}

						if (allMatch)
						{
							int startIndex = _gameboard.Count - targetRecipe.Length;
							Log.WriteLine("Found " + TargetRecipe + " after " + startIndex);
							break;
						}

						allMatch = true;
						for (int j = 0; j < targetRecipe.Length; j++)
						{
							if (targetRecipe[targetRecipe.Length - 1 - j] != _gameboard[_gameboard.Count - 2 - j])
							{
								allMatch = false;
								break;
							}
						}

						if (allMatch)
						{
							int startIndex = _gameboard.Count - targetRecipe.Length - 1;
							Log.WriteLine("Found " + TargetRecipe + " after " + startIndex);
							break;
						}
					}

					if (_numRecipesCreated > 100000000)
					{
						Log.WriteLine("Break!");
						break;
					}

				}
			}
		}

		private static void GenerateNewRecipes()
		{
			int newRecipe = 0;
			for (int j = 0; j < _elfPositions.Length; j++)
			{
				int ingredient = _gameboard[_elfPositions[j]];
				newRecipe += ingredient;
			}
				
			AppendRecipeToGameboard(newRecipe);

			for (int j = 0; j < _elfPositions.Length; j++)
			{
				int pos = _elfPositions[j];
				int ingredient = _gameboard[pos];
				int advance = ingredient + 1;
				int newIndex = (pos + advance) % _gameboard.Count;
				_elfPositions[j] = newIndex;
			}

		}
		
		public static void LogGameboard()
		{
			_buffer.Clear();
			for (int i = 0; i < _gameboard.Count; i++)
			{
				int elfIndex = Int32.MinValue;
				for (int j = 0; j < _elfPositions.Length; j++)
				{
					if (_elfPositions[j] == i)
					{
						elfIndex = j;
						break;
					}
				}

				if (elfIndex > Int32.MinValue)
				{
					if (elfIndex == 0)
					{
						//_buffer.Append('(');
						_buffer.Append(_gameboard[i]);
						//_buffer.Append(')');
					} else if (elfIndex == 1)
					{
						//_buffer.Append('[');
						_buffer.Append(_gameboard[i]);
						//_buffer.Append(']');	
					}
					
				}
				else
				{
					//_buffer.Append(' ');
					_buffer.Append(_gameboard[i]);
					//_buffer.Append(' ');
				}
				
				
			}
			Log.WriteLine(_buffer.ToString());
		}

		private static void AppendRecipeToGameboard(int val)
		{
			_scratch.Clear();
			if (val == 0)
			{
				_numRecipesCreated++;
				_scratch.Add(val);
			}
			else
			{
				while (val > 0)
				{
					_numRecipesCreated++;
					_scratch.Add(val % 10);
					val = val / 10;
				}
			}


			_scratch.Reverse();
			_gameboard.AddRange(_scratch);
		}
		
	}
}