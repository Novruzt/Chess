using Chess.Logic.Enums;
using Chess.Logic.Moves;
using Chess.Logic.Moves.Abstract;
using Chess.Logic.Pieces.Abstract;

namespace Chess.Logic.Pieces;
public class Pawn : Piece
{
    private readonly Direction forward;

    public override PieceType Type => PieceType.Pawn;
    public override Player Color { get; }

    public Pawn(Player color)
    {
        Color = color;

        if (Color == Player.White)
            forward = Direction.North;
        else 
            forward = Direction.South;
    }
    public override Piece Copy()
    {
        Pawn copy = new(Color);
        copy.HasMoved = HasMoved;

        return copy;
    }
    public override IEnumerable<Move> GetMoves(Position from, Board board)
    {
        return ForwardMoves(from, board).Concat(DiagonalMoves(from, board));
    }

    public override bool CanCaptureOpponentKing(Position from, Board board)
    {
        return DiagonalMoves(from, board).Any(move =>
        {
            Piece piece = board[move.To];
            return piece != null && piece.Type== PieceType.King;
        });
    }

    private bool CanMoveTo(Position pos, Board board)
    {
        return Board.IsInside(pos) && board.IsEmpty(pos);
    }

    private bool CanCaptureAt(Position pos, Board board)
    {
        if(!Board.IsInside(pos) || board.IsEmpty(pos))
            return false;

        return board[pos].Color != Color;
    }

    private IEnumerable<Move> ForwardMoves(Position from, Board board)
    {
        Position oneMove = from + forward;

        if (CanMoveTo(oneMove, board))
        {
            yield return new NormalMove(from, oneMove);

            Position twoMove = oneMove + forward;

            if (!HasMoved && CanMoveTo(twoMove, board))
            {
                yield return new NormalMove(from, twoMove);
            }
        }
    }

    private IEnumerable<Move> DiagonalMoves(Position from, Board board)
    {
        Direction[] directions = new Direction[2];

        if (forward == Direction.North)
        {
            directions[0] = Direction.NorthEast;
            directions[1] = Direction.NorthWest;
        }
        else
        {
            directions[0] = Direction.SouthEast;
            directions[1] = Direction.SouthWest;
        }
       
        foreach (Direction dir in directions)
        {
            Position to  = from + dir;

            if(CanCaptureAt(to, board))
            {
                yield return new NormalMove(from, to);
            }
        }
    } 
}
