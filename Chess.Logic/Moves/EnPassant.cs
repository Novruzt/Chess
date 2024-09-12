using Chess.Logic.Enums;
using Chess.Logic.Moves.Abstract;

namespace Chess.Logic.Moves;
public class EnPassant : Move
{
    private readonly Position capturePos;

    public override MoveType Type => MoveType.EnPassant;
    public override Position From { get; }
    public override Position To { get; }

    public EnPassant(Position from, Position to)
    {
        From = from;
        To = to;
        capturePos = new(from.Row, to.Column);
    }

    public override bool Execute(Board board)
    {
        new NormalMove(From, To).Execute(board);
        board[capturePos] = null;

        return true;
    }
}
