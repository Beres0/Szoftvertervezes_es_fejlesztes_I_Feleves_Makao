using Makao.View;
using System;

namespace Makao.GameVariants
{
    internal class UnoGraphicsBuilder : DeckGraphicsBuilder
    {
        public UnoGraphicsBuilder() : base(UnoDeck.Deck)
        {
            SetSuitDescriptors();
            SetRankDescriptors();
            SetSpecialCardsGraphics();
        }

        private void SetRankDescriptors()
        {
            Ranks[(int)UnoDeck.Rank.Zero] = new RankDescriptor('0', "Nulla");
            Ranks[(int)UnoDeck.Rank.One] = new RankDescriptor('1', "Egyes");
            Ranks[(int)UnoDeck.Rank.Two] = new RankDescriptor('2', "Kettes");
            Ranks[(int)UnoDeck.Rank.Three] = new RankDescriptor('3', "Hármas");
            Ranks[(int)UnoDeck.Rank.Four] = new RankDescriptor('4', "Négyes");
            Ranks[(int)UnoDeck.Rank.Five] = new RankDescriptor('5', "Ötös");
            Ranks[(int)UnoDeck.Rank.Six] = new RankDescriptor('6', "Hatos");
            Ranks[(int)UnoDeck.Rank.Seven] = new RankDescriptor('7', "Hetes");
            Ranks[(int)UnoDeck.Rank.Eight] = new RankDescriptor('8', "Nyolcas");
            Ranks[(int)UnoDeck.Rank.Nine] = new RankDescriptor('9', "Kilences");
            Ranks[(int)UnoDeck.Rank.Skip] = new RankDescriptor('Ø', "Skip");
            Ranks[(int)UnoDeck.Rank.Reverse] = new RankDescriptor("«»", "Reverse");
            Ranks[(int)UnoDeck.Rank.PlusTwo] = new RankDescriptor("+2", "Plus 2");
        }

        private void SetSpecialCardsGraphics()
        {
            ConsoleColorPair specialColors = new ConsoleColorPair(ConsoleColor.Cyan, ConsoleColor.DarkGray);
            SpecialCards[(int)UnoDeck.Special.Wild] = new CardGraphics
                (new SuitDescriptor(' ', "Wild", specialColors), new RankDescriptor("▀▄", "Wild"));
            SpecialCards[(int)UnoDeck.Special.WildPlusFour] = new CardGraphics
               (new SuitDescriptor(' ', "Wild", specialColors), new RankDescriptor("+4", "Draw 4"));
        }

        private void SetSuitDescriptors()
        {
            Suits[(int)UnoDeck.Suit.Red] = new SuitDescriptor
                (' ', "Piros", new ConsoleColorPair(ConsoleColor.White, ConsoleColor.Red));
            Suits[(int)UnoDeck.Suit.Yellow] = new SuitDescriptor
                (' ', "Sárga", new ConsoleColorPair(ConsoleColor.Black, ConsoleColor.Yellow));
            Suits[(int)UnoDeck.Suit.Green] = new SuitDescriptor
                (' ', "Zöld", new ConsoleColorPair(ConsoleColor.Black, ConsoleColor.Green));
            Suits[(int)UnoDeck.Suit.Blue] = new SuitDescriptor
                (' ', "Kék", new ConsoleColorPair(ConsoleColor.White, ConsoleColor.Blue));
        }
    }
}