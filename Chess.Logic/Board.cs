using Chess.Logic.Enums;
using Chess.Logic.Moves;
using Chess.Logic.Pieces;
using Chess.Logic.Pieces.Abstract;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;

namespace Chess.Logic;
public class Board
{
    private readonly Piece[,] pieces = new Piece[8, 8];

    private readonly Dictionary<Player, Position> pawnSkipPositions = new()
    {
        { Player.White, null },
        { Player.Black, null }
    };
    
    public Piece this[int row, int column]
    {
        get { return pieces[row, column]; }
        set { pieces[row, column] = value; }
    }

    public Piece this[Position pos]
    {
        get { return pieces[pos.Row, pos.Column]; }
        set { this[pos.Row, pos.Column] = value; }
    }

    public static Board Inital()
    {
        Board board = new();
        board.AddStartPieces();

        return board;
     }

    #region Positional methods

    public IEnumerable<Position> PiecePositions()
    {
        for(int row =  0; row < 8; row++)
        {
            for(int col = 0; col<8; col++)
            {
                Position pos = new Position(row, col);

                if (!IsEmpty(pos))
                {
                    yield return pos;
                }
            }
        }
    }

    public Position GetPawnSkipPosition(Player player)
    {
        return pawnSkipPositions[player];
    }

    public void SetPawnSkipPosition(Player player, Position pos)
    {
        pawnSkipPositions[player] = pos;
    }

    public IEnumerable<Position> PiecePositionsFor(Player player)
    {
        return PiecePositions().Where(pos => this[pos].Color == player);
    }

    public bool IsInCheck(Player player)
    {
        return PiecePositionsFor(player.Opponent()).Any(pos =>
        {
            Piece piece = this[pos];
            return piece.CanCaptureOpponentKing(pos, this);
        });
    }

    public Board Copy()
    {
        Board copy = new();

        foreach(Position pos in PiecePositions())
        {
            copy[pos] = this[pos].Copy();
        }

        return copy;
    }

    public static bool IsInside(Position pos)
    {
        return pos.Row >= 0 && pos.Row < 8 && pos.Column >= 0 && pos.Column < 8;
    }

    public bool IsEmpty(Position pos)
    {
        return this[pos] == null;
    }

    #endregion

    #region Insufficient Materials
    public Counting CountPieces()
    {
        Counting counting = new();

        foreach (Position pos in PiecePositions())
        {
            Piece piece = this[pos];
            counting.Increment(piece.Color, piece.Type);
        }

        return counting;
    }

    public bool InsufficientMaterial()
    {
        Counting counting = CountPieces();

        return IsOnlyKings(counting) || IsBishops(counting) || IsKingKnightVsKing(counting) || IsKingKnightVsKing(counting);
    }

    private static bool IsOnlyKings(Counting counting)
    {
        return counting.TotalCount == 2;
    }

    private bool IsBishops(Counting counting)
    {  
        if(counting.TotalCount == 4)
            if(counting.White(PieceType.Bishop) == 1 && counting.Black(PieceType.Bishop) == 1)
            {
                Position wBishopPos = PiecePositionsFor(Player.White).First(pos => this[pos].Type == PieceType.Bishop);
                Position bBishopPos = PiecePositionsFor(Player.Black).First(pos => this[pos].Type == PieceType.Bishop);

                return wBishopPos.SquareColor() == bBishopPos.SquareColor();
            }

        return counting.TotalCount == 3 && (counting.White(PieceType.Bishop) == 1 || counting.Black(PieceType.Bishop) == 1);
    }

    private static bool IsKingKnightVsKing(Counting counting)
    {
        return counting.TotalCount == 3 && (counting.White(PieceType.Knight) == 1 || counting.Black(PieceType.Knight) == 1);
    }

    #endregion

    #region ThreeFold

    public bool CastleRightKS(Player player)
    {
        return player switch
        {
            Player.White => IsUnmovedKingAndRook(new Position(7, 4), new Position(7, 7)),
            Player.Black => IsUnmovedKingAndRook(new Position(0, 4), new Position(0, 7)),
            _ => false
        };
    }

    public bool CastleRightQS(Player player)
    {
        return player switch
        {
            Player.White => IsUnmovedKingAndRook(new Position(7, 4), new Position(7, 0)),
            Player.Black => IsUnmovedKingAndRook(new Position(0, 4), new Position(0, 0)),
            _ => false
        };
    }

    public bool CanCaptureEnPassant(Player player)
    {
        Position skipPos = GetPawnSkipPosition(player.Opponent());

        if (skipPos == null)
            return false;

        Position[] pawnPositions = player switch
        {
            Player.White => new Position[] { skipPos + Direction.SouthEast, skipPos + Direction.SouthWest },
            Player.Black =>  new  Position[] { skipPos + Direction.NorthEast, skipPos + Direction.NorthWest },
            _=>Array.Empty<Position>()
        };

        return HasPawnInPosition(player, pawnPositions, skipPos);
    }

    private bool HasPawnInPosition(Player player, Position[] pawnPositions, Position skipPos)
    {
        foreach (Position pos in pawnPositions.Where(IsInside))
        {
            Piece piece = this[pos];

            if (piece is null || piece.Color != player || piece.Type != PieceType.Pawn)
                continue;

            EnPassant move = new(pos, skipPos);

            if (move.IsLegal(this))
                return true;
        }

        return false;
    }

    private bool IsUnmovedKingAndRook(Position kingPos, Position rookPos)
    {
        if(IsEmpty(kingPos) || IsEmpty(rookPos))
            return false;

        Piece king = this[kingPos];
        Piece rook = this[rookPos];

        return king.Type == PieceType.King && rook.Type==PieceType.Rook &&
               !king.HasMoved && !rook.HasMoved;
    }

    #endregion

    private void AddStartPieces()
    {

        #region First Black Row
        this[0, 0] = new Rook(Player.Black);
        this[0, 1] = new Knight(Player.Black);
        this[0, 2] = new Bishop(Player.Black);
        this[0, 3] = new Queen(Player.Black);
        this[0, 4] = new King(Player.Black);
        this[0, 5] = new Bishop(Player.Black);
        this[0, 6] = new Knight(Player.Black);
        this[0, 7] = new Rook(Player.Black);
        #endregion

        #region First White Row
        this[7, 0] = new Rook(Player.White);
        this[7, 1] = new Knight(Player.White);
        this[7, 2] = new Bishop(Player.White);
        this[7, 3] = new Queen(Player.White);
        this[7, 4] = new King(Player.White);
        this[7, 5] = new Bishop(Player.White);
        this[7, 6] = new Knight(Player.White);
        this[7, 7] = new Rook(Player.White);
        #endregion

        for (int i = 0; i < 8; i++)
        {
            this[1, i] = new Pawn(Player.Black);
            this[6, i] = new Pawn(Player.White);
        }
    }

    public Position? GetKingPosition(Player currentPlayer)
    {
        int kingRow, kingCol;   
        for(int row = 0; row< 8; row++)
        {
            for(int col = 0; col< 8; col++)
            {
                Piece piece = this[row, col];

                if (piece is null)
                    continue;

                if(piece.Type==PieceType.King && piece.Color == currentPlayer)
                {
                    return new(row, col);
                }                  
            }
        }

        return null;
    }
}
