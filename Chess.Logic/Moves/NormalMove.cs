using Chess.Logic.Enums;
using Chess.Logic.Moves.Abstract;
using Chess.Logic.Pieces.Abstract;

namespace Chess.Logic.Moves;
public class NormalMove : Move
{
    public override MoveType Type => MoveType.Normal;

    public override Position From { get; }

    public override Position To { get; }


    public NormalMove(Position from, Position to)
    {
        From = from;
        To = to;
    }

    public override bool Execute(Board board)
    {
        Piece piece = board[From];
        bool capture = !board.IsEmpty(To);
        board[To] = piece;

        board[From] = null;
        piece.HasMoved = true;

        return capture || piece.Type== PieceType.Pawn;
    }
}
