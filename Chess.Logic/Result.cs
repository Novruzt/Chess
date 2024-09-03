using Chess.Logic.Enums;

namespace Chess.Logic;
public class Result
{
    public Player Winner { get; set; }
    public EndReason Reason { get; set; }

    public Result(Player winner, EndReason reason)
    {
        Winner = winner;
        Reason = reason;
    }

    public static Result Win(Player winner)
    {
        return new(winner, EndReason.Checkmate);
    }

    public static Result Draw(EndReason reason)
    {
        return new(Player.None, reason);
    }
}
