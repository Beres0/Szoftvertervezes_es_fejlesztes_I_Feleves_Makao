using System;

namespace Makao.View
{
    public struct ConsoleColorPair
    {
        public static readonly ConsoleColorPair Default = new ConsoleColorPair(ConsoleColor.Gray, ConsoleColor.Black);
        public ConsoleColor Background { get; }

        public ConsoleColor Foreground { get; }

        public ConsoleColorPair(ConsoleColor foreground, ConsoleColor background)
        {
            Foreground = foreground;
            Background = background;
        }
    }
}