using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using Log=System.Console;
namespace AdventOfCode
{
	public class Day10ASolver
	{
		public const int ImageSize = 1024;
		public const int Quality = 100;

		public const int FramesToSimulate = 1000000;
		
		public static void Solve()
		{
			string inputPath = FileUtils.GetProjectFilePath("Days/Day10/ProblemA/input.txt");
			String[] lines = File.ReadAllLines(inputPath);

			string outpath = "/scratch/Day10Images";
			DirectoryInfo di = new DirectoryInfo(outpath);
			if (!di.Exists)
			{
				di.Create();
			}

			List<Point> points = new List<Point>();

			foreach (var line in lines)
			{
				string l = line.Trim();
				if (!string.IsNullOrEmpty(l))
				{
					points.Add(new Point(l));
				}
			}

			for (int i = 0; i < FramesToSimulate; i++)
			{
				string fileName = i + ".png";
				RenderGameboard(points, Path.Combine(outpath, fileName));
				foreach (var p in points)
				{
					p.Advance();
				}
			}
		}

		public static void RenderGameboard(List<Point> points, string fileName)
		{
			int inFrame = 0;
			foreach (var p in points)
			{
				int adjustedX = p.X + ImageSize / 2;
				int adjustedY = p.Y + ImageSize / 2;
				if (adjustedX > 0 && adjustedX < ImageSize && adjustedY > 0 && adjustedY < ImageSize)
				{
					inFrame++;
				}
			}

			if (inFrame != points.Count)
			{
				return;
			}
			
			Bitmap b = new Bitmap(ImageSize, ImageSize);

			
			foreach (var p in points)
			{
				int adjustedX = p.X + ImageSize / 2;
				int adjustedY = p.Y + ImageSize / 2;
				if (adjustedX > 0 && adjustedX < ImageSize && adjustedY > 0 && adjustedY < ImageSize)
				{
					b.SetPixel(adjustedX, adjustedY, System.Drawing.Color.Black);
				}
			}
			
			using (var graphics = Graphics.FromImage(b))
			{
				graphics.CompositingQuality = CompositingQuality.HighSpeed;
				graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
				graphics.CompositingMode = CompositingMode.SourceCopy;
				graphics.DrawImage(b, 0, 0, ImageSize, ImageSize);
				using (var output = File.Open(fileName, FileMode.Create))
				{
					var qualityParamId = Encoder.Quality;
					var encoderParameters = new EncoderParameters(1);
					encoderParameters.Param[0] = new EncoderParameter(qualityParamId, Quality);
					var codec = ImageCodecInfo.GetImageDecoders().FirstOrDefault(c => c.FormatID == ImageFormat.Png.Guid);
					b.Save(output, codec, encoderParameters);
				}
			}
		}

		public class Point
		{
			public int X;
			public int Y;

			public int VelocityX;
			public int VelocityY;

			public Point(string rawInput)
			{
				int pivot = rawInput.IndexOf('<') + 1;
				string pos = rawInput.Substring(pivot, rawInput.IndexOf('>') - pivot);
				string rest = rawInput.Substring(rawInput.IndexOf('>') + 1);
				pivot = rest.IndexOf('<') + 1;
				string vel = rest.Substring(pivot, rest.IndexOf('>') - pivot);
				string[] posParts = pos.Split(',');
				X = Int32.Parse(posParts[0]);
				Y = Int32.Parse(posParts[1]);
				string[] velParts = vel.Split(',');
				VelocityX = Int32.Parse(velParts[0]);
				VelocityY = Int32.Parse(velParts[1]);
			}

			public void Advance()
			{
				X += VelocityX;
				Y += VelocityY;
			}
		}
	}
}