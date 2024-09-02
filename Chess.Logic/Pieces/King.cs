using Chess.Logic.Enums;
using Chess.Logic.Moves;
using Chess.Logic.Moves.Abstract;
using Chess.Logic.Pieces.Abstract;

namespace Chess.Logic.Pieces;
public class King:Piece
{
    private Direction[] directions =
    {
        Direction.North,
        Direction.East,
        Direction.South,
        Direction.West,

        Direction.NorthEast,
        Direction.NorthWest,

        Direction.SouthEast,
        Direction.SouthWest,
    };
    public override PieceType Type => PieceType.King;
    public override Player Color { get; }

    public King(Player color)
    {
        Color = color;
    }

    public override Piece Copy()
    {
        King copy = new(Color);
        copy.HasMoved = HasMoved;

        return copy;
    }

    public override IEnumerable<Move> GetMoves(Position from, Board board)
    {
        foreach (Position to in MovePosition(from, board))
            yield return new NormalMove(from, to);
    }

    public override bool CanCaptureOpponentKing(Position from, Board board)
    {
        return MovePosition(from, board).Any(to =>
        {
            Piece piece = board[to];
            return piece != null && piece.Type== PieceType.King;
        });
    }

    private IEnumerable<Position> MovePosition(Position from, Board board)
    {
        foreach(Direction dir in directions)
        {
            Position to = from + dir;

            if (!Board.IsInside(to))
                continue;

            if (board.IsEmpty(to) || board[to].Color != Color)
                yield return to;
        }
    }
}
