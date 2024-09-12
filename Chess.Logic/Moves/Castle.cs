using Chess.Logic.Enums;
using Chess.Logic.Moves.Abstract;

namespace Chess.Logic.Moves;
public class Castle : Move
{

    private readonly Direction kingMoveDir;
    private readonly Position rookFrom;
    private readonly Position rookTo;

    public override MoveType Type { get; }

    public override Position From { get; }

    public override Position To { get; }


    public Castle(MoveType type, Position kingPos)
    {
        Type = type;
        From = kingPos;

        if (type == MoveType.CastleKS)
        {
            kingMoveDir = Direction.East;
            To = new(kingPos.Row, 6);
            rookFrom = new(kingPos.Row, 7);
            rookTo = new(kingPos.Row, 5);
        }
        else if (type == MoveType.CastleQS)
        {
            kingMoveDir = Direction.West;
            To = new(kingPos.Row, 2);
            rookFrom = new(kingPos.Row, 0);
            rookTo = new(kingPos.Row, 3);
        }
    }
    public override bool Execute(Board board)
    {
       new NormalMove(From, To).Execute(board);
       new NormalMove(rookFrom, rookTo).Execute(board);

       return false;
    }

    public override bool IsLegal(Board board)
    {
        Player player = board[From].Color;

        if (board.IsInCheck(player))
            return false;
        
        Board copy = board.Copy();
        Position kingPosInCopy = From;

        for(int i = 0; i<2; i++)
        {
            new NormalMove(kingPosInCopy, kingPosInCopy + kingMoveDir).Execute(copy);
            kingPosInCopy += kingMoveDir;

            if (copy.IsInCheck(player))
                return false;
        }

        return true;
    }
}
