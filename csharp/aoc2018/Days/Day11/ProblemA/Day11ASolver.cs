
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Log=System.Console;
namespace AdventOfCode
{
	public class Day11ASolver
	{
	
		const int SerialId = 9110;
		const int NumWorkers = 1;
		private const int NumRows = 300;
		private const int NumCols = 300;
		
		public static void Solve()
		{
			
			using (var timer = new Timer("Day 11A Problem"))
			{
				
				//String[] lines = File.ReadAllLines(inputPath);

				List<Task<PowerBox>> lst = new List<Task<PowerBox>>();
				int rowSlice = NumRows / NumWorkers;
				bool foundEnd = false;
				for (int i = 1; i <= NumRows; i+= rowSlice)
				{
					lst.Add(CreateJob(i, rowSlice));	
				}
				

				Task<PowerBox>[] tasks = lst.ToArray();
				Log.WriteLine("Awaiting " + tasks.Length + " tasks");
				Task.WaitAll(tasks);
				PowerBox best = null;
				for (int i = 0; i < tasks.Length; i++)
				{
					if (best == null || best.Power < tasks[i].Result.Power)
					{
						best = tasks[i].Result;
					}
				}
				Log.WriteLine("Best box at " + best.TopLeftCornerX + "," + best.TopLeftCornerY + " with total power " + best.Power);
			}
		}
		

		public class PowerBox
		{
			public int Power;
			public int TopLeftCornerX;
			public int TopLeftCornerY;
		}

		public static Task<PowerBox> CreateJob(int startRow, int workSlice)
		{
			return Task<PowerBox>.Run(() => FindBestBox(startRow, workSlice));
		}

		public static PowerBox FindBestBox(int startRow, int workSlice)
		{
			PowerBox res = null;

			startRow = Math.Clamp(startRow, 2, NumRows - 1);
			int endRow = Math.Clamp(startRow + workSlice, 2, NumRows - 1);
			//Log.WriteLine("Starting " + startRow + " ");
			for(int i = startRow; i < endRow; i++)
			{
				for(int j = 2; j < NumCols; j++)
				{
					PowerBox box = GenerateBox(i, j);
					if (res == null || box.Power > res.Power)
					{
						res = box;
					}
				}	
			}
			
			return res;
		}

		public static int CalculatePowerAtLocation(int i, int j)
		{
			int rackId = i + 10;
			int powerLevel = rackId * j;
			powerLevel += SerialId;
			powerLevel *= rackId;
			powerLevel =  Math.Abs(powerLevel / 100 % 10);
			powerLevel -= 5;
			return powerLevel;
		}
		
		public static PowerBox GenerateBox(int i, int j)
		{
			//Log.WriteLine("Generating a box at " + i + " , " + j);
			PowerBox res = new PowerBox();
			res.TopLeftCornerX = i-1;
			res.TopLeftCornerY = j-1;
			int totalPower = 0;
			for (int x = -1; x <= 1; x++)
			{
				for (int y = -1; y <= 1; y++)
				{
					totalPower += CalculatePowerAtLocation(i+x, j+y);
				}	
			}

			res.Power = totalPower;
			return res;
		}
	}
}