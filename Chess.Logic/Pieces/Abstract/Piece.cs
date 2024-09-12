using Chess.Logic.Enums;
using Chess.Logic.Moves.Abstract;

namespace Chess.Logic.Pieces.Abstract;
public abstract class Piece
{
    public abstract PieceType Type { get; }
    public abstract Player Color { get; }
    public bool HasMoved { get; set; } = false;

    public abstract Piece Copy();
    public abstract IEnumerable<Move> GetMoves(Position from, Board board);

    public virtual bool CanCaptureOpponentKing(Position from, Board board)
    {
        return GetMoves(from, board).Any(move =>
        {
            Piece piece = board[move.To];
            return piece != null && piece.Type==PieceType.King; 
        });
    }

    protected IEnumerable<Position> MovePositionInDirection(Position from, Board board, Direction dir)
    {
        for (Position pos = from + dir; Board.IsInside(pos); pos += dir)
        {
            if (board.IsEmpty(pos))
            {
                yield return pos;
                continue;
            }

            Piece piece = board[pos];
            if (piece.Color != Color)
            {
                yield return pos;
            }

            yield break;
        }
    }

    protected IEnumerable<Position> MovePositionInDirections(Position from, Board board, Direction[] dirs)
    { 
        var directions = dirs.SelectMany(dir => MovePositionInDirection(from, board, dir));
        return  directions;
    }
}
