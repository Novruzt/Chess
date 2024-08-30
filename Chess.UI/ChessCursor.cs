using System.IO;
using System.Windows;
using System.Windows.Input;

namespace Chess.UI
{
    public static class ChessCursor
    {
        public static readonly Cursor White = LoadCursor("Assets/CursorW.cur");
        public static readonly Cursor Black = LoadCursor("Assets/CursorB.cur");
        private static Cursor LoadCursor(string filepath)
        {
            Stream stream = Application.GetResourceStream(new Uri(filepath, UriKind.Relative)).Stream;

            return new Cursor(stream, true);
        }
    }
}
