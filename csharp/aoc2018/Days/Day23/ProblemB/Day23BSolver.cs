using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Z3;
using System.Xml.Schema;

using Log=System.Console;
namespace AdventOfCode
{
	public class Day23BSolver
	{

		public static List<Nanobot> AllBots = new List<Nanobot>();

		public static void Solve()
		{
			string inputPath = FileUtils.GetProjectFilePath("Days/Day23/ProblemB/input.txt");
			string[] lines = File.ReadAllLines(inputPath);
			int count = 0;

			foreach (var line in lines)
			{
				string l = line.Trim();
				if (!string.IsNullOrEmpty(l))
				{
					Nanobot bot = new Nanobot(l, count);
					AllBots.Add(bot);
					count++;
				}
			}

			using (var ctx = new Context())
			{
				using (var opt = ctx.MkOptimize())
				{
					ArithExpr X = (ArithExpr)ctx.MkConst("x", ctx.MkIntSort());
					ArithExpr Y = (ArithExpr)ctx.MkConst("y", ctx.MkIntSort());
					ArithExpr Z = (ArithExpr)ctx.MkConst("z", ctx.MkIntSort());

					ArithExpr zero = (ArithExpr)ctx.MkNumeral(0, ctx.IntSort);
					ArithExpr one = (ArithExpr)ctx.MkNumeral(1, ctx.IntSort);
					ArithExpr negOne = (ArithExpr)ctx.MkNumeral(-1, ctx.IntSort);


					List<ArithExpr> signals = new List<ArithExpr>();

					foreach (var bot in AllBots)
					{

						ArithExpr dX = ctx.MkSub(X, (ArithExpr)ctx.MkNumeral(bot.X, ctx.MkIntSort()));
						ArithExpr dY = ctx.MkSub(Y, (ArithExpr)ctx.MkNumeral(bot.Y, ctx.MkIntSort()));
						ArithExpr dZ = ctx.MkSub(Z, (ArithExpr)ctx.MkNumeral(bot.Z, ctx.MkIntSort()));
						
						ArithExpr aDX = (ArithExpr)ctx.MkITE(ctx.MkGe(dX, zero), dX, ctx.MkMul(dX, negOne));
						ArithExpr aDY = (ArithExpr)ctx.MkITE(ctx.MkGe(dY, zero), dY, ctx.MkMul(dY, negOne));
						ArithExpr aDZ = (ArithExpr)ctx.MkITE(ctx.MkGe(dZ, zero), dZ, ctx.MkMul(dZ, negOne));

						ArithExpr dist = ctx.MkAdd(aDX, aDY, aDZ);

						signals.Add((ArithExpr)ctx.MkITE(ctx.MkLe(dist, ctx.MkInt(bot.Range)), one, zero));
					}
					
					var totalSignals = ctx.MkAdd(signals);
					opt.MkMaximize(totalSignals);
					
					ArithExpr DX = (ArithExpr)ctx.MkITE(ctx.MkGe(X, zero), X, ctx.MkMul(X, negOne));
					ArithExpr DY = (ArithExpr)ctx.MkITE(ctx.MkGe(Y, zero), Y, ctx.MkMul(Y, negOne));
					ArithExpr DZ = (ArithExpr)ctx.MkITE(ctx.MkGe(Z, zero), Z, ctx.MkMul(Z, negOne));
					
					ArithExpr distFromOrigin = ctx.MkAdd(DX, DY, DZ);
					
					opt.MkMinimize(distFromOrigin);
					opt.Check();
					var model = opt.Model;
					Log.WriteLine(model.ToString());
				}
			}
		}
		
		


		public class Nanobot
		{
			public long X;
			public long Y;
			public long Z;
			public long Range;

			public int Id;

			public Nanobot(string line, int id)
			{
				Id = id;

				string[] parts = line.Split('>');

				string[] posParts = parts[0].Split('<')[1].Split(',');
				X = Int64.Parse(posParts[0]);
				Y = Int64.Parse(posParts[1]);
				Z = Int64.Parse(posParts[2]);

				Range = Int64.Parse(parts[1].Split('=')[1]);
			}
			

			public string Debug()
			{
				return Id + ": pos=<" + X + "," + Y + "," + Z + ">, r=" + Range;
			}

			
		}
	}
}