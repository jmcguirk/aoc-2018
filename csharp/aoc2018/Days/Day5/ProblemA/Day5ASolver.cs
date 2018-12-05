using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Log=System.Console;
namespace AdventOfCode
{
	public class Day5ASolver
	{

		
		public static void Solve()
		{	
			string inputPath = FileUtils.GetProjectFilePath("Days/Day5/ProblemA/input.txt");
			string text = File.ReadAllText(inputPath);
			int index = 0;
			bool didReact;
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
			
			Log.WriteLine(text.Length);
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