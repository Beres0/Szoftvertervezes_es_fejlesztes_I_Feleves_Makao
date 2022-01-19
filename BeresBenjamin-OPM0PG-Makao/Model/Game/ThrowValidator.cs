using Makao.Collections;
using System;

namespace Makao.Model
{
    public class ThrowValidator
    {
        private Game Game { get; }

        public ThrowValidator(Game game)
        {
            Game = game;
        }

        private Result AreOtherCardsValid(ReadOnlyDynamicArray<Card> hand, ReadOnlyDynamicArray<int> selection)
        {
            Card firstCard = hand[selection[0]];
            for (int i = 1; i < selection.Count; i++)
            {
                if (!IsItValidIndex(selection[i], hand))
                {
                    return Result.CardDoesNotExists;
                }
                if (!firstCard.IsSameRank(hand[selection[i]]))
                {
                    return Result.NotAllCardsAreTheSameRank;
                }
                for (int j = 0; j < i; j++)
                {
                    if (selection[i] == selection[j])
                    {
                        return Result.DuplicationInSelection;
                    }
                }
            }
            return Result.Success;
        }

        private Result CanThrowMoreCardsAtOnce(ReadOnlyDynamicArray<Card> hand, ReadOnlyDynamicArray<int> selection)
        {
            if (!Game.Rules.Settings.HasFlag(RulesSettings.CanThrowMoreCards) && selection.Count > 1)
            {
                return Result.CantThrowMoreCardsAtOnce;
            }
            else return CantThrowAllCardAsOnce(hand, selection);
        }

        private Result CantThrowAllCardAsOnce(ReadOnlyDynamicArray<Card> hand, ReadOnlyDynamicArray<int> selection)
        {
            if (hand.Count > 1 && selection.Count >= hand.Count)
            {
                return Result.CantThrowAllCardsAtOnce;
            }
            else return IsFirstCardValid(hand, selection);
        }

       /* private Result HasPunishment(Card card)
        {
            if (Game.Punishment != null)
            {
                return IsItCounter(card);
            }
            else return Result.Success;
        }
       */

    

        private Result IsFirstCardValid(ReadOnlyDynamicArray<Card> hand, ReadOnlyDynamicArray<int> selection)
        {
            if (IsItValidIndex(selection[0], hand))
            {
                Result result = IsValidCard(hand[selection[0]]);
                if (result == Result.Success)
                {
                    return AreOtherCardsValid(hand, selection);
                }
                else return result;
            }
            else return Result.CardDoesNotExists;
        }

        

        public Result IsValidCard(Card card)
        {
            if(Game.Punishment!=null)
            {
                return IsItPunishmentCounter(card);
            }
            else if(Game.RankModifier!=null||Game.SuitModifier!=null)
            {
                return IsThrowModifierCounter(card);
            }
            else
            {
                return IsSameSuitOrRankOrSpecial(card);
            }
        }

        private Result IsItPunishmentCounter(Card card)
        {
            if (Game.Rules.IsItActiveCard(card, out ActiveCardProperties properties)
           && properties.HasPunishmentCounter(out Counter counter)
              && counter(Game.DiscardPile))
            {
                return Result.Success;
            }
            else return Result.NotCounter;
        }

        
        private Result IsSameSuitOrRankOrSpecial(Card card)
        {
            if (Game.TopOfDiscardPile.HasValue
                && card.IsSameSuitOrRank(Game.TopOfDiscardPile.Value)
                || card.IsSpecial
                || Game.TopOfDiscardPile.Value.IsSpecial)
            {
                return Result.Success;
            }
            else return Result.NotSameRankOrSuitOrSpecial;
        }

        private Result IsThrowModifierCounter(Card card)
        {
            if (Game.Rules.IsItActiveCard(card, out ActiveCardProperties properties)
          && properties.HasPunishmentCounter(out Counter counter)
             && counter(Game.DiscardPile))
            {
                return Result.Success;
            }
            else if (Game.SuitModifier != null && Game.RankModifier != null)
            {
                return IsFitForRankAndSuitModifiers(card);
            }
            else if (Game.SuitModifier != null)
            {
                return IsFitForSuitModifier(card);
            }
            else
            {
                return IsFitForRankModifier(card);
            }
        }
        private Result IsFitForRankAndSuitModifiers(Card card)
        {
            if (Game.RankModifier.ItIsFit(card) && Game.SuitModifier.ItIsFit(card))
            {
                return Result.Success;
            }
            else return Result.RankAndSuitAreNotFitForModifiers;
        }

        private Result IsFitForRankModifier(Card card)
        {
            if (Game.RankModifier.ItIsFit(card))
            {
                return Result.Success;
            }
            else return Result.RankIsNotFitForModifier;
        }

        private Result IsFitForSuitModifier(Card card)
        {
            if (Game.SuitModifier.ItIsFit(card))
            {
                return Result.Success;
            }
            else return Result.SuitIsNotFitForModifier;
        }






        /*
        private Result IsFitForRankAndSuitModifiers(Card card)
        {
            if (Game.SuitModifier.ItIsFit(card) && Game.RankModifier.ItIsFit(card))
            {
                return HasPunishment(card);
            }
            else return Result.RankAndSuitAreNotFitForModifiers;
        }

        private Result IsFitForRankModifier(Card card)
        {
            if (Game.RankModifier.ItIsFit(card))
            {
                return HasPunishment(card);
            }
            else return Result.RankIsNotFitForModifier;
        }

        private Result IsFitForSuitModifier(Card card)
        {
            if (Game.SuitModifier.ItIsFit(card))
            {
                return HasPunishment(card);
            }
            else return Result.SuitIsNotFitForModifier;
        }

        private Result IsItCounter(Card card)
        {
            if (Game.Rules.IsItActiveCard(card, out ActiveCardProperties properties)
                && properties.HasPunishmentCounter(out Counter counter)
                   && counter(Game.DiscardPile))
            {
                return Result.Success;
            }
            else return Result.NotCounter;
        }

      

    


        public Result CardIsFit(Card card)
        {
            if (Game.SuitModifier != null && Game.RankModifier != null)
            {
                return IsFitForRankAndSuitModifiers(card);
            }
            else if (Game.SuitModifier != null)
            {
                return IsFitForSuitModifier(card);
            }
            else if (Game.RankModifier != null)
            {
                return IsFitForRankModifier(card);
            }
            else if (Game.Punishment != null)
            {
                return IsItCounter(card);
            }
            else return IsSameSuitOrRankOrSpecial(card);
        }
        */
        private bool IsItValidIndex(int index, ReadOnlyDynamicArray<Card> hand)
        {
            return index >= 0 && index < hand.Count;
        }
        public Result IsValidThrow(ReadOnlyDynamicArray<Card> hand, ReadOnlyDynamicArray<int> selection)
        {
            return IsSelectionNotEmpty(hand, selection);
        }

        private Result IsSelectionNotEmpty(ReadOnlyDynamicArray<Card> hand, ReadOnlyDynamicArray<int> selection)
        {
            if (selection.IsEmpty)
            {
                return Result.ThrowSelectionIsEmpty;
            }
            else return CanThrowMoreCardsAtOnce(hand, selection);
        }
    }
}