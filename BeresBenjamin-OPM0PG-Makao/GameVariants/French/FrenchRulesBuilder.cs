using Makao.Collections;
using Makao.Model;

namespace Makao.GameVariants

{
    public class FrenchRulesBuilder : RulesBuilder
    {
        public FrenchRulesBuilder() : base(FrenchDeck.Deck)
        {
            SetBasicParameters();
            SetThrowingExchanges();
            SetKeywords();

            SetAceRank();
            SetIIRank();
            SetIIIRank();
            SetIVRank();
            SetVIRank();
            SetKingRank();
        }

        private void SetAceRank()
        {
            SetRankInAllSuit(FrenchDeck.Rank.Ace, new ActiveCardProperties(askModifier: Ask.Suit, counter: (dp) => SimpleCounter(dp, FrenchDeck.Rank.Ace)));
        }

        private void SetBasicParameters()
        {
            MaxPlayer = 6;
            StarterHandSize = 5;

            Settings = RulesSettings.CanThrowMoreCards |
                       RulesSettings.AllowPunishmentStacking |
                       RulesSettings.GameEndsWithTheFirstWinner;
            ThrowModifierDuration = ThrowModifierDuration.NextValidThrow;
        }

        private void SetIIIRank()
        {
           
            SetRankInAllSuit
                (FrenchDeck.Rank.III, new ActiveCardProperties(punishment: new MissATurn(1), counter: (dp)=>SimpleCounter(dp,FrenchDeck.Rank.III)));
        }

        private void SetIIRank()
        {

            SetRankInAllSuit
                (FrenchDeck.Rank.II, new ActiveCardProperties(punishment: new DrawExtraCards(2), counter: (dp) => SimpleCounter(dp, FrenchDeck.Rank.II)));
        }

        private void SetIVRank()
        {
            SetRankInAllSuit(FrenchDeck.Rank.IV,
            new ActiveCardProperties(gameModifier: GameModifier.Reverse));
        }

        private void SetKeywords()
        {
            ThrowingKeywords = new AssociativeArray<int, Keyword>();
            ThrowingKeywords.Add(1, new Keyword("makaó", 5));
            ThrowingKeywords.Add(0, new Keyword("fáraó", 5));
        }

        private void SetKingRank()
        {
            SetRankInAllSuit
                (FrenchDeck.Rank.King, new ActiveCardProperties(askModifier: Ask.Rank, counter: (dp) => SimpleCounter(dp, FrenchDeck.Rank.King)));
        }

        private void SetThrowingExchanges()
        {
            ThrowingExchanges = new AssociativeArray<int, int>();
            ThrowingExchanges.Add(2, 1);
        }

        private void SetVIRank()
        {
            SetRankInAllSuit(FrenchDeck.Rank.VI,
                new ActiveCardProperties(gameModifier: GameModifier.SwapHands));
        }
    }
}