using System;
using System.Diagnostics;

namespace AdventOfCode
{
	public class Timer : IDisposable
	{
		private string _message;
		private Stopwatch _stopWatch;

		public Timer(string msg)
		{
			_message = msg;
			_stopWatch = new Stopwatch();
			_stopWatch.Start();
		}
		
		public void Dispose()
		{
			this._stopWatch.Stop();
			var ms = this._stopWatch.ElapsedMilliseconds;
			Console.WriteLine(_message + " completed in " + ms + " ms");
		}
	}
}