using Chess.Logic.Enums;
using Chess.Logic.Pieces.Abstract;
using System.Reflection.Metadata;

namespace Chess.Logic;
public class Counting
{
    private readonly Dictionary<PieceType, int> whiteCount = new();
    private readonly Dictionary<PieceType, int> blackCount = new();

    public int TotalCount { get; private set; }

    public Counting()
    {
        foreach(PieceType type in Enum.GetValues(typeof(PieceType)))
        {
            whiteCount[type] = 0;
            blackCount[type] = 0;
        }
    }

    public void Increment(Player color, PieceType Type)
    {
        if (color == Player.White)
            whiteCount[Type]++;
        else if(color == Player.Black)
            blackCount[Type]++;
       
        TotalCount++;
    }

    public int White(PieceType type)
    {
        return whiteCount[type];
    }

    public int Black(PieceType type)
    {
        return blackCount[type];
    }
}
