using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using Log=System.Console;
namespace AdventOfCode
{
	public class Day7BSolver
	{
		public const int CharIntegerOffset = 65;

		public static void Solve()
		{
			string inputPath = FileUtils.GetProjectFilePath("Days/Day7/ProblemB/input.txt");
			string[] lines = File.ReadAllLines(inputPath);

			const int DepPivot = 5;
			const int SubPivot = 36;
			const int MaxRunningJobs = 5;

			Job[] queue = new Job[26];
			Job[] runnableQueue = new Job[26];
			Job[] runningQueue = new Job[26];
			int queuedJobs = 0;
			int runnableJobs = 0;
			int runningJobs = 0;
			foreach (var line in lines)
			{
				string l = line.Trim();
				if (string.IsNullOrEmpty(l))
				{
					continue;
				}
				char dep = line[DepPivot];
				char subject = line[SubPivot];
				Job depJob = queue[dep - CharIntegerOffset];
				if (depJob == null)
				{
					depJob = new Job(dep);
					queue[depJob.LetterId] = depJob;
					queuedJobs++;
				}
				
				Job j = queue[subject - CharIntegerOffset];
				if (j == null)
				{
					j = new Job(subject);
					queue[j.LetterId] = j;
					queuedJobs++;
				}
				j.AddDependency(depJob);
			}
			int totalSeconds = 0;
			StringBuilder sb = new StringBuilder();
			while (queuedJobs > 0 || runnableJobs > 0 || runningJobs > 0)
			{
				if (queuedJobs > 0)
				{
					for (int i = 0; i < queue.Length; i++)
					{
						
						if(queue[i] != null)
						{
							if (queue[i].Runnable)
							{	
								var j = queue[i];
								queue[i] = null;
								runnableQueue[i] = j;
								queuedJobs--;
								runnableJobs++;
							}

						}
					}
				}

				if (runnableJobs > 0 && runningJobs < MaxRunningJobs)
				{
					for (int i = 0; i < runnableQueue.Length; i++)
					{
						if(runnableQueue[i] != null)
						{
							var j = runnableQueue[i];
							runnableQueue[i] = null;
							runningQueue[i] = j;
							runnableJobs--;
							runningJobs++;
							if (runningJobs >= MaxRunningJobs)
							{
								break;
							}
						}
					}
				}

				if (runningJobs > 0)
				{
					for (int i = 0; i < runningQueue.Length; i++)
					{
						if(runningQueue[i] != null)
						{
							runningQueue[i].DoWork(totalSeconds);
							if (runningQueue[i].IsComplete)
							{
								var completedJob = runningQueue[i];
								runningQueue[i] = null;
								runningJobs--;
								for (int j = 0; j < queue.Length; j++)
								{
									if(queue[j] != null)
									{
										queue[j].MarkDependencyCompleted(completedJob);
									}
								}
							}
						}
					}
					totalSeconds++;
					
				}

			}
			Log.WriteLine("Took " + MaxRunningJobs + " workers a total of " + totalSeconds + " seconds to complete");
			
		}

		public class Job
		{
			public char Letter;
			public int LetterId;
			public List<int> Dependencies = new List<int>();
			public int RemainingWork;
			public const int BaseWork = 60;

			public Job(char letter)
			{
				 // Normalize for lower case characters
				Letter = letter;
				LetterId = letter - CharIntegerOffset;
				RemainingWork = BaseWork + LetterId + 1;
			}

			public bool IsComplete
			{
				get
				{
					return RemainingWork <= 0;
				}
			}

			public void AddDependency(Job j)
			{
				Dependencies.Add(j.LetterId);
			}

			public void MarkDependencyCompleted(Job j)
			{
				Dependencies.Remove(j.LetterId);

			}

			public void DoWork(int currSecond)
			{
				RemainingWork--;
			}

			public bool Runnable
			{
				get { return Dependencies.Count == 0; }
			}
			
			
		}

	}
}