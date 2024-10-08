﻿using Chess.Logic.Enums;
using Chess.Logic.Moves;
using Chess.Logic.Moves.Abstract;
using Chess.Logic.Pieces.Abstract;

namespace Chess.Logic.Pieces;
public class Knight : Piece
{
    public override PieceType Type => PieceType.Knight;
    public override Player Color { get; }

    public Knight(Player color)
    {
        Color = color;
    }

    public override Piece Copy()
    {
        Knight copy = new(Color);
        copy.HasMoved=HasMoved;

        return copy;
    }

    public override IEnumerable<Move> GetMoves(Position from, Board board)
    {
        var movePoses = MovePositions(from, board).Select(to => new NormalMove(from, to));
        return movePoses;
    }

    private static IEnumerable<Position> PotentialToPositions(Position from)
    {
        foreach(Direction vDir in new Direction[] {Direction.North, Direction.South })
        {
            foreach(Direction hDir in new Direction[] { Direction.East, Direction.West })
            {
                yield return from+2*vDir+hDir;
                yield return from+2*hDir+vDir;
            }
        }
    }

    private IEnumerable<Position> MovePositions(Position from, Board board)
    {
        var MovePoses = PotentialToPositions(from).Where(pos => Board.IsInside(pos)
        && (board.IsEmpty(pos) || board[pos].Color != Color));
        return MovePoses;
    }
}
