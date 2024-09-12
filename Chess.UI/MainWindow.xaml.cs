using Chess.Logic;
using Chess.Logic.Enums;
using Chess.Logic.Moves;
using Chess.Logic.Moves.Abstract;
using Chess.Logic.Pieces.Abstract;
using Chess.Logic.States;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using Point = System.Windows.Point;
using Color = System.Windows.Media.Color;

using Rectangle = System.Windows.Shapes.Rectangle;

namespace Chess.UI;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private Position? redPositionInCheck = null;

    private readonly Image[,] pieceImages = new Image[8,8];
    private readonly Rectangle[,] highlights = new Rectangle[8,8];
    private readonly Dictionary<Position, Move> moveCache = new();

    private GameState gameState;
    private Position? selectedPosition = null;

    public MainWindow()
    {
        InitializeComponent();
        InitalizeBoard();

        gameState = new(Player.White, Board.Inital());
        DrawBoard(gameState.Board);

        SetCursor(gameState.CurrentPlayer);
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

    private void BoardGrid_MouseDown(object sender, MouseButtonEventArgs e)
    {

        if (IsMenuOnScreen())
            return;

        Point point = e.GetPosition(BoardGrid);

        Position pos =ToSquarePosition(point);

        if (selectedPosition is null)
            OnFromPositionSelected(pos);
        else
            OnToPositionSelected(pos);

        if (gameState.Board.IsInCheck(gameState.CurrentPlayer))
            RedIfKingIsInCheck();
        else
            HideRed();
    }

    private Position ToSquarePosition(Point point)
    {
        double squareSize = BoardGrid.ActualWidth / 8;
        
        int row = (int)(point.Y / squareSize);
        int col = (int)(point.X / squareSize);  

        return new Position(row, col);
    }

    private Point PositionToPoint(Position pos)
    {
        double squareSize = BoardGrid.ActualWidth / 8;
        double x = pos.Column * squareSize;
        double y = pos.Row * squareSize;

        return new Point(x, y);
    }

    private void OnFromPositionSelected(Position pos)
    {
        IEnumerable<Move> moves = gameState.LegalMovesForPiece(pos);

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
            if (move.Type == MoveType.PawnPromotion)
                HandlePromotion(move.From, move.To);
            else
                HandleMove(move); 
        }
    }

    private void HandlePromotion(Position from, Position to)
    {
        pieceImages[to.Row, to.Column].Source = Images.GetImage(gameState.CurrentPlayer, PieceType.Pawn);
        pieceImages[from.Row, from.Column].Source = null;

        PromotionMenu promotionMenu = new(gameState.CurrentPlayer);
        MenuContainer.Content=promotionMenu;

        promotionMenu.PieceSelected += type =>
        {
            MenuContainer.Content = null;
            Move promotionMove = new PawnPromotion(from, to, type);
            HandleMove(promotionMove);
        };
    }

    private void HandleMove(Move move)
    {
        gameState.MakeMove(move);
        DrawBoard(gameState.Board);
        SetCursor(gameState.CurrentPlayer);

        if (gameState.IsGameOver())
            ShowGameOver();
    }

    private void CacheMoves(IEnumerable<Move> moves)
    {
        moveCache.Clear();

        foreach(Move move in moves)
        {
            moveCache[move.To] = move;
        }
    }
    #region RedKing
    private Position? GetKingPosition()
    {
        return gameState.Board.GetKingPosition(gameState.CurrentPlayer);
    }
    private void RedIfKingIsInCheck()
    {
        redPositionInCheck = GetKingPosition(); 
        Color color = Color.FromRgb(255, 0, 0);

        if (redPositionInCheck is not null)
            highlights[redPositionInCheck.Row, redPositionInCheck.Column].Fill = new SolidColorBrush(color);
    }

    private void HideRed()
    {
        if (redPositionInCheck is not null)
            highlights[redPositionInCheck.Row, redPositionInCheck.Column].Fill = Brushes.Transparent;
    }
    #endregion
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

    private bool IsMenuOnScreen()
    {
        return MenuContainer.Content != null;
    }

    private void ShowGameOver()
    {
        GameOverMenu gameOverMenu = new(gameState);
        MenuContainer.Content = gameOverMenu;

        gameOverMenu.OptionSelected += option =>
        {
            if (option == Option.Restart)
            {
                MenuContainer.Content = null;
                RestartGame();
            }
            else
                Application.Current.Shutdown();
        };
    }

    private void RestartGame()
    {
        selectedPosition = null;
        HideHighlights();
        moveCache.Clear();
        gameState = new(Player.White, Board.Inital());

        DrawBoard(gameState.Board);
        SetCursor(gameState.CurrentPlayer);
    }

    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        if (!IsMenuOnScreen() && e.Key == Key.Escape)
            ShowPauseMenu();
    }

    private void ShowPauseMenu()
    {
        PauseMenu pauseMenu = new();
        MenuContainer.Content = pauseMenu;

        pauseMenu.OptionSelected += option =>
        {
            MenuContainer.Content = null;

            if (option == Option.Restart)
            {
                RestartGame();
            }
        }; 
    }
}