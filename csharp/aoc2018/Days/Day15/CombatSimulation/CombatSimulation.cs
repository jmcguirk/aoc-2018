using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Log=System.Console;
namespace AdventOfCode.Combat
{
    public class CombatSimulation
    {

        private const char GoblinFaction = 'G';
        private const char ElfFaction = 'E';
        private const char EmptyTile = '.';
        private const char ObstructionTile = '#';
        private const int StartingHealth = 200;
        private static int GoblinAttackPower = 3;
        private static int ElfAttackPower = 3;
        private int _maxX;
        private int _maxY;

        private List<CombatSimulationActor> _allActors = new List<CombatSimulationActor>();
        private List<CombatSimulationActor> _deadActors = new List<CombatSimulationActor>();
        private Dictionary<char, List<CombatSimulationActor>> _factions = new Dictionary<char, List<CombatSimulationActor>>();
        private CombatSimulationTile[] _allTiles;
        private CombatSimulationActor[] _unitPositions;
        private Dictionary<int, char> _debugTiles = new Dictionary<int, char>();

        private int _simulationStep;
        
        
        public CombatSimulation(string[] lines, int attackPower = -1)
        {
            int x = 0;
            int y = 0;
            if (attackPower > 0)
            {
                ElfAttackPower = attackPower;
            }
            List<CombatSimulationTile> tiles = new List<CombatSimulationTile>();
            foreach (var line in lines)
            {
                x = 0;
                foreach(var character in line)
                {
                    CombatSimulationActor actor = TryParseCombatActor(character, x, y);
                    CombatSimulationTile tile;
                    if(actor != null)
                    {
                        _allActors.Add(actor);
                        List<CombatSimulationActor> myTeam;
                        if (!_factions.TryGetValue(actor.Faction, out myTeam))
                        {
                            myTeam = new List<CombatSimulationActor>();
                            _factions[actor.Faction] = myTeam;
                        }
                        myTeam.Add(actor);
                        tile = ParseTile(EmptyTile, x, y);
                    }
                    else
                    {
                        tile = ParseTile(character, x, y);
                    }
                    tiles.Add(tile);
                    x++;
                }
                _maxX = x;
                y++;
            }
            
            _maxY = y;

				
            _allTiles = new CombatSimulationTile[tiles.Count];
            foreach (var tile in tiles)
            {
                tile.TileIndex = GetTileIndex(tile.X, tile.Y);
                _allTiles[tile.TileIndex] = tile;
            }
            
            _unitPositions = new CombatSimulationActor[_allTiles.Length];
				
            foreach (var actor in _allActors)
            {
                actor.TileIndex = GetTileIndex(actor.X, actor.Y);
                _unitPositions[actor.TileIndex] = actor;
            }
        }

        public class BattleResult
        {
            public int ElfAliveCount;
            public int ElfParticipantCount;
            public char Victor;
        }

        public BattleResult DoSimulation(int numTurns = Int32.MaxValue)
        {
            BattleResult result = new BattleResult();
            result.ElfParticipantCount = this._factions['E'].Count;            
            RenderGameboard();
            for (int i = 0; i < numTurns; i++)
            {   
                _debugTiles.Clear();
                
                // Step 1, sort the actors by their current position
                _allActors.Sort(CompareActorsByLocation);

                bool actorReportedDone = false;
                // Step 2, ask each actor to act
                foreach (var actor in _allActors)
                {
                    if (!actor.Update())
                    {
                        actorReportedDone = true; 
                        break;
                    }
                }
                
                // Step 3, gather the dead and remove them from the gameboard
                _deadActors.Clear();
                foreach (var actor in _allActors)
                {
                    if (!actor.IsAlive)
                    {
                        _deadActors.Add(actor);
                    }
                }

                foreach (var actor in _deadActors)
                {
                    _allActors.Remove(actor);
                    if (_unitPositions[actor.TileIndex] == actor)
                    {
                        _unitPositions[actor.TileIndex] = null;
                    }

                    this._factions[actor.Faction].Remove(actor);
                }
                _simulationStep++;
                // Finally, render the resulting gameboard
                RenderGameboard();
                if (actorReportedDone)
                {
                    break;
                }
                
            }

            RenderCombatOver();
            result.ElfAliveCount = this._factions['E'].Count;
            foreach (var actor in _allActors)
            {
                if (actor.IsAlive)
                {
                    result.Victor = actor.Faction;
                    break;
                }
            }

            return result;
        }

        private void RenderCombatOver()
        {
            int sum = 0;
            foreach (var actor in _allActors)
            {
                if (actor.IsAlive)
                {
                    sum += actor.RemainingHealth;    
                }
                
            }

            int checksum = sum * (this._simulationStep - 1);
            Log.WriteLine("Combat finished on turn " + (this._simulationStep-1) + ", total HP was " + sum + " generating a final solution of " + checksum);
        }

        public void GetNearByFoes(CombatSimulationActor actor, List<CombatSimulationActor> scratchList)
        {
            scratchList.Clear();
            CombatSimulationActor foe;
            // North
            foe = GetActorAt(actor.X, actor.Y - 1);
            if (foe != null && foe.IsAlive && foe.Faction != actor.Faction)
            {
                scratchList.Add(foe);
            }
            // West
            foe = GetActorAt(actor.X - 1, actor.Y);
            if (foe != null && foe.IsAlive && foe.Faction != actor.Faction)
            {
                scratchList.Add(foe);
            }
            // East
            foe = GetActorAt(actor.X + 1, actor.Y);
            if (foe != null && foe.IsAlive && foe.Faction != actor.Faction)
            {
                scratchList.Add(foe);
            }
            // South
            foe = GetActorAt(actor.X, actor.Y + 1);
            if (foe != null && foe.IsAlive && foe.Faction != actor.Faction)
            {
                scratchList.Add(foe);
            }
        }

        public void GetAllFoes(CombatSimulationActor actor, List<CombatSimulationActor> scratchList)
        {
            scratchList.Clear();
            foreach (var kvp in this._factions)
            {
                if (kvp.Key != actor.Faction)
                {
                    foreach (var foe in kvp.Value)
                    {
                        if (foe.IsAlive)
                        {
                            scratchList.Add(foe);    
                        }
                            
                    }
                    
                }
            }
        }
        
        private int CompareActorsByLocation(CombatSimulationActor a, CombatSimulationActor b)
        {
            int rowCompare = a.Y.CompareTo(b.Y);
            if (rowCompare != 0)
            {
                return rowCompare;
            }

            return a.X.CompareTo(b.X);
        }

        public void DebugTiles(List<CombatSimulationTile> tiles, char c)
        {
            foreach (var tile in tiles)
            {
                DebugTile(tile.X, tile.Y, c);
            }
        }
        
        public void DebugTile(CombatSimulationTile tile, char c)
        {
            DebugTile(tile.X, tile.Y, c);
        }

        public void DebugTile(int tileX, int tileY, char c)
        {
            _debugTiles[GetTileIndex(tileX, tileY)] = c;
        }

        public void MoveActorToTile(CombatSimulationActor actor, CombatSimulationTile tile)
        {
            _unitPositions[actor.TileIndex] = null;
            actor.X = tile.X;
            actor.Y = tile.Y;
            actor.TileIndex = tile.TileIndex;
            _unitPositions[actor.TileIndex] = actor;
        }

        public CombatSimulationActor GetActorAt(int x, int y)
        {
            int index = GetTileIndex(x, y);
            return GetActorAtIndex(index);
        }
        
        private CombatSimulationActor GetActorAtIndex(int index)
        {
            return _unitPositions[index];
        }
        
        public CombatSimulationTile GetTileAt(int x, int y)
        {
            int index = GetTileIndex(x, y);
            return GetTileAtIndex(index);
        }
        
        public CombatSimulationTile GetTileAtIndex(int index)
        {
            return _allTiles[index];
        }


        public int GetTileIndex(int x, int y)
        {
            return (_maxX * y) + x;
        }
        
        private bool IsKnownFaction(char t)
        {
            return t == ElfFaction || t == GoblinFaction;
        }
        
        private CombatSimulationActor TryParseCombatActor(char tile, int x, int y)
        {
            CombatSimulationActor res = null;
            if (!IsKnownFaction(tile))
            {
                return res;
            }

            int num = 0;
            List<CombatSimulationActor> team;
            if (this._factions.TryGetValue(tile, out team))
            {
                num = team.Count;
            }
            num++;
            int pow = GoblinAttackPower;
            if (tile == 'E')
            {
                pow = ElfAttackPower;
            }
            res = new CombatSimulationActor(this, tile, num, StartingHealth, pow, x, y);
            return res;
        }

        
        private CombatSimulationTile ParseTile(char tile, int x, int y)
        {
            CombatSimulationTile res = new CombatSimulationTile();
            if (tile == ObstructionTile)
            {
                res.TileType = CombatSimulationTileType.Obstruction;
            }
            else
            {
                res.TileType = CombatSimulationTileType.Empty;
            }

            res.OriginalChar = tile;
            res.X = x;
            res.Y = y;
            return res;
        }

        public void RenderPath(List<CombatSimulationTile> tiles)
        {
            this._debugTiles.Clear();
            this.DebugTiles(tiles, '^');
            RenderGameboard(true);
        }

        public void RenderGameboard( bool midStep = false)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Environment.NewLine);
            sb.Append("Turn Number - " + _simulationStep);
            if (midStep)
            {
                sb.Append(Environment.NewLine);
                sb.Append("Mid step");
                sb.Append(Environment.NewLine);
            }
            sb.Append(Environment.NewLine);
            for (int y = 0; y < _maxY; y++)
            {
                for (int x = 0; x < _maxX; x++)
                {
                    int tileIndex = GetTileIndex(x, y);
                    char debug;
                    if (_debugTiles.TryGetValue(tileIndex, out debug))
                    {
                        sb.Append(debug);
                        continue;
                    }
                    var actor = GetActorAtIndex(tileIndex);
                    if (actor != null)
                    {
                        sb.Append(actor.Render());
                    }
                    else
                    {
                        CombatSimulationTile tile = GetTileAtIndex(tileIndex);
                        sb.Append(tile.Render());
                    }
						
                }

                sb.Append(' ');
                sb.Append(' ');
                var actorInRowCount = 0;
                for (int x = 0; x < _maxX; x++)
                {
                    int tileIndex = GetTileIndex(x, y);
                    var actor = GetActorAtIndex(tileIndex);
                    if (actor != null)
                    {
                        if (actorInRowCount > 0)
                        {
                            sb.Append(',');
                            sb.Append(' ');
                        }

                        

                        sb.Append(actor.Id);
                        if (actor.Id == CombatSimulationActor.DebugActorId)
                        {
                            sb.Append('*');
                        }
                        sb.Append("(" + actor.RemainingHealth + ")");
                        actorInRowCount++;
                    }
                }

                sb.Append(Environment.NewLine);
            }

            //Log.WriteLine(sb);
        }

        public void GetUnoccupiedTilesNearActor(CombatSimulationActor actor, List<CombatSimulationTile> scratchTiles)
        {
            GetUnoccupiedTilesNearTile(actor.Tile, scratchTiles);
        }

        public void GetUnoccupiedTilesNearTile(CombatSimulationTile subject, List<CombatSimulationTile> scratchTiles)
        {
            scratchTiles.Clear();
            // North
            CombatSimulationActor foe = GetActorAt(subject.X, subject.Y - 1);
            if (foe == null || !foe.IsAlive)
            {
                var tile = this.GetTileAt(subject.X, subject.Y - 1);
                if (!tile.IsObstruction)
                {
                    scratchTiles.Add(tile);
                }
            }
            // West
            foe = GetActorAt(subject.X - 1, subject.Y);
            if (foe == null || !foe.IsAlive)
            {
                var tile = this.GetTileAt(subject.X - 1, subject.Y);
                if (!tile.IsObstruction)
                {
                    scratchTiles.Add(tile);
                }
            }
            // East
            foe = GetActorAt(subject.X + 1, subject.Y);
            if (foe == null || !foe.IsAlive)
            {
                var tile = this.GetTileAt(subject.X + 1, subject.Y);
                if (!tile.IsObstruction)
                {
                    scratchTiles.Add(tile);
                }
            }
            // South
            foe = GetActorAt(subject.X, subject.Y + 1);
            if (foe == null || !foe.IsAlive)
            {
                var tile = this.GetTileAt(subject.X, subject.Y + 1);
                if (!tile.IsObstruction)
                {
                    scratchTiles.Add(tile);
                }
            }
        }
    }
}