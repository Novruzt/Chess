using Chess.Logic.Enums;
using Chess.Logic.Moves.Abstract;

namespace Chess.Logic.Moves;
public class DoublePawn : Move
{
    private readonly Position skippedPosition;

    public override MoveType Type => MoveType.DoublePawn;
    public override Position From { get; }
    public override Position To { get; }

    public DoublePawn(Position from, Position to)
    {
        From = from;
        To = to;
        skippedPosition = new((from.Row + to.Row) / 2, from.Column);
    }
    public override bool Execute(Board board)
    {
        Player player = board[From].Color;
        board.SetPawnSkipPosition(player, skippedPosition);
        new NormalMove(From, To).Execute(board);

        return true;
    }
}
