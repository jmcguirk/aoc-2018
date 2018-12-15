using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;

namespace AdventOfCode.Combat
{
    public class CombatSimulationPathSovler
    {
        private static Dictionary<int, CombatSimulationTile> _visited = new Dictionary<int, CombatSimulationTile>();
        private static Dictionary<CombatSimulationTile, int> _costs = new Dictionary<CombatSimulationTile, int>();
        

        
        private static List<CombatSimulationTile> _frontier = new List<CombatSimulationTile>();

        private static Dictionary<CombatSimulationTile, CombatSimulationTile> _pathInProgress =
            new Dictionary<CombatSimulationTile, CombatSimulationTile>();
        
        private static List<CombatSimulationTile> _startCandidates = new List<CombatSimulationTile>();
        private static List<CombatSimulationTile> _startPathCopy = new List<CombatSimulationTile>();
        
        private static List<CombatSimulationTile> _scratch = new List<CombatSimulationTile>();

        public static int Distance(CombatSimulationTile tileA, CombatSimulationTile tileB)
        {
            return Math.Abs(tileA.X - tileB.X) + Math.Abs(tileA.Y - tileB.Y);
        }

        public static bool RequestPath(CombatSimulationActor actor, CombatSimulationTile targetTile,
            List<CombatSimulationTile> bestPath)
        {
            var startTile = actor.Tile;
            var sim = actor.Sim;
            _startCandidates.Clear();
            sim.GetUnoccupiedTilesNearTile(startTile, _startCandidates);
            Int32 bestPathLength = Int32.MaxValue;
            bool hasPath = false;
            int indx = 0;
            foreach (var candidate in _startCandidates)
            {
                indx++;
                if (RequestPathFromTile(sim, candidate, targetTile, _startPathCopy))
                {
                    if (actor.IsDebug)
                    {
                        sim.RenderPath(_startPathCopy);
                        actor.Log("Candidate had a valid path of length " + _startPathCopy.Count +
                                  " that started on tile " + candidate.X + "," + candidate.Y + " and ended on " + targetTile.X + "," + targetTile.Y + " actor was on " + startTile.X + "," + startTile.Y);    
                    }
                    
                    if (_startPathCopy.Count < bestPathLength)
                    {
                        
                        bestPath.Clear();
                        bestPath.Add(candidate);
                        bestPath.AddRange(_startPathCopy);
                        bestPathLength = _startPathCopy.Count;
                        hasPath = true;
                    }
                }
            }

            return hasPath;
        }

        public static bool RequestPathFromTile(CombatSimulation sim, CombatSimulationTile startTile, CombatSimulationTile targetTile, List<CombatSimulationTile> bestPath)
        {
            
            // Clear our initial state
            _costs.Clear();
            _frontier.Clear();
            _pathInProgress.Clear();
            _scratch.Clear();
            bestPath.Clear();

            _frontier.Add(startTile);
            _costs[startTile] = 0;
            _pathInProgress[startTile] = null;
            
            do
            {
                _frontier.Sort(delegate(CombatSimulationTile tA, CombatSimulationTile tB)
                {
                    
                    
                    return (_costs[tA] + Distance(tA, targetTile)).CompareTo(_costs[tB] + Distance(tB, targetTile));
                    //if (distCompare != 0)
                    //{
                      //  return distCompare;
                    //}

                    return tA.TileIndex.CompareTo(tB.TileIndex);
                });

                CombatSimulationTile currentTile = _frontier[0];
                if (currentTile == targetTile)
                {
                    break;
                }
                _frontier.RemoveAt(0);
                
                sim.GetUnoccupiedTilesNearTile(currentTile, _scratch);
                foreach (var neighbor in _scratch)
                {
                    int navCost = 0;
                    _costs.TryGetValue(currentTile, out navCost);
                    navCost++;

                    int knownCost = 0;
                    if (!_costs.TryGetValue(neighbor, out knownCost))
                    {
                        knownCost = Int32.MaxValue;
                    }

                    if (navCost < knownCost)
                    {
                        _costs[neighbor] = navCost;
                        _pathInProgress[neighbor] = currentTile;
                        _frontier.Add(neighbor);
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
                return true;
            }
            

            return false;
        }
    }
    
    
}