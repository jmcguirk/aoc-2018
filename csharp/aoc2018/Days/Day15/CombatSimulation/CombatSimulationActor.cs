using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;


namespace AdventOfCode.Combat
{
    public class CombatSimulationActor
    {
        public const string DebugActorId = null;
        //public const string DebugActorId = "G16";
        public string Id;
        public readonly char Faction;
        public int RemainingHealth;
        public int AttackPower;
        public readonly CombatSimulation Sim;
        private List<CombatSimulationActor> _scratchList = new List<CombatSimulationActor>();
        private List<CombatSimulationTile> _scratchTiles = new List<CombatSimulationTile>();
        private List<CombatSimulationTile> _scratchPath = new List<CombatSimulationTile>();
        private List<PathingOption> _pathingOptions = new List<PathingOption>();

        private HashSet<CombatSimulationTile> _calculatedPathingOptions = new HashSet<CombatSimulationTile>();

        public int X;
        public int Y;
        public int TileIndex;
        

        public CombatSimulationActor(CombatSimulation sim, char faction, int numInFaction, int remainingHealth, int attackPower, int initialX, int initialY)
        {
            this.Id = faction.ToString() + numInFaction;
            this.Sim = sim;
            this.Faction = faction;
            this.RemainingHealth = remainingHealth;
            this.AttackPower = attackPower;
            this.X = initialX;
            this.Y = initialY;
        }

        public char Render()
        {
            if (this.Id == DebugActorId)
            {
                return 'D';
            }
            return Faction;
        }

        public bool IsDebug
        {
            get { return this.Id == DebugActorId; }
            
        }

        public bool IsAlive
        {
            get { return this.RemainingHealth > 0; }
        }

        public bool Update()
        {

            if (!this.IsAlive)
            {
                return true;
            }
            Log("Beginning Move");
               
            
            // Check to see if any of our cardinal directions have a target
            this.Sim.GetNearByFoes(this, _scratchList);
            if (_scratchList.Count > 0)
            {
                _scratchList.Sort(CompareFoesByHealth);
                AttackFoe(_scratchList[0]);
                return true;
            }
            
            Log("No nearby targets, considering targets");
            
            this.Sim.GetAllFoes(this, _scratchList);

            if (_scratchList.Count == 0)
            {
                Log("No foes?");
                return false;
            }
            Log("Considering " + _scratchList.Count + " possible foes");

            _calculatedPathingOptions.Clear();
            _pathingOptions.Clear();
            if (this.IsDebug)
            {
                this.Sim.RenderGameboard(true);
            }
            foreach (var foe in this._scratchList)
            {
                Log("Considering " + foe.Id);
                this.Sim.GetUnoccupiedTilesNearActor(foe, _scratchTiles);
                Log("Found " + _scratchTiles.Count + " possible pathing locations to attack " + foe.Id);
                //this.Sim.DebugTiles(_scratchTiles, '?');
                foreach (var option in _scratchTiles)
                {
                    if (!_calculatedPathingOptions.Contains(option))
                    {
                        if (CombatSimulationPathSovler.RequestPath(this, option, _scratchPath))
                        {
                            var pathingOption = new PathingOption();
                            pathingOption.Path.AddRange(_scratchPath);
                            pathingOption.PathingCost = _scratchPath.Count;
                            pathingOption.TargetTile = option;
                            pathingOption.TargetActor = foe;
                            _pathingOptions.Add(pathingOption);
                        }
                        //_calculatedPathingOptions.Add(option);
                        
                    }
                }
            }

            if (_pathingOptions.Count == 0)
            {
                Log("Could not find a pathing option, bailing");
                return true;
            }
            
            _pathingOptions.Sort(delegate(PathingOption optionA, PathingOption optionB)
            {
                var pA = optionA.Path;
                var pB = optionB.Path;
                var dist = pA.Count.CompareTo(pB.Count);
                if (dist != 0)
                {
                    return dist;    
                }

                return optionA.TargetTile.TileIndex.CompareTo(optionB.TargetTile.TileIndex);
            });
            
            


            
            
            var bestOption = _pathingOptions[0];
            Log("Best path was to " + bestOption.TargetActor.Id + " ending on " + bestOption.TargetTile.X + "," + bestOption.TargetTile.Y + " starting on " + bestOption.Path[0].X + "," + bestOption.Path[0].Y + " with path length " + bestOption.Path.Count);
            
            if (this.Id == DebugActorId)
            {
                for (int i = 1; i < bestOption.Path.Count; i++)
                {
                    this.Sim.DebugTile(bestOption.Path[i], '^');
                }    
                this.Sim.DebugTile(bestOption.TargetTile, 'F');
            }
            
            
            
            this.Sim.MoveActorToTile(this, bestOption.Path[0]);
            
            // Now that we have moved, check to see if we're now in range of another foe and attack them, otherwise - end the turn
            this.Sim.GetNearByFoes(this, _scratchList);
            if (_scratchList.Count > 0)
            {
                _scratchList.Sort(CompareFoesByHealth);
                AttackFoe(_scratchList[0]);
                return true;
            }

            return true;
        }

        private int CompareFoesByHealth(CombatSimulationActor a, CombatSimulationActor b)
        {
            int hpComp = a.RemainingHealth.CompareTo(b.RemainingHealth);
            if (hpComp != 0)
            {
                return hpComp;
            }

            return a.TileIndex.CompareTo(b.TileIndex);
        }


        public CombatSimulationTile Tile
        {
            get { return this.Sim.GetTileAt(this.X, this.Y); }
        }

        public void Log(string message)
        {
            if (DebugActorId != Id)
            {
                return;
            }
            System.Console.WriteLine("["+this.Id+ "] " + message);
        }

        private void AttackFoe(CombatSimulationActor foe)
        {
            foe.RemainingHealth -= this.AttackPower;
            Log("attacked " + foe.Id + " for " + this.AttackPower + " damage - remaining HP is " + foe.RemainingHealth);
            if (!foe.IsAlive)
            {
                Log(foe.Id + " died!");
            }
            
        }

        public class PathingOption
        {
            public CombatSimulationTile TargetTile;
            public CombatSimulationActor TargetActor;
            public bool Pathable;
            public int PathingCost;
            public List<CombatSimulationTile> Path = new List<CombatSimulationTile>();
        }
    }
}