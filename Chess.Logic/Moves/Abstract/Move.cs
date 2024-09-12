using Chess.Logic.Enums;
using Chess.Logic.Pieces.Abstract;

namespace Chess.Logic.Moves.Abstract;
public abstract class Move
{
    public abstract MoveType Type { get; }
    public abstract Position From { get; }
    public abstract Position To { get; }

    public abstract bool  Execute(Board board);

    public virtual bool IsLegal(Board board)
    {
        Piece piece = board[From];

        if(piece is null)
            return false;

        Player player = piece.Color;
        Board copy = board.Copy();
        Execute(copy);

        return !copy.IsInCheck(player);
    }
}
