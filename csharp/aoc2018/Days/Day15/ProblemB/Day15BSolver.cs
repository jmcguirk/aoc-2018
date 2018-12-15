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
	public class Day15BSolver
	{
		public static void Solve()
		{	
			string inputPath = FileUtils.GetProjectFilePath("Days/Day15/ProblemB/input.txt");
			String[] lines = File.ReadAllLines(inputPath);
			int startingPower = 3;
			for (int i = startingPower; i <= 1000; i++)
			{
				// TODO - This should be doing a binary search, but this problem has exhausted me :p
				CombatSimulation sim = new CombatSimulation(lines, i);
				var result = sim.DoSimulation();
				Log.WriteLine("Completed simulation for attack power " + i + " Winner: " + result.Victor + " with alive elves " + result.ElfAliveCount + "/" + result.ElfParticipantCount);
				if (result.Victor == 'E' && result.ElfAliveCount == result.ElfParticipantCount)
				{
					Log.WriteLine("Finished! - Final power was " + i);
				}
					
			}
			
		}

		
	}
}