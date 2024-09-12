using Chess.Logic.Enums;
using Chess.Logic.GameEnds;
using Chess.Logic.Moves.Abstract;
using Chess.Logic.Pieces.Abstract;

namespace Chess.Logic.States;
public class GameState
{
    private int noCaptureNorPawnMoves = 0;

    private string stateString;
    private readonly Dictionary<string, int> stateHistory = new();

    public Board Board { get; }
    public Player CurrentPlayer { get; private set; }
    public Result Result { get; private set; } = null;

    public GameState(Player currentPlayer, Board board)
    {
        Board = board;
        CurrentPlayer = currentPlayer;

        stateString = new StateString(CurrentPlayer, board).ToString();
        stateHistory[stateString] = 1;
    }

    public bool IsGameOver() => Result != null;

    public IEnumerable<Move> LegalMovesForPiece(Position pos)
    {
        if (Board.IsEmpty(pos) || Board[pos].Color != CurrentPlayer)
            return Enumerable.Empty<Move>();

        Piece piece = Board[pos];
        IEnumerable<Move> moveCandicates = piece.GetMoves(pos, Board);

        return moveCandicates.Where(move => move.IsLegal(Board));
    }

    public void MakeMove(Move move)
    {
        Board.SetPawnSkipPosition(CurrentPlayer, null);
        bool capturePown = move.Execute(Board);

        if (capturePown)
        {
            noCaptureNorPawnMoves = 0;
            stateHistory.Clear();
        }          
        else
            noCaptureNorPawnMoves++;

        CurrentPlayer = CurrentPlayer.Opponent();
        UpdateStateString();

        CheckForGameOver();
    }

    public IEnumerable<Move> AllLegalMovesFor(Player player)
    {
        IEnumerable<Move> moveCandidates = Board.PiecePositionsFor(player).SelectMany(pos =>
        {
            Piece piece = Board[pos];
            return piece.GetMoves(pos, Board);
        });

        return moveCandidates.Where(move => move.IsLegal(Board));
    }

    private void CheckForGameOver()
    {
        if (!AllLegalMovesFor(CurrentPlayer).Any())
        {
            if (Board.IsInCheck(CurrentPlayer))
                Result = Result.Win(CurrentPlayer.Opponent());
            else
                Result = Result.Draw(EndReason.Stalemate);
        }

        else if (Board.InsufficientMaterial())
            Result = Result.Draw(EndReason.InsufficientMaterial);
        else if (FiftyMoveRule())
            Result = Result.Draw(EndReason.FiftyMove);
        else if (ThreeFoldRepetition())
            Result = Result.Draw(EndReason.ThreeFoldRepetition);
    }

    private bool FiftyMoveRule()
    {
        int fullMoves = noCaptureNorPawnMoves / 2;

        return fullMoves == 50;
    }

    private bool ThreeFoldRepetition()
    {
        return stateHistory[stateString] == 3;
    }

    private void UpdateStateString()
    {
        stateString = new StateString(CurrentPlayer, Board).ToString();

        if (!stateHistory.ContainsKey(stateString))
            stateHistory[stateString] = 1;
        else 
            stateHistory[stateString]++;
    }
}
