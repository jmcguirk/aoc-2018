using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Log=System.Console;
namespace AdventOfCode
{
	public class Day5BSolver
	{

		
		public static void Solve()
		{	
			string inputPath = FileUtils.GetProjectFilePath("Days/Day5/ProblemB/input.txt");
			string text = File.ReadAllText(inputPath);
			HashSet<char> chars = new HashSet<char>();
			for (int i = 0; i < text.Length; i++)
			{
				char c = Char.ToLower(text[i]);
				if (!chars.Contains(c))
				{
					chars.Add(c);
				}
			}

			int bestVal = Int32.MaxValue;
			char bestC = ' ';
			foreach (var c in chars)
			{
				int len = TestReaction(text, c);
				if (len < bestVal)
				{
					bestVal = len;
					bestC = c;
				}
			}
			Console.WriteLine("Best pair to remove is " + bestC + " and it creates a polymer of length " + bestVal);
		}

		private static string Filter(string text, char c)
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < text.Length; i++)
			{
				if (text[i] != c && text[i] != Char.ToUpper(c))
				{
					sb.Append(text[i]);
				}
			}

			return sb.ToString();
		}

		private static int TestReaction(string text, char charToTest)
		{
			int index = 0;
			bool didReact;
			text = Filter(text, charToTest);
			while (index < text.Length)
			{
				didReact = false;
				if (index > 0)
				{
					if (DoesReact(text[index], text[index - 1]))
					{
						didReact = true;
						text = text.Remove(index, 1);
						index--;
						text = text.Remove(index, 1);
					}
				} else if (index < text.Length - 1)
				{
					if (DoesReact(text[index], text[index + 1]))
					{
						didReact = true;
						text = text.Remove(index+1, 1);
						text = text.Remove(index, 1);
					}
				}

				if (!didReact)
				{
					index++;
				}
			}

			return text.Length;
		}

		private static bool DoesReact(char a, char b)
		{
			if (a == b)
			{
				return false;
			}

			return Char.ToUpper(a) == Char.ToUpper(b);
		}
		
	}
}