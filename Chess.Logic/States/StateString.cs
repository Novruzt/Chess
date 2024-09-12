using Chess.Logic.Enums;
using Chess.Logic.Pieces.Abstract;
using System.Text;

namespace Chess.Logic.States;
public class StateString
{
    private readonly StringBuilder sb = new();

    public StateString(Player currentPlayer, Board board)
    {
        AddPiecePlacement(board);
        sb.Append(' ');
        AddCurrentPlayer(currentPlayer);
        sb.Append(' ');
        AddCastlingRights(board);
        sb.Append(' ');
        AddEndPassant(board, currentPlayer);
    }

    public override string ToString()
    {
        return sb.ToString();
    }

    private static char PieceChar(Piece piece)
    {
        char c = piece.Type switch
        {
            PieceType.Pawn => 'p',
            PieceType.Knight => 'n',
            PieceType.Rook => 'r',
            PieceType.Bishop => 'b',
            PieceType.Queen => 'q',
            PieceType.King => 'k',
            _=> ' ' 
        };

        if(piece.Color==Player.White)
            return char.ToUpper(c);
        return c;
    }

    private void AddRowData(Board board, int row)
    {
        int empty = 0;

        for(int col = 0; col<8; col++)
        {
            if (board[row, col] == null)
            {
                empty++;
                continue;
            }

            if(empty > 0)
            {
                sb.Append(empty);
                empty = 0;
            }

            sb.Append(PieceChar(board[row, col]));
        }

        if (empty > 0)
        {
            sb.Append(empty);
        }
    }

    private void AddPiecePlacement(Board board)
    {
        for(int row = 0; row<8; row++)
        {
            if(row!= 0)
                sb.Append('/');

            AddRowData(board, row);
        }
    }

    private void AddCurrentPlayer(Player current)
    {
        if (current == Player.White)
            sb.Append('w');
        else if (current == Player.Black)
            sb.Append('b');
    }

    private void AddCastlingRights(Board board)
    {
        bool castleWKS = board.CastleRightKS(Player.White);
        bool castleWQS = board.CastleRightQS(Player.White);

        bool castleBKS = board.CastleRightKS(Player.Black);
        bool castleBQS = board.CastleRightQS(Player.Black);

        if(!(castleWKS || castleWQS || castleBKS || castleBQS))
        {
            sb.Append('-');
            return;
        }

        if (castleWKS)
            sb.Append('K');
        if (castleWQS)
            sb.Append('Q');
        if (castleBKS)
            sb.Append('k');
        if (castleBQS)
            sb.Append('q');
    }

    private void AddEndPassant(Board board, Player current)
    {
        if (!board.CanCaptureEnPassant(current))
        {
            sb.Append('-');
            return;
        }

        Position pos = board.GetPawnSkipPosition(current.Opponent());

        char file = (char)('a' + pos.Column);
        int rank = 8-pos.Row;

        sb.Append(file);
        sb.Append(rank);
    }
}
