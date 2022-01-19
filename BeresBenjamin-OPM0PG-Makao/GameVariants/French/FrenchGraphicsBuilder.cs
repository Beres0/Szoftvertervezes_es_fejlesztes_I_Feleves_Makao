using Makao.View;
using System;

namespace Makao.GameVariants
{
    public class FrenchGraphicsBuilder : DeckGraphicsBuilder
    {
        private const ConsoleColor Background = ConsoleColor.White;

        public FrenchGraphicsBuilder() : base(FrenchDeck.Deck)
        {
            SetSuitDescriptors();
            SetRankDescriptors();
            SetSpecialCardsGraphics();
            Facedown = DefaultFacedown;
        }

        private void SetRankDescriptors()
        {
            Ranks[(int)FrenchDeck.Rank.Ace] = new RankDescriptor('A', "Ász");
            Ranks[(int)FrenchDeck.Rank.II] = new RankDescriptor('2', "Kettes");
            Ranks[(int)FrenchDeck.Rank.III] = new RankDescriptor('3', "Hármas");
            Ranks[(int)FrenchDeck.Rank.IV] = new RankDescriptor('4', "Négyes");
            Ranks[(int)FrenchDeck.Rank.V] = new RankDescriptor('5', "Ötös");
            Ranks[(int)FrenchDeck.Rank.VI] = new RankDescriptor('6', "Hatos");
            Ranks[(int)FrenchDeck.Rank.VII] = new RankDescriptor('7', "Hetes");
            Ranks[(int)FrenchDeck.Rank.VIII] = new RankDescriptor('8', "Nyolcas");
            Ranks[(int)FrenchDeck.Rank.IX] = new RankDescriptor('9', "Kilences");
            Ranks[(int)FrenchDeck.Rank.X] = new RankDescriptor('X', "Tizes");
            Ranks[(int)FrenchDeck.Rank.Jack] = new RankDescriptor('J', "Jumbó");
            Ranks[(int)FrenchDeck.Rank.Queen] = new RankDescriptor('Q', "Királynő");
            Ranks[(int)FrenchDeck.Rank.King] = new RankDescriptor('K', "Király");
        }

        private void SetSpecialCardsGraphics()
        {
            SpecialCards[(int)FrenchDeck.Special.Joker] = new CardGraphics
                (new SuitDescriptor('J', "JOKER", new ConsoleColorPair(ConsoleColor.DarkYellow, Background)),
                 new RankDescriptor('*', "JOKER"));
        }

        private void SetSuitDescriptors()
        {
            Suits[(int)FrenchDeck.Suit.Hearts] = new SuitDescriptor
                ('♥', "Szív", new ConsoleColorPair(ConsoleColor.Red, Background));

            Suits[(int)FrenchDeck.Suit.Diamonds] = new SuitDescriptor
                ('♦', "Káró", new ConsoleColorPair(ConsoleColor.Red, Background));

            Suits[(int)FrenchDeck.Suit.Clubs] = new SuitDescriptor
                ('♣', "Treff", new ConsoleColorPair(ConsoleColor.Black, Background));

            Suits[(int)FrenchDeck.Suit.Spades] = new SuitDescriptor
                ('♠', "Pikk", new ConsoleColorPair(ConsoleColor.Black, Background));
        }
    }
}