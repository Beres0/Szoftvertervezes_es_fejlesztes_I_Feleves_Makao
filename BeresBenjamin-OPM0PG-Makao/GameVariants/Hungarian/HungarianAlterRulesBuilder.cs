using Makao.Collections;
using Makao.Model;

namespace Makao.GameVariants
{
    public class HungarianAlterRulesBuilder : HungarianRulesBuilder
    {
        public HungarianAlterRulesBuilder() : base()
        {
            SetLowerRank();
            SetIXRank();
            SetVIIRank();
        }

        private bool AcornVIICounter(ReadOnlyDynamicArray<Card> discardPile)
        {
            Card acornVII = Card.Create(HungarianDeck.Suit.Acorns, HungarianDeck.Rank.VII);
            Card acornLower = Card.Create(HungarianDeck.Suit.Acorns, HungarianDeck.Rank.Lower);

            if (!discardPile.IsEmpty)
            {
                Card last = discardPile[discardPile.Count - 1];
                
                if(last.IsSameRank(acornVII))
                {
                    return true;
                }
                else if (discardPile.Count == 1)
                {
                    return last == acornLower;
                }
                else if (discardPile.Count > 1)
                {
                    Card secondLast = discardPile[discardPile.Count - 2];

                    return last == acornLower && secondLast != acornVII;
                }
            }
            return false;
        }

        private void SetIXRank()
        {
            SetRankInAllSuit(HungarianDeck.Rank.IX, new ActiveCardProperties(askModifier: Ask.Suit,counter: (dp) => SimpleCounter(dp, HungarianDeck.Rank.IX)));
        }

        private void SetLowerRank()
        {
            SetRankInAllSuit(HungarianDeck.Rank.Lower, null);

            Counter acornCounter = (dp) => dp.TryGetLastItem(out Card item)
                                                     && item.Is(HungarianDeck.Suit.Acorns, HungarianDeck.Rank.VII);
            SetRankInSpecificSuit
                (HungarianDeck.Rank.Lower,
                 new ActiveCardProperties(punishment: new DrawExtraCards(3), counter: acornCounter),
                 HungarianDeck.Suit.Acorns);
        }

        private void SetVIIRank()
        {
            SetRankInSpecificSuit
                (HungarianDeck.Rank.VII,new ActiveCardProperties(punishment: new DrawExtraCards(2), counter: AcornVIICounter),HungarianDeck.Suit.Acorns);
        }
    }
}