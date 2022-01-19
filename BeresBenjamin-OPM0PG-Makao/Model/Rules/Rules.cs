using Makao.Collections;
using System;

namespace Makao.Model
{
    public class Rules
    {
        private ActiveCardProperties[,] ActiveCardProperties { get; }
        private ReadOnlyDynamicArray<ActiveCardProperties> SpecialRankProperties { get; }
        public ReadOnlyDynamicArray<RankRequestCondition> AskRankConditions { get; }
        public Deck Deck { get; }
        public int MaxPlayer { get; }
        public ReadOnlyAssociativeArray<int, int> NumberOfDecks { get; }
        public RulesSettings Settings { get; }
        public int StarterHandSize { get; }
        public ReadOnlyAssociativeArray<int, int> ThrowingExchanges { get; }
        public ReadOnlyAssociativeArray<int, Keyword> ThrowingKeywords { get; }
        public ThrowModifierDuration ThrowModifierDuration { get; }

        public Rules(RulesBuilder builder)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            Deck = builder.Deck;

            MaxPlayer = builder.MaxPlayer;
            StarterHandSize = builder.StarterHandSize;

            Settings = builder.Settings;
            ThrowModifierDuration = builder.ThrowModifierDuration;
            ActiveCardProperties = builder.ActiveCardProperties;
            SpecialRankProperties = new ReadOnlyDynamicArray<ActiveCardProperties>(builder.SpecialRankProperties);
            AskRankConditions = new ReadOnlyDynamicArray<RankRequestCondition>(builder.AskRankConditions);
            NumberOfDecks = builder.NumberOfDecks.AsReadOnly();
            ThrowingExchanges = builder.ThrowingExchanges != null ? builder.ThrowingExchanges.CopyAsAssociativeArray().AsReadOnly() : null;
            ThrowingKeywords = builder.ThrowingKeywords != null ? builder.ThrowingKeywords.CopyAsAssociativeArray().AsReadOnly() : null;
        }

        public DynamicArray<Card> GetDrawPile(int playersCount)
        {
            int numberOfDecks = 1;
            if (!NumberOfDecks.IsEmpty)
            {
                int i = 0;
                while (i < NumberOfDecks.Count && playersCount >= NumberOfDecks.GetKey(i))
                {
                    i++;
                }

                i--;
                if (i >= 0)
                {
                    numberOfDecks = NumberOfDecks.GetValue(i);
                }
            }

            if (Settings.HasFlag(RulesSettings.AllowSpecialCards))
            {
                return Deck.GetDeckWithSpecials(numberOfDecks);
            }
            else
            {
                return Deck.GetDeck(numberOfDecks);
            }
        }

        public bool HasAskRankCondition(int rank, out RankRequestCondition condition)
        {
            condition = null;
            if (AskRankConditions != null && AskRankConditions[rank] != null)
            {
                condition = AskRankConditions[rank];
                return true;
            }
            else return false;
        }

        public bool HasThrowingExchange(int selectionCount, out int exchange)
        {
            exchange = default;
            if (ThrowingExchanges != null)
            {
                return ThrowingExchanges.TryGetValue(selectionCount, out exchange);
            }
            else return false;
        }

        public bool IsItActiveCard(Card card, out ActiveCardProperties properties)
        {
            if (card.IsSpecial && SpecialRankProperties[Deck.GetSpecialIndex(card.Rank)] != null)
            {
                properties = SpecialRankProperties[Deck.GetSpecialIndex(card.Rank)];
                return true;
            }
            else if (ActiveCardProperties[card.Suit, card.Rank] != null)
            {
                properties = ActiveCardProperties[card.Suit, card.Rank];
                return true;
            }
            else
            {
                properties = null;
                return false;
            }
        }

        public bool NeedKeywords(int countBeforeThrow, int countAfterThrow, out ReadOnlyDynamicArray<Keyword> keywords)
        {
            DynamicArray<Keyword> collected = new DynamicArray<Keyword>();

            for (int i = 0; i < ThrowingKeywords.Count; i++)
            {
                int key = ThrowingKeywords.GetKey(i);
                if (countAfterThrow <= key && countBeforeThrow > key)
                {
                    collected.Add(ThrowingKeywords.GetValue(i));
                }
            }
            keywords = collected.AsReadOnly();
            return collected.Count > 0;
        }
    }
}