using Makao.Collections;
using Makao.Model;
using System;

namespace Makao.GameVariants
{
    public class UnoDeck : Deck
    {
        public enum Rank
        {
            Zero, One, Two, Three, Four, Five, Six, Seven, Eight, Nine, Skip, Reverse, PlusTwo
        }

        public enum Special
        {
            Wild, WildPlusFour
        }

        public enum Suit
        {
            Red, Yellow, Green, Blue
        }

        public static readonly UnoDeck Deck = new UnoDeck();

        private UnoDeck() : base()
        {
            NumberOfSuits = Enum.GetValues<Suit>().Length;
            int[] cardsPerRank = GetDefaultCardsPerRanks(Enum.GetValues<Rank>().Length, 2);
            cardsPerRank[0] = 1;
            CardsPerRank = new ReadOnlyDynamicArray<int>(cardsPerRank);
            CardsPerSpecialRank = new ReadOnlyDynamicArray<int>(GetDefaultCardsPerRanks(Enum.GetValues<Special>().Length, 4));
        }

        public static Card GetCard(Suit suit, Rank rank)
        {
            return new Card((int)suit, (int)rank);
        }

        public static Card GetSpecialCard(Special special)
        {
            return new Card(Card.SpecialSuit, Deck.GetSpecialRank((int)special));
        }
    }
}