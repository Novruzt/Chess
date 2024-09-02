namespace Chess.Logic.Enums;
public enum EndReason
{
    None = 0,
    CheckMate,
    Stalemate,
    FiftyMove,
    InsufficientMaterial,
    ThreeFoldRepetition
}
