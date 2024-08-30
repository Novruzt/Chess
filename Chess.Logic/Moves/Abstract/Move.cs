using Chess.Logic.Enums;

namespace Chess.Logic.Moves.Abstract;
public abstract class Move
{
    public abstract MoveType Type { get; }
    public abstract Position From { get; }
    public abstract Position To { get; }

    public abstract void  Execute(Board board);
}
