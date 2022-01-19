using System;

namespace Makao.Model
{
    public struct Card : IEquatable<Card>
    {
        public const int SpecialSuit = -1;

        public bool IsSpecial => Suit == SpecialSuit;

        public int Rank { get; }

        public int Suit { get; }

        public Card(int suit, int rank)
        {
            if (suit < -1) throw new ArgumentException($"Suit must be greater than or equal to {SpecialSuit}!", nameof(suit));
            if (rank < 0) throw new ArgumentException("Rank must be greater than or equal to 0!", nameof(rank));
            Suit = suit;
            Rank = rank;
        }

        public static Card Create<TSuit, TRank>(TSuit suit, TRank rank) where TSuit : Enum where TRank : Enum
        {
            return new Card(Convert.ToInt32(suit), Convert.ToInt32(rank));
        }

        public static Card CreateSpecial<TSpecial>(Deck deck, TSpecial specialRank) where TSpecial : Enum
        {
            return new Card(SpecialSuit, deck.GetSpecialRank(Convert.ToInt32(specialRank)));
        }

        public static bool operator !=(Card left, Card right)
        {
            return !(left == right);
        }

        public static bool operator ==(Card left, Card right)
        {
            return left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            return obj is Card card && Equals(card);
        }

        public bool Equals(Card other)
        {
            return Suit == other.Suit &&
                   Rank == other.Rank;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Suit, Rank);
        }

        public bool Is<TSuit, TRank>(TSuit suit, TRank rank) where TSuit : Enum where TRank : Enum
        {
            return IsSameSuit(suit) && IsSameRank(rank);
        }

        public bool IsSameRank(Card other)
        {
            return Rank == other.Rank;
        }

        public bool IsSameRank<TRank>(TRank rank) where TRank : Enum
        {
            return Convert.ToInt32(rank) == Rank;
        }

        public bool IsSameSuit(Card other)
        {
            return Suit == other.Suit;
        }

        public bool IsSameSuit<TSuit>(TSuit suit) where TSuit : Enum
        {
            return Convert.ToInt32(suit) == Suit;
        }

        public bool IsSameSuitOrRank(Card other)
        {
            return Suit == other.Suit || Rank == other.Rank;
        }

        public override string ToString()
        {
            return $"[{Suit},{Rank}]";
        }
    }
}