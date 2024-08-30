using Chess.Logic.Enums;
using Chess.Logic.Pieces;
using Chess.Logic.Pieces.Abstract;
using System.ComponentModel;

namespace Chess.Logic;
public class Board
{
    private readonly Piece[,] pieces = new Piece[8, 8];
    
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

        for(int i = 0; i<8; i++)
        {
            this[1, i] = new Pawn(Player.Black);
            this[6, i] = new Pawn(Player.White);
        }
    }

    public static bool IsInside(Position pos)
    {
        return pos.Row>=0 && pos.Row<8 && pos.Column>=0 && pos.Column<8;
    }

    public bool IsEmpty(Position pos)
    {
        return this[pos] == null;
    }
}
