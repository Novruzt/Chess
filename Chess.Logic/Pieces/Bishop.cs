using Chess.Logic.Enums;
using Chess.Logic.Moves;
using Chess.Logic.Moves.Abstract;
using Chess.Logic.Pieces.Abstract;

namespace Chess.Logic.Pieces;
public class Bishop : Piece
{
    private static readonly Direction[] directions =
    {
        Direction.NorthEast,
        Direction.NorthWest,
        Direction.SouthEast,
        Direction.SouthWest,
    };

    public override PieceType Type => PieceType.Bishop;
    public override Player Color { get; }

    public Bishop(Player color)
    {
        Color = color;
    }

    public override Piece Copy()
    {
        Bishop copy = new(Color);
        copy.HasMoved= HasMoved;

        return copy;
    }

    public override IEnumerable<Move> GetMoves(Position from, Board board)
    {
        return MovePositionInDirections(from, board, directions)
              .Select(to=>new NormalMove(from, to));
    }
}
