using Chess.Logic.Enums;
using Chess.Logic.Moves.Abstract;
using Chess.Logic.Pieces.Abstract;

namespace Chess.Logic;
public class GameStates
{
    public Board Board { get; }
    public Player CurrentPlayer { get; private set; }

    public GameStates(Player currentPlayer, Board board)
    {
        Board = board;
        CurrentPlayer = currentPlayer;
    }

    public IEnumerable<Move> LegalMovesForPiece(Position pos)
    {
        if (Board.IsEmpty(pos) || Board[pos].Color!= CurrentPlayer)
            return Enumerable.Empty<Move>();

        Piece piece = Board[pos];

        return piece.GetMoves(pos, Board);
    }

    public void MakeMove(Move move)
    {
        move.Execute(Board);
        CurrentPlayer = CurrentPlayer.Opponent();
    }
}
