using Chess.Logic.Enums;
using Chess.Logic.Moves;
using Chess.Logic.Moves.Abstract;
using Chess.Logic.Pieces.Abstract;

namespace Chess.Logic.Pieces;
public class Rook:Piece
{
    private Direction[] directions =
    {
        Direction.East,
        Direction.West,
        Direction.North, 
        Direction.South, 
    };
    public override PieceType Type => PieceType.Rook;
    public override Player Color { get; }

    public Rook(Player color)
    {
        Color = color;
    }

    public override Piece Copy()
    {
        Rook copy = new(Color);
        copy.HasMoved = HasMoved;

        return copy;
    }

    public override IEnumerable<Move> GetMoves(Position from, Board board)
    {
        return MovePositionInDirections(from, board, directions).Select(to=>new NormalMove(from, to));
    }
}
