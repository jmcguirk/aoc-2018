using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using Priority_Queue;

namespace AdventOfCode
{
    public class CaveStatePathSolver
    {
        private static Dictionary<CaveStateNode, CaveStateNode> _visited = new Dictionary<CaveStateNode, CaveStateNode>();
        private static Dictionary<CaveStateNode, int> _costs = new Dictionary<CaveStateNode, int>();
        

        
        private static SimplePriorityQueue<CaveStateNode> _frontier = new SimplePriorityQueue<CaveStateNode>();

        private static Dictionary<CaveStateNode, CaveStateNode> _pathInProgress =
            new Dictionary<CaveStateNode, CaveStateNode>();
        
        private static List<CaveStateNode> _startCandidates = new List<CaveStateNode>();
        private static List<CaveStateNode> _startPathCopy = new List<CaveStateNode>();
        
        private static List<CaveStateNode> _scratch = new List<CaveStateNode>();

        public static int Distance(CaveStateNode tileA, CaveStateNode tileB)
        {
            int res = Math.Abs(tileA.TileX - tileB.TileY) + Math.Abs(tileA.TileX - tileB.TileY);
            if (tileA.CaveTool != tileB.CaveTool)
            {
                res += 7;
            }

            return res;
        }

        
        public static bool RequestPathFromTile(CaveStateNode startTile, CaveStateNode targetTile, List<CaveStateNode> bestPath, out int totalCost)
        {
            
            // Clear our initial state
            _costs.Clear();
            _frontier.Clear();
            _pathInProgress.Clear();
            _scratch.Clear();
            bestPath.Clear();

            totalCost = Int32.MaxValue;
            _frontier.Enqueue(startTile, 0);
            _costs[startTile] = 0;
            _pathInProgress[startTile] = null;
            
            do
            {
                /*
                _frontier.Sort(delegate(CaveStateNode tA, CaveStateNode tB)
                {
                    
                    
                    return (_costs[tA] + Distance(tA, targetTile)).CompareTo(_costs[tB] + Distance(tB, targetTile));
                });*/

                CaveStateNode currentTile = _frontier.Dequeue();
                if (currentTile == targetTile)
                {
                    break;
                }
                //_frontier.RemoveAt(0);

                Dictionary<CaveStateNode, int> transitions = currentTile.Transitions;
                foreach (var kvp in transitions)
                {
                    var neighbor = kvp.Key;
                    int incrementalCost = kvp.Value;
                    int navCost = 0;
                    _costs.TryGetValue(currentTile, out navCost);
                    navCost += incrementalCost;
                    int knownCost = 0;
                    if (!_costs.TryGetValue(neighbor, out knownCost))
                    {
                        knownCost = Int32.MaxValue;
                    }

                    if (navCost < knownCost)
                    {
                        _costs[neighbor] = navCost;
                        _pathInProgress[neighbor] = currentTile;
                        _frontier.Enqueue(neighbor, navCost + Distance(neighbor, targetTile));
                    }
                }
            } while (_frontier.Count > 0);

            if (_pathInProgress.ContainsKey(targetTile))
            {
                var next = targetTile;
                while (next != startTile)
                {
                    bestPath.Add(next);
                    next = _pathInProgress[next];
                }
                bestPath.Reverse();
                totalCost = _costs[targetTile];
                return true;
            }
            

            return false;
        }
    }
    
    
}