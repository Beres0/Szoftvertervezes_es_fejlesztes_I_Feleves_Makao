using Makao.Collections;
using Makao.Model;
using System;

namespace Makao.View
{
    public class DeckGraphics
    {
        private readonly CardGraphics[,] normalCardGraphics;
        public Deck Deck { get; }
        public CardGraphics Facedown { get; }

        public ReadOnlyDynamicArray<RankDescriptor> Ranks { get; }
        public ReadOnlyDynamicArray<CardGraphics> SpecialCards { get; }
        public ReadOnlyDynamicArray<SuitDescriptor> Suits { get; }
        public CardGraphics this[Card card] => this[card.Suit, card.Rank];

        public CardGraphics this[int suit, int rank]
        {
            get
            {
                if (Card.SpecialSuit == suit)
                {
                    return SpecialCards[Deck.GetSpecialIndex(rank)];
                }
                else
                {
                    return normalCardGraphics[suit, rank];
                }
            }
        }

        public DeckGraphics(DeckGraphicsBuilder builder)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            Deck = builder.Deck;
            Ranks = new ReadOnlyDynamicArray<RankDescriptor>(builder.Ranks);
            Suits = new ReadOnlyDynamicArray<SuitDescriptor>(builder.Suits);
            SpecialCards = new ReadOnlyDynamicArray<CardGraphics>(builder.SpecialCards);
            Facedown = builder.Facedown;

            normalCardGraphics = CreateNormalCardGraphicsFromDescriptors();
        }

        private CardGraphics[,] CreateNormalCardGraphicsFromDescriptors()
        {
            CardGraphics[,] graphics = new CardGraphics[Suits.Count, Ranks.Count];
            for (int suit = 0; suit < Suits.Count; suit++)
            {
                for (int rank = 0; rank < Ranks.Count; rank++)
                {
                    if (Suits[suit] == null || Ranks[rank] == null)
                    {
                        graphics[suit, rank] = CardGraphics.Error;
                    }
                    graphics[suit, rank] = new CardGraphics(Suits[suit], Ranks[rank]);
                }
            }
            return graphics;
        }
    }
}