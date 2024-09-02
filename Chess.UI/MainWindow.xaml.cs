using Chess.Logic;
using Chess.Logic.Enums;
using Chess.Logic.Moves.Abstract;
using Chess.Logic.Pieces.Abstract;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Rectangle = System.Windows.Shapes.Rectangle;

namespace Chess.UI;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly Image[,] pieceImages = new Image[8,8];
    private readonly Rectangle[,] highlights = new Rectangle[8,8];
    private readonly Dictionary<Position, Move> moveCache = new();

    private GameState gameStates;
    private Position? selectedPosition = null;

    public MainWindow()
    {
        InitializeComponent();
        InitalizeBoard();

        gameStates = new(Player.White, Board.Inital());
        DrawBoard(gameStates.Board);

        SetCursor(gameStates.CurrentPlayer);
    }

    private void InitalizeBoard()
    {
        for(int row = 0; row<8; row++)
        {
            for(int col = 0; col<8; col++)
            {
                Image Image = new();
                pieceImages[row, col] = Image;
                PieceGrid.Children.Add(Image);

                Rectangle highlight = new();
                highlights[row, col] = highlight;
                HighlightGrid.Children.Add(highlight);
            }
        }
    }

    private void DrawBoard(Board board)
    {
        for(int row = 0; row<8; row++)
        {
            for(int col=0; col<8; col++)
            {
                Piece piece = board[row,col];
                pieceImages[row,col].Source = Images.GetImage(piece);
            }
        }
    }

    private void BoardGrid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        Point point = e.GetPosition(BoardGrid);

        Position pos =ToSquarePosition(point);

        if (selectedPosition is null)
            OnFromPositionSelected(pos);
        else
            OnToPositionSelected(pos);
    }

    private Position ToSquarePosition(Point point)
    {
        double squareSize = BoardGrid.ActualWidth / 8;
        
        int row = (int)(point.Y / squareSize);
        int col = (int)(point.X / squareSize);  

        return new Position(row, col);
    }

    private void OnFromPositionSelected(Position pos)
    {
        IEnumerable<Move> moves = gameStates.LegalMovesForPiece(pos);

        if (moves.Any())
        {
            selectedPosition = pos;
            CacheMoves(moves);
            ShowHighlights();
        }
    }

    private void OnToPositionSelected(Position pos)
    {
        selectedPosition = null;
        HideHighlights();

        if(moveCache.TryGetValue(pos, out Move move))
        {
            gameStates.MakeMove(move);
            DrawBoard(gameStates.Board);
            SetCursor(gameStates.CurrentPlayer);
        }
    }

    private void CacheMoves(IEnumerable<Move> moves)
    {
        moveCache.Clear();

        foreach(Move move in moves)
        {
            moveCache[move.To] = move;
        }
    }

    private void ShowHighlights()
    {
        Color color = Color.FromArgb(150, 125, 255, 125);

        foreach(Position to in moveCache.Keys)
            highlights[to.Row, to.Column].Fill = new SolidColorBrush(color);
    }

    private void HideHighlights()
    {
        foreach (Position to in moveCache.Keys)
            highlights[to.Row, to.Column].Fill = Brushes.Transparent;
    }

    private void SetCursor(Player player)
    {
        if(player == Player.White)
            Cursor = ChessCursor.White;
        else 
            Cursor = ChessCursor.Black;
    }
}