using System;

namespace Makao.View
{
    public class SuitDescriptor : VisualDescriptor
    {
        public static readonly SuitDescriptor Error = new SuitDescriptor
            ('#', "ERROR", new ConsoleColorPair(ConsoleColor.White, ConsoleColor.Red));

        public ConsoleColorPair Colors { get; }

        public SuitDescriptor(char mark, string name, ConsoleColorPair colors) : base(mark, name)
        {
            Colors = colors;
        }
    }
}