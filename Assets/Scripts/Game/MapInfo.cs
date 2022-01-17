using System.Collections.Generic;

public class MapInfo
{
    public int bigLevelID, levelID;
    public List<Grids.GridState> gridPoints = new List<Grids.GridState>();
    public List<Grids.GridPosIndex> monsterPath = new List<Grids.GridPosIndex>();
    public List<Round.RoundInfo> roundInfo = new List<Round.RoundInfo>();
}
