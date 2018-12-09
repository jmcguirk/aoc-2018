using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using Log=System.Console;
namespace AdventOfCode
{
	public class Day9BSolver
	{

		private static int _numPlayers = 10;
		private static int _highestMarble = 1618;
		
		private const int _scoreMarbleMultiple = 23;
		private const int _normalPlayJump = 2;
		private const int _scoreRewind = 7;
		private const int _logRate = 100000;
		
		
		private static List<int> _marbles = new List<int>();
			
		private static int _currMarbleIndex = 0;
		private static int _nextMarbleValue = 0;
		private static int _turnIndex = 0;

		private static int _currPlayer = -1;
		
		private static  Dictionary<int, long> _playerScores = new Dictionary<int, long>();
		
		private static Regex _numericRgx = new Regex("[^0-9 -]");
		
		public static void Solve()
		{
			string inputPath = FileUtils.GetProjectFilePath("Days/Day9/ProblemB/input.txt");
			string text = File.ReadAllText(inputPath);

			string[] parts = text.Split(';');
			_numPlayers = Int32.Parse(_numericRgx.Replace(parts[0], ""));
			_highestMarble = Int32.Parse(_numericRgx.Replace(parts[1], ""));

			Log.WriteLine("Starting a game with " + _numPlayers + " players and highest marble value " + _highestMarble);

			DoInitialSetup();
			PlayGame();
			LogWinner();
		}

		private static void DoInitialSetup()
		{
			// Initial setup, add the first two marbles
			_marbles.Add(0);
			_nextMarbleValue++;
			//LogGameboard();
			_turnIndex++;
			_currPlayer++;
			
			
			_marbles.Add(1);
			_nextMarbleValue++;
			_currMarbleIndex = 1;
			//LogGameboard();
			_turnIndex++;
			_currPlayer++;
		}

		private static void PlayGame()
		{
			while (_nextMarbleValue <= _highestMarble)
			{
				if (_nextMarbleValue > 0 &&  _nextMarbleValue % _scoreMarbleMultiple == 0)
				{
					PlaceScoringMarble();
				}
				else
				{
					PlaceNormalMarble();
				}
				//LogGameboard();
				_nextMarbleValue++;
				_turnIndex++;
				_currPlayer++;
				_currPlayer = _currPlayer % _numPlayers;
				if (_nextMarbleValue % _logRate == 0)
				{
					float percent = (float)_nextMarbleValue / _highestMarble;
					Log.WriteLine(percent);
				}
			}
		}

		private static void LogWinner()
		{
			int winningPlayerIndex = 0;
			var winningPlayerValue = Int64.MinValue;
			foreach (var kvp in _playerScores)
			{
				if (kvp.Value > winningPlayerValue)
				{
					winningPlayerIndex = kvp.Key;
					winningPlayerValue = kvp.Value;
				}
			}
			
			Log.WriteLine("Winning player is " + (winningPlayerIndex + 1) + " with a score of " + winningPlayerValue);
		}
		
		private static void PlaceNormalMarble()
		{
			int placementIndex = _currMarbleIndex + _normalPlayJump;
			if (placementIndex == _marbles.Count)
			{
				_marbles.Add(_nextMarbleValue);
				_currMarbleIndex = _marbles.Count - 1;
			}
			else
			{
				placementIndex = placementIndex % _marbles.Count;
				_marbles.Insert(placementIndex, _nextMarbleValue);
				_currMarbleIndex = placementIndex;
			}
		}
		
		private static void PlaceScoringMarble()
		{
			int totalPointsToAdd = _nextMarbleValue;
			int targetRemovalIndex = (_currMarbleIndex - _scoreRewind) % _marbles.Count;
			if (targetRemovalIndex < 0)
			{
				targetRemovalIndex = _marbles.Count + targetRemovalIndex;
			}
			int targetRemovalValue = _marbles[targetRemovalIndex];
			_marbles.RemoveAt(targetRemovalIndex);
			_currMarbleIndex = targetRemovalIndex;
			totalPointsToAdd += targetRemovalValue;
			long curr;
			_playerScores.TryGetValue(_currPlayer, out curr);
			curr += totalPointsToAdd;
			_playerScores[_currPlayer] = curr;
		}

		private static void LogGameboard()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("[" + (_currPlayer + 1) + "] ");
			for (int i = 0; i < _marbles.Count; i++)
			{
				if (i == _currMarbleIndex)
				{
					sb.Append("(" + _marbles[i] + ") ");
				}
				else
				{
					sb.Append(_marbles[i] + " ");
				}
			}
			Log.WriteLine(sb.ToString());
		}
		
		

	}
}