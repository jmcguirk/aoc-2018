
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Log=System.Console;
namespace AdventOfCode
{
	public class Day11BSolver
	{
	
		const int SerialId = 9110;
		private const int NumRows = 300;
		private const int NumCols = 300;
		public static int[] PowerValues;
		
		public static void Solve()
		{
			
			using (var timer = new Timer("Day 11B Problem"))
			{
				PowerValues = new int[NumRows * NumCols];
				for (int i = 1; i < NumRows; i++)
				{
					for (int j = 1; j < NumCols; j++)
					{
						SetPowerValue(i, j, CalculatePowerAtLocation(i, j));
					}
				}
				//String[] lines = File.ReadAllLines(inputPath);

				List<Task<PowerBox>> lst = new List<Task<PowerBox>>();
				bool foundEnd = false;
				for (int i = 1; i <= NumRows; i++)
				{
					lst.Add(CreateJob(i));	
				}
				

				Task<PowerBox>[] tasks = lst.ToArray();
				Log.WriteLine("Awaiting " + tasks.Length + " tasks");
				Task.WaitAll(tasks);
				PowerBox best = null;
				for (int i = 0; i < tasks.Length; i++)
				{
					if (tasks[i].Result == null)
					{
						continue;
					}
					if (best == null || best.Power < tasks[i].Result.Power)
					{
						best = tasks[i].Result;
					}
				}
				Log.WriteLine("Best box at " + best.TopLeftCornerX + "," + best.TopLeftCornerY + " with total power " + best.Power + " and size " + best.Size);
			}
		}

		public static void SetPowerValue(int x, int y, int val)
		{
			PowerValues[((x-1) * NumCols) + (y - 1)] = val;
		}
		
		public static int GetPowerValue(int x, int y )
		{
			return PowerValues[((x - 1) * NumCols) + (y - 1)];
		}
		
		public class PowerBox
		{
			public int Power;
			public int Size;
			public int TopLeftCornerX;
			public int TopLeftCornerY;
		}

		public static Task<PowerBox> CreateJob(int size)
		{
			return Task<PowerBox>.Run(() => FindBestBoxOfSize(size));
		}

		public static PowerBox FindBestBoxOfSize(int size)
		{
			PowerBox res = null;

			int endRow = NumRows - size;
			int endCol = NumCols - size;
			//Log.WriteLine("Starting " + startRow + " ");
			for(int i = 1; i <= endRow; i++)
			{
				for(int j = 1; j < endCol; j++)
				{
					PowerBox box = GenerateBoxOfSize(i, j, size);
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
		
		public static PowerBox GenerateBoxOfSize(int i, int j, int size)
		{
			//Log.WriteLine("Generating a box at " + i + " , " + j);
			PowerBox res = new PowerBox();
			res.Size = size;
			res.TopLeftCornerX = i;
			res.TopLeftCornerY = j;
			int totalPower = 0;
			for (int x = 0; x < size; x++)
			{
				for (int y = 0; y < size; y++)
				{
					totalPower += GetPowerValue(i+x, j+y);
				}	
			}

			res.Power = totalPower;
			return res;
		}
	}
}