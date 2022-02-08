using System.Collections.Generic;

public class MapInfo
{
    public int LevelGroupId;
    public int LevelId;
    public List<GridState> gridPoints = new List<GridState>();
    public List<GridPosIndex> monsterPath = new List<GridPosIndex>();
    public List<Round.RoundInfo> roundInfo = new List<Round.RoundInfo>();
}
