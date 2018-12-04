using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using Log=System.Console;
namespace AdventOfCode
{
	public class Day4ASolver
	{

		
		public static void Solve()
		{
			int sum = 0;
			
			string inputPath = FileUtils.GetProjectFilePath("Days/Day4/ProblemA/input.txt");
			List<SecurityLog> logRecords = new List<SecurityLog>();
			using (var reader = new System.IO.StreamReader(inputPath))
			{
				string line;
				while((line = reader.ReadLine()) != null)
				{
					string input = line.Trim();
					if (!String.IsNullOrEmpty(input))
					{
						logRecords.Add(new SecurityLog(input));	
					}
					
				}  
			}
			logRecords.Sort(CompareLogTimestamps);

			Dictionary<string, Guard> guards = new Dictionary<string, Guard>();
			
			Guard activeGuard = null;
			int curYear = Int32.MinValue;
			int curMonth = Int32.MinValue;
			int currDay = Int32.MinValue;

			foreach (var log in logRecords)
			{
				Log.WriteLine("Processing " + log.RawLog);
				if (log.Year != curYear || log.Month != curMonth || log.Day != currDay)
				{
					if (activeGuard != null)
					{
						activeGuard.FinalizeShift();
					}

					curYear = log.Year;
					curMonth = log.Month;
					currDay = log.Day;
				}
				
				switch (log.LogType)
				{
					case SecurityLogType.GuardOnDuty:
						if (activeGuard != null)
						{
							activeGuard.GoOffDuty(log.Minute);
						}

						activeGuard = null;
						if (!guards.TryGetValue(log.GuardId, out activeGuard))
						{
							activeGuard = new Guard(log.GuardId);
							guards[log.GuardId] = activeGuard;
						}
						activeGuard.ComeOnDuty(log.Minute);
						break;
					case SecurityLogType.GuardWokeUp:
						if (activeGuard != null)
						{
							activeGuard.WakeUp(log.Minute);
						}
						break;
					case SecurityLogType.GuardFellAsleep:
						if (activeGuard != null)
						{
							activeGuard.GoToSleep(log.Minute);
						}
						else
						{
							Log.WriteLine("Null guard");
						}
						break;
					
				}
			}

			Guard topGuard = null;
			foreach (var kvp in guards)
			{
				if (topGuard == null || topGuard.TotalAsleepMinutes < kvp.Value.TotalAsleepMinutes)
				{
					topGuard = kvp.Value;
				}
			}

			int sleepiestMinuteValue = Int32.MinValue;
			int sleepiestMinute = 0;
			foreach (var kvp in topGuard.SleepHistogram)
			{
				//Log.WriteLine(kvp.Key + " " + kvp.Value);
				if (kvp.Value > sleepiestMinuteValue)
				{
					sleepiestMinute = kvp.Key;
					sleepiestMinuteValue = kvp.Value;
				}
			}
			Log.WriteLine("Sleepiest guard is " + topGuard.GuardId + " result is " + (sleepiestMinute * Int32.Parse(topGuard.GuardId)));
		}

		private static int CompareLogTimestamps(SecurityLog x, SecurityLog y)
		{
			int yearCompare = x.Year.CompareTo(y.Year);
			if (yearCompare != 0)
			{
				return yearCompare;
			}
			int monthCompare = x.Month.CompareTo(y.Month);
			if (monthCompare != 0)
			{
				return monthCompare;
			}
			int dayCompare = x.Day.CompareTo(y.Day);
			if (dayCompare != 0)
			{
				return dayCompare;
			}
			int minuteCompare = x.Minute.CompareTo(y.Minute);
			return minuteCompare;
		}

		public class Guard
		{
			public string GuardId;

			public bool IsAwake;

			public int LastFrameSimulated;

			public int TotalAsleepMinutes;
			public int TotalAwakeMinutes;

			public Dictionary<int, int> SleepHistogram = new Dictionary<int, int>();
			
			private static Regex rgx = new Regex("[^a-zA-Z0-9 -]");

			public Guard(string id)
			{
				GuardId = rgx.Replace(id, "");

			}
			
			public void ComeOnDuty(int frame)
			{
				IsAwake = true;
				LastFrameSimulated = frame;
				TotalAwakeMinutes++;
			}

			private void LogAsleepFrame(int frame)
			{
				//Log.WriteLine("Log asleep frame " + frame + " " + GuardId);
				int cur;
				if(!SleepHistogram.TryGetValue(frame, out cur))
				{
					cur = 0;
				}
				cur++;
				SleepHistogram[frame] = cur;
			}

			public void GoOffDuty(int frame)
			{
				if (!IsAwake)
				{
					for (int i = LastFrameSimulated; i < frame; i++)
					{
						TotalAwakeMinutes++;
						LogAsleepFrame(i);
					}	
				}
				else
				{
					for (int i = LastFrameSimulated; i < frame; i++)
					{
						TotalAwakeMinutes++;
					}
				}				
			}

			public void GoToSleep(int frame)
			{
				//Log.WriteLine("Go to sleep guard");
				for (int i = LastFrameSimulated; i < frame; i++)
				{
					TotalAwakeMinutes++;
				}
				LastFrameSimulated = frame;
				IsAwake = false;
			}
			
			public void WakeUp(int frame)
			{
				for (int i = LastFrameSimulated; i < frame; i++)
				{
					TotalAsleepMinutes++;
					LogAsleepFrame(i);
				}
				LastFrameSimulated = frame;
				IsAwake = true;
			}

			public void FinalizeShift()
			{
				GoOffDuty(60);
			}
		}
		

		public class SecurityLog
		{
			public SecurityLogType LogType;
			public string RawLog;

			public int Year;
			public int Month;
			public int Day;

			public string GuardId;
			public int Minute;

			public SecurityLog(string rawInput)
			{
				RawLog = rawInput;
				string[] parts = rawInput.Split(']');
				ParseDateTime(parts[0].Substring(1));
				ParseAction(parts[1].Substring(1));
			}

			public void ParseAction(string rawInput)
			{
				switch (rawInput[0])
				{
					case 'G':
						LogType = SecurityLogType.GuardOnDuty;
						GuardId = rawInput.Split(' ')[1];
						break;
					case 'f':
						LogType = SecurityLogType.GuardFellAsleep;
						break;
					case 'w':
						LogType = SecurityLogType.GuardWokeUp;
						break;
				}
			}

			private void ParseDateTime(string rawInput)
			{
				string[] parts = rawInput.Split(' ');
				string[] dateParts = parts[0].Split('-');
				string[] timeParts = parts[1].Split(':');

				Year = Int32.Parse(dateParts[0]);
				Month = Int32.Parse(dateParts[1]);
				Day = Int32.Parse(dateParts[2]);

				Minute = Int32.Parse(timeParts[1]);
			}
			
			
			
		}

		public enum SecurityLogType
		{
			Unknown,
			GuardOnDuty,
			GuardFellAsleep,
			GuardWokeUp
		}


		
	}
}