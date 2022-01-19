using Makao.Collections;
using Makao.Model;
using System;

namespace Makao.View
{
    public static class ConsoleHelper
    {
        public static void WriteCard(Card card, DeckGraphics graphics)
        {
            CardGraphics graph = graphics[card];
            WriteCard(graph);
        }

        public static void WriteCard(CardGraphics graphics)
        {
            WriteColorizedText(graphics.Text, graphics.Suit.Colors);
        }

        public static void WriteCards(IDynamicArray<Card> cards, DeckGraphics graphics, char separator = ' ')
        {
            if (!cards.IsNullOrEmpty())
            {
                WriteCard(cards[0], graphics);

                for (int i = 1; i < cards.Count; i++)
                {
                    Console.Write(separator);
                    WriteCard(cards[i], graphics);
                }
            }
        }

        public static void WriteCollection<T>
                                    (IDynamicArray<T> collection, Action separator, Action<T> writeMethod)
        {
            if (!collection.IsNullOrEmpty())
            {
                writeMethod(collection[0]);
                for (int i = 1; i < collection.Count; i++)
                {
                    separator();
                    writeMethod(collection[i]);
                }
            }
        }

        public static void WriteCollection<T>(IDynamicArray<T> collection, string separator = ", ")
        {
            if (!collection.IsEmpty)
            {
                Console.Write(collection[0]);
                for (int i = 1; i < collection.Count; i++)
                {
                    Console.Write(separator);
                    Console.WriteLine(collection[i]);
                }
            }
        }

        public static void WriteColorizedText(string text, ConsoleColorPair colors)
        {
            WriteColorizedText(text, colors.Foreground, colors.Background);
        }

        public static void WriteColorizedText(string text, ConsoleColor foreground, ConsoleColor background)
        {
            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;
            Console.Write(text);
            Console.ResetColor();
        }

        public static void WriteColorizedTextCenter(string text, ConsoleColorPair colors)
        {
            Console.Write(new string(' ', (Console.BufferWidth - 6 - text.Length) / 2));
            WriteColorizedText(text, colors);
        }

        public static void WriteLineCardsWithText(string text, ReadOnlyDynamicArray<Card> cards, DeckGraphics graphics, char separator = ' ')
        {
            Console.Write(text);
            for (int i = 0; i < cards.Count; i++)
            {
                WriteCard(cards[i], graphics);
                Console.Write(separator);
            }
            Console.WriteLine();
        }

        public static void WriteLineCardWithText(string text, Card card, DeckGraphics graphics)
        {
            Console.Write(text);
            WriteCard(card, graphics);
            Console.ResetColor();
            Console.WriteLine();
        }

        public static void WriteLineCenter(string text)
        {
            Console.WriteLine(new string(' ', (Console.BufferWidth - 6 - text.Length) / 2) + text);
        }

        public static void WriteLineRight(string text)
        {
            Console.Write(new string(' ', Console.BufferWidth - text.Length) + text);
        }

        public static void WriteSepartorLine(char character)
        {
            Console.WriteLine(new string(character, Console.BufferWidth - 1));
        }
    }
}