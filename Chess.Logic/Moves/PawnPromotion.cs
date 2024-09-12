using Chess.Logic.Enums;
using Chess.Logic.Moves.Abstract;
using Chess.Logic.Pieces;
using Chess.Logic.Pieces.Abstract;

namespace Chess.Logic.Moves;
public class PawnPromotion : Move
{
    private readonly PieceType newType;

    public override MoveType Type => MoveType.PawnPromotion;
    public override Position From { get; }
    public override Position To { get; }

    public PawnPromotion(Position from, Position to, PieceType newType)
    {    
        From = from;
        To = to;
        this.newType = newType;
    }

    public override bool Execute(Board board)
    {
        Piece pawn = board[From];
        board[From] = null;

        Piece promotionPiece = CreatePromotionPiece(pawn.Color);
        promotionPiece.HasMoved = true;
        board[To] = promotionPiece;

        return true;
    }

    private Piece CreatePromotionPiece(Player color)
    {
        return newType switch
        {
            PieceType.Knight =>   new Knight(color),
            PieceType.Bishop =>   new Bishop(color),
            PieceType.Rook   =>   new Rook(color),
            _ => new Queen(color),
        };
    }
}
