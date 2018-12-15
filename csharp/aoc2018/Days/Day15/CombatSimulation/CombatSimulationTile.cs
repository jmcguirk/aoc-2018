namespace AdventOfCode.Combat
{
    public class CombatSimulationTile
    {
        public int X;
        public int Y;
        public int TileIndex;
        public char OriginalChar;
        public CombatSimulationTileType TileType;

        public char Render()
        {
            return OriginalChar;
        }

        public bool IsObstruction => TileType == CombatSimulationTileType.Obstruction;
    }

    public enum CombatSimulationTileType
    {
        Empty,
        Obstruction
    }
    
    
    
}