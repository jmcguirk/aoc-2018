using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Log=System.Console;
namespace AdventOfCode
{
    public class CaveStateNode
    {
        public int TileX;
        public int TileY;
        public int TileIndex;
        public int CaveTool;
        public char TileType;
        public int ErosionLevel;
        public int RiskLevel;
        
            

        public Dictionary<CaveStateNode, int> Transitions = new Dictionary<CaveStateNode, int>(); 
        
        public const char Rocky = '.';
        public const char Wet = '=';
        public const char Narrow = '|';
        public const char Target = 'T';
        public const char Mouth = 'M';

        public const int RockyRisk = 0;
        public const int WetRisk = 1;
        public const int NarrowRisk = 2;


        public bool IsValidState(int tool)
        {
            if (TileType == Rocky || TileType == Target || TileType == Mouth)
            {
                return tool == CaveTools.ClimbingGear || tool == CaveTools.Torch;
            }
            if (TileType == Wet)
            {
                return tool == CaveTools.ClimbingGear || tool == CaveTools.None;
            }
            if (TileType == Narrow)
            {
                return tool == CaveTools.Torch || tool == CaveTools.None;
            }

            return false;
        }

        public void AddTransition(CaveStateNode node, int cost)
        {
            Transitions[node] = cost;
        }

        public int NeighborCount
        {
            get { return Transitions.Keys.Count; }
        }

        public string DebugLog()
        {
            return TileX + "," + TileY + " " + TileType + " Tool: " + DescribeTool() + ", Neighbors " + NeighborCount;
        }

        public string DescribeNeighbors()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var kvp in Transitions)
            {
                stringBuilder.AppendLine("    " + kvp.Value + ":" + kvp.Key.DebugLog());
            }
            return stringBuilder.ToString();
        }

        private string DescribeTool()
        {
            switch (CaveTool)
            {
                case CaveTools.None:
                    return "None";
                case CaveTools.ClimbingGear:
                    return "ClimbingGear";
                case CaveTools.Torch:
                    return "Torch";
            }

            return "Unknown";
        }
    }



    public class CaveTools
    {
        public const int None = 0;
        public const int ClimbingGear = 1;
        public const int Torch = 2;
    }
    
}