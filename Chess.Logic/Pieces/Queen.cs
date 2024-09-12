using Chess.Logic.Enums;
using Chess.Logic.Moves.Abstract;
using Chess.Logic.Moves;
using Chess.Logic.Pieces.Abstract;

namespace Chess.Logic.Pieces;
public class Queen : Piece
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

    public override PieceType Type => PieceType.Queen;
    public override Player Color { get; }

    public Queen(Player color)
    {
        Color = color;
    }

    public override Piece Copy()
    {
        Queen copy = new(Color);
        copy.HasMoved = HasMoved;

        return copy;
    }

    public override IEnumerable<Move> GetMoves(Position from, Board board)
    {
        var getMoves = MovePositionInDirections(from, board, directions).Select(to => new NormalMove(from, to));
        return getMoves;
    }
}
