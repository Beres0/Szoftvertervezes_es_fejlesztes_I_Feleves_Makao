using Makao.View;
using System;

namespace Makao.GameVariants
{
    public class HungarianGraphicsBuilder : DeckGraphicsBuilder
    {
        private const ConsoleColor Background = ConsoleColor.White;

        public HungarianGraphicsBuilder() : base(HungarianDeck.Deck)
        {
            SetSuitDescriptors();
            SetRankDescriptors();
        }

        private void SetRankDescriptors()
        {
            Ranks[(int)HungarianDeck.Rank.VII] = new RankDescriptor('7', "Hetes");
            Ranks[(int)HungarianDeck.Rank.VIII] = new RankDescriptor('8', "Nyolcas");
            Ranks[(int)HungarianDeck.Rank.IX] = new RankDescriptor('9', "Kilences");
            Ranks[(int)HungarianDeck.Rank.X] = new RankDescriptor('X', "Tizes");
            Ranks[(int)HungarianDeck.Rank.Lower] = new RankDescriptor('↓', "Alsó");
            Ranks[(int)HungarianDeck.Rank.Upper] = new RankDescriptor('↑', "Felső");
            Ranks[(int)HungarianDeck.Rank.King] = new RankDescriptor('K', "Király");
            Ranks[(int)HungarianDeck.Rank.Ace] = new RankDescriptor('A', "Ász");
        }

        private void SetSuitDescriptors()
        {
            Suits[(int)HungarianDeck.Suit.Hearts] = new SuitDescriptor
                ('♥', "Szív", new ConsoleColorPair(ConsoleColor.Red, Background));
            Suits[(int)HungarianDeck.Suit.Bells] = new SuitDescriptor
                ('●', "Tök", new ConsoleColorPair(ConsoleColor.DarkYellow, Background));
            Suits[(int)HungarianDeck.Suit.Acorns] = new SuitDescriptor
                ('¶', "Makk", new ConsoleColorPair(ConsoleColor.DarkYellow, Background));
            Suits[(int)HungarianDeck.Suit.Leaves] = new SuitDescriptor
                ('♠', "Zöld", new ConsoleColorPair(ConsoleColor.DarkGreen, Background));
        }
    }
}