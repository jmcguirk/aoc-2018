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
using Log=System.Console;
namespace AdventOfCode
{
	public class Day8BSolver
	{


		public static void Solve()
		{
			string inputPath = FileUtils.GetProjectFilePath("Days/Day8/ProblemB/input.txt");
			string text = File.ReadAllText(inputPath);
			string[] parts = text.Split(' ');
			Tree t = new Tree(parts);
			Log.WriteLine("Total value is " + t.TotalValue);
		}



		public class Tree
		{
			public int TotalWeight;

			private int _parseIndex;
			private string[] _buff;

			private Node _root;

			private Char _label = 'A';

			public int TotalValue;
			
			public Tree(string[] buff)
			{
				_buff = buff;
				_root = ParseNext();
				TotalValue = _root.ComputeValue();
			}

			private Node ParseNext()
			{
				var n = new Node();
				n.Label = _label;
				_label++;
				int numChildren = Int32.Parse(_buff[_parseIndex++]);
				int numMeta = Int32.Parse(_buff[_parseIndex++]);
				for (int i = 0; i < numChildren; i++)
				{
					n.Children.Add(ParseNext());
				}
				for (int i = 0; i < numMeta; i++)
				{
					int meta = Int32.Parse(_buff[_parseIndex++]);
					n.Metadata.Add(meta);
					TotalWeight += meta;
					n.TotalMeta += meta;
				}
				return n;
			}

			class Node
			{
				public List<int> Metadata = new List<int>();
				public List<Node> Children = new List<Node>();
				public char Label;
				public int TotalMeta;
				private int? _val;

				public int ComputeValue()
				{
					if (_val.HasValue)
					{
						return _val.Value;
					}
					if (Children.Count == 0)
					{
						_val = TotalMeta;
						return _val.Value;
					}

					int sum = 0;
					foreach (int index in Metadata)
					{
						if (index <= Children.Count)
						{
							sum += Children[index - 1].ComputeValue();
						}
					}

					_val = sum;
					return _val.Value;
				}
			}
		}
	}
}