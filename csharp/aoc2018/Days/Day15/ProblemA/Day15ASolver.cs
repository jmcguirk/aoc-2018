using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using AdventOfCode.Combat;
using Log=System.Console;
namespace AdventOfCode
{
	public class Day15ASolver
	{
		public static void Solve()
		{	
			string inputPath = FileUtils.GetProjectFilePath("Days/Day15/ProblemA/input.txt");
			String[] lines = File.ReadAllLines(inputPath);
			CombatSimulation sim = new CombatSimulation(lines);
			sim.DoSimulation();
		}

		
	}
}