using Chess.Logic.Enums;
using Chess.Logic.Moves;
using Chess.Logic.Moves.Abstract;
using Chess.Logic.Pieces.Abstract;

namespace Chess.Logic.Pieces;
public class King : Piece
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


    #region moves
    public override IEnumerable<Move> GetMoves(Position from, Board board)
    {
        foreach (Position to in MovePosition(from, board))
            yield return new NormalMove(from, to);

        if(CanCastleKingSide(from, board))
            yield return new Castle(MoveType.CastleKS, from);

        if(CanCastleQueenSide(from, board))
            yield return new Castle(MoveType.CastleQS, from);
    }

    public override bool CanCaptureOpponentKing(Position from, Board board)
    {
        return MovePosition(from, board).Any(to =>
        {
            Piece piece = board[to];
            return piece != null && piece.Type == PieceType.King;
        });
    }

    private IEnumerable<Position> MovePosition(Position from, Board board)
    {
        foreach (Direction dir in directions)
        {
            Position to = from + dir;

            if (!Board.IsInside(to))
                continue;

            if (board.IsEmpty(to) || board[to].Color != Color)
                yield return to;
        }
    }
    #endregion

    #region Castling

    private static bool IsUnmovedRook(Position pos, Board board)
    {
        if (board.IsEmpty(pos))
            return false;

        Piece piece = board[pos];
        return piece.Type == PieceType.Rook && !piece.HasMoved;
    }

    public static bool AllEmpty(IEnumerable<Position> positions, Board board)
    {
        return positions.All(pos=>board.IsEmpty(pos));
    }

    private bool CanCastleKingSide(Position from, Board board)
    {
        if(HasMoved)
            return false;

        Position rookPos = new(from.Row, 7);
        Position[] between = { new(from.Row, 5), new(from.Row, 6) } ;

        return IsUnmovedRook(rookPos, board) && AllEmpty(between, board);
    }

    private bool CanCastleQueenSide(Position from, Board board)
    {
        if (HasMoved)
            return false;

        Position rookPos = new(from.Row, 0);
        Position[] between = { new(from.Row, 1), new(from.Row, 2), new(from.Row, 3) };

        return IsUnmovedRook(rookPos, board) && AllEmpty(between, board);
    }

    #endregion
}
