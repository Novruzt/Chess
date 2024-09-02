using Chess.Logic.Enums;
using Chess.Logic.Moves.Abstract;
using Chess.Logic.Pieces.Abstract;

namespace Chess.Logic;
public class GameState
{
    public Board Board { get; }
    public Player CurrentPlayer { get; private set; }
    public Result Result { get; private set; } = null;

    public GameState(Player currentPlayer, Board board)
    {
        Board = board;
        CurrentPlayer = currentPlayer;
    }

    public bool IsGameOver() => Result != null;

    public IEnumerable<Move> LegalMovesForPiece(Position pos)
    {
        if (Board.IsEmpty(pos) || Board[pos].Color!= CurrentPlayer)
            return Enumerable.Empty<Move>();

        Piece piece = Board[pos];
        IEnumerable<Move> moveCandicates = piece.GetMoves(pos, Board);

        return moveCandicates.Where(move => move.IsLegal(Board ));
    }

    public void MakeMove(Move move)
    {
        move.Execute(Board);
        CurrentPlayer = CurrentPlayer.Opponent();

        CheckForGameOver();
    }

    public IEnumerable<Move> AllLegalMovesFor(Player player)
    {
        IEnumerable<Move> moveCandidates = Board.PiecePositionsForPlayer(player).SelectMany(pos =>
        {
            Piece piece = Board[pos];
            return piece.GetMoves(pos, Board);
        });

        return moveCandidates.Where(move=>move.IsLegal(Board));
    }
     
    private void CheckForGameOver()
    {
        if (!AllLegalMovesFor(CurrentPlayer).Any())
        {
            if(Board.IsInCheck(CurrentPlayer))
                Result = Result.Win(CurrentPlayer.Opponent());
            else
                Result = Result.Draw(EndReason.Stalemate); 
        }
    }
}
