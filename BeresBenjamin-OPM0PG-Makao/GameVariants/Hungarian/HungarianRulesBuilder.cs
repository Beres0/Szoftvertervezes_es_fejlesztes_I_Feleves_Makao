using Makao.Collections;
using Makao.Model;

namespace Makao.GameVariants
{
    public class HungarianRulesBuilder : RulesBuilder
    {
        public HungarianRulesBuilder() : base(HungarianDeck.Deck)
        {
            SetBasicParameters();
            SetThrowingExchanges();
            SetThrowingKeywords();

            SetVIIRank();
            SetUpperRank();
            SetLowerRank();
            SetAceRank();
        }

        private void SetAceRank()
        {
            SetRankInAllSuit
                (HungarianDeck.Rank.Ace, new ActiveCardProperties(punishment: new MissATurn(1), counter: (dp) => SimpleCounter(dp, HungarianDeck.Rank.Ace)));
        }

        private void SetBasicParameters()
        {
            NumberOfDecks.Add(5, 2);

            MaxPlayer = 6;
            StarterHandSize = 5;

            Settings = RulesSettings.CanThrowMoreCards |
                       RulesSettings.AllowPunishmentStacking |
                       RulesSettings.GameEndsWithTheFirstWinner;

            ThrowModifierDuration = ThrowModifierDuration.NextValidThrow;
        }

        private void SetLowerRank()
        {
            SetRankInAllSuit
                (HungarianDeck.Rank.Lower, new ActiveCardProperties(askModifier: Ask.Suit, counter: (dp) => SimpleCounter(dp, HungarianDeck.Rank.Lower)));
        }

        private void SetThrowingExchanges()
        {
            ThrowingExchanges = new AssociativeArray<int, int>();
            ThrowingExchanges.Add(2, 1);
        }

        private void SetThrowingKeywords()
        {
            ThrowingKeywords = new AssociativeArray<int, Keyword>();
            ThrowingKeywords.Add(1, new Keyword("makaó", 5));
            ThrowingKeywords.Add(0, new Keyword("fáraó", 5));
        }

        private void SetUpperRank()
        {
            SetRankInAllSuit
                (HungarianDeck.Rank.Upper, new ActiveCardProperties(askModifier: Ask.Rank,counter: (dp) => SimpleCounter(dp, HungarianDeck.Rank.Upper)));
        }

        private void SetVIIRank()
        {

            SetRankInAllSuit
                (HungarianDeck.Rank.VII, new ActiveCardProperties(punishment: new DrawExtraCards(2), counter: (dp) => SimpleCounter(dp, HungarianDeck.Rank.VII)));
        }
    }
}