using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Log=System.Console;
namespace AdventOfCode
{
	public class Day8ASolver
	{


		public static void Solve()
		{
			string inputPath = FileUtils.GetProjectFilePath("Days/Day8/ProblemA/input.txt");
			string text = File.ReadAllText(inputPath);
			string[] parts = text.Split(' ');
			Tree t = new Tree(parts);
			Log.WriteLine("Total weight is " + t.TotalWeight);
		}



		public class Tree
		{
			public int TotalWeight;

			private int _parseIndex;
			private string[] _buff;

			private Node _root;
			
			public Tree(string[] buff)
			{
				_buff = buff;
				_root = ParseNext();
			}

			private Node ParseNext()
			{
				var n = new Node();
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
				}
				return n;
			}

			class Node
			{
				public List<int> Metadata = new List<int>();
				public List<Node> Children = new List<Node>();
			}
		}
	}
}