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
	public class Day7ASolver
	{
		public const int CharIntegerOffset = 65;

		public static void Solve()
		{
			string inputPath = FileUtils.GetProjectFilePath("Days/Day7/ProblemA/input.txt");
			string[] lines = File.ReadAllLines(inputPath);

			const int DepPivot = 5;
			const int SubPivot = 36;

			Job[] queue = new Job[26];
			Job[] runnableQueue = new Job[26];
			int queuedJobs = 0;
			int runnableJobs = 0;
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
			Log.WriteLine("Starting job with " + queuedJobs + " queued jobs");
			StringBuilder sb = new StringBuilder();
			while (queuedJobs > 0 || runnableJobs > 0)
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

				if (runnableJobs > 0)
				{
					Job luckyJob = null;
					for (int i = 0; i < runnableQueue.Length; i++)
					{
						if(runnableQueue[i] != null)
						{
							luckyJob = runnableQueue[i];
							runnableQueue[i] = null;
							break; // Need to go back and see if this unlocks an earlier job now
						}
					}

					if (luckyJob != null)
					{
						sb.Append(luckyJob.Letter);
						runnableJobs--;
						for (int i = 0; i < queue.Length; i++)
						{
							if(queue[i] != null)
							{
								queue[i].MarkDependencyCompleted(luckyJob);
							}
						}
					}
				}
				else
				{
					Log.WriteLine("No runnable jobs found, bailing");
					break;
				}
						
			}
			Log.WriteLine(sb.ToString());
			
		}

		public class Job
		{
			public char Letter;
			public int LetterId;
			public List<int> Dependencies = new List<int>();


			public Job(char letter)
			{
				 // Normalize for lower case characters
				Letter = letter;
				LetterId = letter - CharIntegerOffset;
			}

			public void AddDependency(Job j)
			{
				Dependencies.Add(j.LetterId);
			}

			public void MarkDependencyCompleted(Job j)
			{
				Dependencies.Remove(j.LetterId);

			}

			public bool Runnable
			{
				get { return Dependencies.Count == 0; }
			}
			
			
		}

	}
}