using Makao.Collections;
using System;

namespace Makao.Model
{
    public class Deck
    {
        private int deckSize = -1;

        public ReadOnlyDynamicArray<int> CardsPerRank { get; protected set; }

        public ReadOnlyDynamicArray<int> CardsPerSpecialRank { get; protected set; }

        public int DeckSize
        {
            get
            {
                if (deckSize == -1)
                {
                    deckSize = NumberOfSuits * SumOfCards(CardsPerRank) + SumOfCards(CardsPerSpecialRank);
                }
                return deckSize;
            }
        }

        public int NumberOfRanks => CardsPerRank.Count;

        public int NumberOfSpecialRanks => CardsPerSpecialRank.Count;

        public int NumberOfSuits { get; protected set; }

        private Deck(int numberOfSuits, int[] cardsCountPerSpecialRanks = null)
        {
            if (numberOfSuits < 1)
            {
                throw new ArgumentException("Number of suits must be greater than zero!", nameof(numberOfSuits));
            }
            NumberOfSuits = numberOfSuits;
            CardsPerSpecialRank = new ReadOnlyDynamicArray<int>(cardsCountPerSpecialRanks);
        }

        protected Deck()
        {
        }

        public Deck(int numberOfSuits, int[] cardsCountPerRanks, int[] cardsCountPerSpecialRanks = null)
            : this(numberOfSuits, cardsCountPerSpecialRanks)
        {
            if (cardsCountPerRanks is null || cardsCountPerRanks.Length == 0)
            {
                throw new ArgumentNullException(nameof(cardsCountPerRanks));
            }

            CardsPerRank = new ReadOnlyDynamicArray<int>(cardsCountPerRanks);
        }

        public Deck(int numberOfSuits, int numberOfRanks, int[] cardsCountPerSpecialRanks = null)
            : this(numberOfSuits, GetDefaultCardsPerRanks(numberOfRanks, 1), cardsCountPerSpecialRanks)
        {
        }

        protected static int[] GetDefaultCardsPerRanks(int numberOfRanks, int defaultCount)
        {
            int[] cardsCountPerRanks = new int[numberOfRanks];
            for (int i = 0; i < numberOfRanks; i++)
            {
                cardsCountPerRanks[i] = defaultCount;
            }
            return cardsCountPerRanks;
        }

        protected int SumOfCards(ReadOnlyDynamicArray<int> cardsCountPerRanks)
        {
            int sum = 0;
            for (int i = 0; i < cardsCountPerRanks.Count; i++)
            {
                sum += cardsCountPerRanks[i];
            }
            return sum;
        }

        public DynamicArray<Card> GetDeck(int deckCount)
        {
            DynamicArray<Card> cards = new DynamicArray<Card>();
            for (int deck = 0; deck < deckCount; deck++)
            {
                for (int suit = 0; suit < NumberOfSuits; suit++)
                {
                    for (int rank = 0; rank < NumberOfRanks; rank++)
                    {
                        for (int card = 0; card < CardsPerRank[rank]; card++)
                        {
                            cards.Add(new Card(suit, rank));
                        }
                    }
                }
            }
            return cards;
        }

        public DynamicArray<Card> GetDeckUniques()
        {
            DynamicArray<Card> cards = new DynamicArray<Card>();
            for (int suit = 0; suit < NumberOfSuits; suit++)
            {
                for (int rank = 0; rank < NumberOfRanks; rank++)
                {
                    cards.Add(new Card(suit, rank));
                }
            }
            return cards;
        }

        public DynamicArray<Card> GetDeckUniquesWithSpecial()
        {
            DynamicArray<Card> cards = GetDeckUniques();
            for (int index = 0; index < NumberOfSpecialRanks; index++)
            {
                cards.Add(new Card(Card.SpecialSuit, GetSpecialRank(index)));
            }
            return cards;
        }

        public DynamicArray<Card> GetDeckWithSpecials(int deckCount)
        {
            DynamicArray<Card> cards = GetDeck(deckCount);

            for (int index = 0; index < NumberOfSpecialRanks; index++)
            {
                for (int i = 0; i < CardsPerSpecialRank[index]; i++)
                {
                    cards.Add(new Card(Card.SpecialSuit, GetSpecialRank(index)));
                }
            }
            return cards;
        }

        public int GetSpecialIndex(int rank)
        {
            return rank - NumberOfRanks;
        }

        public int GetSpecialRank(int index)
        {
            return NumberOfRanks + index;
        }
    }
}