using Makao.Collections;
using Makao.Model;

namespace Makao.GameVariants
{
    public class UnoRulesBuilder : RulesBuilder
    {
        public UnoRulesBuilder() : base(UnoDeck.Deck)
        {
            MaxPlayer = 10;
            StarterHandSize = 7;
            Settings = RulesSettings.AllowPunishmentStacking |
                       RulesSettings.CanThrowMoreCards |
                       RulesSettings.AllowSpecialCards;
            ThrowModifierDuration = ThrowModifierDuration.NextValidThrow;

            ThrowingKeywords = new AssociativeArray<int, Keyword>();
            ThrowingKeywords.Add(1, new Keyword("UNO", 4));

            SetSkipRank();
            SetReverseRank();
            SetPlusTwoRank();
            SetWildRank();
            SetWildPlusFourRank();
        }

        private void SetPlusTwoRank()
        {
            Counter counter = (dp) => dp.TryGetLastItem(out Card item) && item.IsSameRank(UnoDeck.Rank.PlusTwo);
            SetRankInAllSuit(UnoDeck.Rank.PlusTwo, new ActiveCardProperties(punishment: new DrawExtraCards(2), counter: counter));
        }

        private void SetReverseRank()
        {
            SetRankInAllSuit(UnoDeck.Rank.Reverse,
                new ActiveCardProperties(gameModifier: GameModifier.Reverse));
        }

        private void SetSkipRank()
        {
            Counter counter = (dp) => dp.TryGetLastItem(out Card item) && item.IsSameRank(UnoDeck.Rank.Skip);
            SetRankInAllSuit(UnoDeck.Rank.Skip,
                new ActiveCardProperties(punishment: new MissATurn(1), counter: counter));
        }

        private void SetWildPlusFourRank()
        {
            Counter counter = dp => dp.TryGetLastItem(out Card item)
            && (item.IsSameRank(UnoDeck.Rank.PlusTwo) || item.Rank == Deck.GetSpecialRank((int)UnoDeck.Special.WildPlusFour));

            SetSpecialRank(UnoDeck.Special.WildPlusFour,
                new ActiveCardProperties(punishment: new DrawExtraCards(4),
                askModifier: Ask.Suit, counter: counter));
        }

        private void SetWildRank()
        {
            Counter counter = dp => dp.TryGetLastItem(out Card item)
            && item.Rank == Deck.GetSpecialRank((int)UnoDeck.Special.Wild);

            SetSpecialRank(UnoDeck.Special.Wild,
                new ActiveCardProperties(askModifier: Ask.Suit,counter:counter));
        }
    }
}