using System;

namespace Makao.View
{
    public class ApplicationSettings
    {
        public ConsoleColorPair CurrentColor { get; set; } = new ConsoleColorPair(ConsoleColor.Cyan, ConsoleColor.Black);
        public ConsoleColorPair CurrentPlayerColor { get; set; } = new ConsoleColorPair(ConsoleColor.Green, ConsoleColor.Black);
        public ConsoleKey DeleteKey { get; set; } = ConsoleKey.Delete;
        public ConsoleKey DownKey { get; set; } = ConsoleKey.DownArrow;
        public ConsoleKey EnterKey { get; set; } = ConsoleKey.Enter;
        public ConsoleKey EscKey { get; set; } = ConsoleKey.Escape;
        public ConsoleColorPair HighlightedColor { get; set; } = new ConsoleColorPair(ConsoleColor.White, ConsoleColor.Black);
        public ConsoleColorPair LeaverColor { get; set; } = new ConsoleColorPair(ConsoleColor.DarkGray, ConsoleColor.Black);
        public ConsoleKey LeftKey { get; set; } = ConsoleKey.LeftArrow;
        public string ReplaysDirectory { get; set; } = "replays";
        public string ReplaysExtension { get; set; } = "mrp";
        public ConsoleKey RightKey { get; set; } = ConsoleKey.RightArrow;
        public string SavesDirectory { get; set; } = "saves";
        public string SavesExtension { get; set; } = "msv";
        public ConsoleColorPair SelectedCardColor { get; set; } = new ConsoleColorPair(ConsoleColor.Black, ConsoleColor.Cyan);
        public ConsoleKey SelectKey { get; set; } = ConsoleKey.Spacebar;
        public char SepLineChar { get; set; } = '=';
        public ConsoleKey UndoKey { get; set; } = ConsoleKey.Backspace;
        public ConsoleKey UpKey { get; set; } = ConsoleKey.UpArrow;
        public ConsoleColorPair WinnerColor { get; set; } = new ConsoleColorPair(ConsoleColor.Yellow, ConsoleColor.Black);
    }
}