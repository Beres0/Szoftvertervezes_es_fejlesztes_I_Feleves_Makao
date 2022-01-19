using Makao.Model;
using System;

namespace Makao.GameVariants
{
    public class HungarianDeck : Deck
    {
        public enum Rank
        {
            VII, VIII, IX, X, Lower, Upper, King, Ace
        }

        public enum Suit
        {
            Hearts, Bells, Acorns, Leaves
        }

        public static readonly HungarianDeck Deck = new HungarianDeck();

        private HungarianDeck() : base(Enum.GetValues<Suit>().Length, Enum.GetValues<Rank>().Length)
        {
        }

        public Card GetCard(Suit suit, Rank rank)
        {
            return new Card((int)suit, (int)rank);
        }
    }
}