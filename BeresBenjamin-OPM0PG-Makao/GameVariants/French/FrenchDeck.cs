using Makao.Model;
using System;

namespace Makao.GameVariants
{
    public class FrenchDeck : Deck
    {
        public enum Rank
        {
            Ace, II, III, IV, V, VI, VII, VIII, IX, X, Jack, Queen, King
        }

        public enum Special
        {
            Joker
        }

        public enum Suit
        {
            Hearts, Diamonds, Clubs, Spades
        }

        public static readonly FrenchDeck Deck = new FrenchDeck();

        private FrenchDeck() : base(Enum.GetValues<Suit>().Length, Enum.GetValues<Rank>().Length, new int[] { 2 })
        {
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