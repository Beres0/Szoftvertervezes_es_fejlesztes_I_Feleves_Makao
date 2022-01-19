using Makao.Collections;
using System;

namespace Makao.Model
{
    public class RulesBuilder
    {
        private int numberOfPlayers;
        public ActiveCardProperties[,] ActiveCardProperties { get; }
        public RankRequestCondition[] AskRankConditions { get; }
        public Deck Deck { get; }

        public int MaxPlayer
        {
            get => numberOfPlayers;
            set => numberOfPlayers = Math.Clamp(value, 2, int.MaxValue);
        }

        public AssociativeArray<int, int> NumberOfDecks { get; }

        public RulesSettings Settings { get; set; }
        public ActiveCardProperties[] SpecialRankProperties { get; }
        public int StarterHandSize { get; set; }
        public AssociativeArray<int, int> ThrowingExchanges { get; set; }
        public AssociativeArray<int, Keyword> ThrowingKeywords { get; set; }
        public ThrowModifierDuration ThrowModifierDuration { get; set; }

        public RulesBuilder(Deck deck)
        {
            Deck = deck ?? throw new ArgumentNullException(nameof(deck));
            ActiveCardProperties = new ActiveCardProperties[deck.NumberOfSuits, deck.NumberOfRanks];
            SpecialRankProperties = new ActiveCardProperties[deck.NumberOfSpecialRanks];
            AskRankConditions = new RankRequestCondition[deck.NumberOfRanks + deck.NumberOfSpecialRanks];
            NumberOfDecks = new AssociativeArray<int, int>();
        }

        public void SetRankInAllSuit(int rank, ActiveCardProperties props)
        {
            for (int i = 0; i < Deck.NumberOfSuits; i++)
            {
                ActiveCardProperties[i, rank] = props;
            }
        }

        public void SetRankInAllSuit<TRank>(TRank rank, ActiveCardProperties props) where TRank : Enum
        {
            int intRank = Convert.ToInt32(rank);
            SetRankInAllSuit(intRank, props);
        }

        public void SetRankInSpecificSuit(int rank, ActiveCardProperties props, params int[] suits)
        {
            for (int i = 0; i < suits.Length; i++)
            {
                ActiveCardProperties[suits[i], rank] = props;
            }
        }

        public void SetRankInSpecificSuit<TSuit, TRank>(TRank rank, ActiveCardProperties props, params TSuit[] suits)
            where TRank : Enum where TSuit : Enum
        {
            int intRank = Convert.ToInt32(rank);
            for (int i = 0; i < suits.Length; i++)
            {
                ActiveCardProperties[Convert.ToInt32(suits[i]), intRank] = props;
            }
        }

        public void SetSpecialRank<TSpecial>(TSpecial specialRank, ActiveCardProperties props) where TSpecial : Enum
        {
            SpecialRankProperties[Convert.ToInt32(specialRank)] = props;
        }


        public bool SimpleCounter<TRank>(ReadOnlyDynamicArray<Card> discardPile,TRank rank) where TRank:Enum
        {
            return discardPile.TryGetLastItem(out Card item) && item.IsSameRank(rank);
        }

        public Rules ToRules()
        {
            return new Rules(this);
        }
    }
}