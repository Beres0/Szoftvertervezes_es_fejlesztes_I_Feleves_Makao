using Makao.Collections;
using System;

namespace Makao.Model
{
    public class BotPlayer : Player
    {
        public BotPlayer(string name) : base(name)
        {
        }

        private bool CanThrow(out ThrowRequest request)
        {
            DynamicArray<int> selection = GetThrowSelection();
            if (selection.IsEmpty || selection == null)
            {
                request = null;
                return false;
            }

            if (Hand.Count > 1 && selection.Count >= Hand.Count)
            {
                while (selection.Count >= Hand.Count)
                {
                    selection.TryPopLast(out int item);
                }
            }

            int countAfterThrow = Hand.Count - selection.Count;

            DynamicArray<string> words = new DynamicArray<string>();
            if (Game.Rules.NeedKeywords(Hand.Count, countAfterThrow, out ReadOnlyDynamicArray<Keyword> keywords))
            {
                for (int i = 0; i < keywords.Count; i++)
                {
                    words.Add(keywords[i].Word);
                }
            }

            request = new ThrowRequest(this, selection, words);
            return true;
        }

        private RankRequest ChooseRank()
        {
            DynamicArray<int>[] groups = HandGroupByRankExceptSpecials();

            FilterGroups(groups, (i) =>
            {
                bool hasCondition = Game.Rules.HasAskRankCondition(i, out RankRequestCondition condition);
                return !hasCondition || (hasCondition && condition.IsTrue(Hand.Count));
            });
            int rank = SelectBiggestGroup(groups);

            return new RankRequest(this, rank);
        }

        private SuitRequest ChooseSuit()
        {
            DynamicArray<int>[] groups = HandGroupBySuitExceptSpecials();
            int suit = SelectBiggestGroup(groups);

            return new SuitRequest(this, suit);
        }

        private void FilterGroups(DynamicArray<int>[] groups, Func<int, bool> P)
        {
            for (int i = 0; i < groups.Length; i++)
            {
                if (!P(i))
                {
                    groups = null;
                }
            }
        }

        private int GetGroupCount(DynamicArray<int> group)
        {
            return group != null ? group.Count : 0;
        }

        private DynamicArray<int> GetThrowableCardsSelection()
        {
            DynamicArray<int> selection = new DynamicArray<int>();
            for (int i = 0; i < Hand.Count; i++)
            {
                if (Game.ThrowValidator.IsValidCard(Hand[i]) == Result.Success)
                {
                    selection.Add(i);
                }
            }
            return selection;
        }

        private DynamicArray<int> GetThrowSelection()
        {
            DynamicArray<int> selection = GetThrowableCardsSelection();

            if (!selection.IsEmpty)
            {
                if (Game.Rules.Settings.HasFlag(RulesSettings.PlayersCanBeCalledBack) && Game.GetPlayer(Game.NextPlayerIndex).Hand.Count == 0)
                {
                    if (SelectionContainsDrawExtraCards(selection, out int index))
                    {
                        return new DynamicArray<int>(new int[] { selection[index] });
                    }
                }

                if (SelectionContainsNormalCard(selection))
                {
                    NormalCardsFilter(selection);
                    DynamicArray<int>[] rankGroups = HandGroupBySelectionRanks(selection);
                    int biggestIndex = SelectBiggestGroup(rankGroups);

                    return rankGroups[biggestIndex];
                }
                else
                {
                    return new DynamicArray<int>(new int[] { selection[0] });
                }
            }
            else return selection;
        }

        private DynamicArray<int>[] HandGroupByRankExceptSpecials()
        {
            DynamicArray<int>[] groups = new DynamicArray<int>[Game.Rules.Deck.NumberOfRanks];
            for (int i = 0; i < Hand.Count; i++)
            {
                Card card = Hand[i];
                if (!card.IsSpecial)
                {
                    int groupIndex = card.Rank;

                    if (groups[groupIndex] == null)
                    {
                        groups[groupIndex] = new DynamicArray<int>();
                    }

                    groups[groupIndex].Add(i);
                }
            }
            return groups;
        }

        private DynamicArray<int>[] HandGroupBySuitExceptSpecials()
        {
            DynamicArray<int>[] groups = new DynamicArray<int>[Game.Rules.Deck.NumberOfSuits];
            for (int i = 0; i < Hand.Count; i++)
            {
                Card card = Hand[i];
                if (!card.IsSpecial)
                {
                    int groupIndex = card.Suit;

                    if (groups[groupIndex] == null)
                    {
                        groups[groupIndex] = new DynamicArray<int>();
                    }

                    groups[groupIndex].Add(i);
                }
            }

            return groups;
        }

        private void NormalCardsFilter(DynamicArray<int> selection)
        {
            int i = 0;
            while (i < selection.Count)
            {
                if (Game.Rules.IsItActiveCard(Hand[selection[i]], out ActiveCardProperties props))
                {
                    selection.RemoveAt(i);
                }
                else i++;
            }
        }

        private int SelectBiggestGroup(DynamicArray<int>[] groups)
        {
            int max = 0;
            for (int i = 1; i < groups.Length; i++)
            {
                if (GetGroupCount(groups[max]) < GetGroupCount(groups[i]))
                {
                    max = i;
                }
            }
            return max;
        }

        private bool SelectionContains(DynamicArray<int> selection, Func<Card, bool> P, out int index)
        {
            int i = 0;
            while (i < selection.Count && !P(Hand[selection[i]]))
            {
                i++;
            }
            index = i;
            return i < selection.Count;
        }

        private bool SelectionContainsDrawExtraCards(DynamicArray<int> selection, out int index)
        {
            Func<Card, bool> P = (c) => Game.Rules.IsItActiveCard(c, out ActiveCardProperties props) && props.Punishment is DrawExtraCards;
            return SelectionContains(selection, P, out index);
        }

        private bool SelectionContainsNormalCard(DynamicArray<int> selection)
        {
            return SelectionContains(selection, (c) => !Game.Rules.IsItActiveCard(c, out ActiveCardProperties props), out int index);
        }

        public void DoSomething()
        {
            if (Game.TypeOfState == typeof(WaitForDecisionState))
            {
                if (CanThrow(out ThrowRequest throwR))
                {
                    Send(Game, throwR);
                }
                else if (Game.AvaliableRequests.HasFlag(RequestType.Pass))
                {
                    Send(Game, new PassRequest(this));
                }
                else
                {
                    Send(Game, new DrawRequest(this));
                }
            }
            else if (Game.TypeOfState == typeof(WaitForRankState))
            {
                Send(Game, ChooseRank());
            }
            else if (Game.TypeOfState == typeof(WaitForSuitState))
            {
                Send(Game, ChooseSuit());
            }
        }

        public DynamicArray<int>[] HandGroupBySelectionRanks(DynamicArray<int> selection)
        {
            int length = Game.Rules.Deck.NumberOfRanks + Game.Rules.Deck.NumberOfSpecialRanks;
            DynamicArray<int>[] rankGroups = new DynamicArray<int>[length];

            for (int i = 0; i < selection.Count; i++)
            {
                if (rankGroups[Hand[selection[i]].Rank] == null)
                {
                    rankGroups[Hand[selection[i]].Rank] = new DynamicArray<int>();
                }

                rankGroups[Hand[selection[i]].Rank].Add(selection[i]);
            }

            for (int i = 0; i < Hand.Count; i++)
            {
                if (rankGroups[Hand[i].Rank] != null && !rankGroups[Hand[i].Rank].Contains(i, out int index))
                {
                    rankGroups[Hand[i].Rank].Add(i);
                }
            }

            return rankGroups;
        }
    }
}