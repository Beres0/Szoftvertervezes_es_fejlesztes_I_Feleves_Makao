using Makao.Collections;
using Makao.GameVariants;
using Makao.Model;
using System;
using System.Text;

namespace Makao.View
{
    public class GameVariantsWriter : IWriter
    {
        private DynamicArray<Card> askRankCards;
        private DynamicArray<Card> askSuitCards;
        private DynamicArray<Card> drawExtraCards;
        private DynamicArray<Card> missATurnCards;
        private DynamicArray<Card> reverseCards;
        private StringBuilder sb;
        private DynamicArray<Card> specialCards;
        private DynamicArray<Card> swapHandCards;
        public GameVariant GameVariant { get; }

        public GameVariantsWriter(GameVariant gameVariant)
        {
            GameVariant = gameVariant;
            sb = new StringBuilder();

            SetCardGroups();
        }

        private string AskRankConditionToString()
        {
            sb.Clear();
            ReadOnlyDynamicArray<RankRequestCondition> askRankConditions = GameVariant.Rules.AskRankConditions;

            for (int i = 0; i < GameVariant.Rules.Deck.NumberOfRanks; i++)
            {
                RankRequestCondition condition = askRankConditions[i];
                if (condition != null)
                {
                    sb.Append($"-{GameVariant.Graphics.Ranks[i].Name} rangkérés feltétele: ");
                    if (condition.Relation == RankRequestCondition.RelationType.Equal)
                    {
                        sb.AppendLine($"Ha a játékosnak pontosan {condition.HandCount} kártyája van.");
                    }
                    else if (condition.Relation == RankRequestCondition.RelationType.Lower)
                    {
                        sb.AppendLine($"Ha a játékosnak kevesebb, mint {condition.HandCount} kártyája van.");
                    }
                    else if (condition.Relation == RankRequestCondition.RelationType.Greater)
                    {
                        sb.AppendLine($"Ha a játékosnak több, mint {condition.HandCount} kártyája van.");
                    }
                }
            }

            return sb.ToString();
        }

        private string DeckBasicsToString()
        {
            sb.Clear();
            sb.AppendLine("PAKLI TULAJDONSÁGAI");
            sb.Append("Színek: ");
            sb.AppendLine(GameVariant.Graphics.Suits.Join((i) => i.Name, ", "));
            sb.Append("Rangok: ");
            sb.AppendLine(GameVariant.Graphics.Ranks.Join((i) => i.Name, ", "));

            if (GameVariant.Rules.Settings.HasFlag(RulesSettings.AllowSpecialCards))
            {
                sb.Append("Speciális kártyák: ");
                sb.AppendLine(GameVariant.Graphics.SpecialCards.Join((i) => i.Rank.Name, ", "));
            }
            return sb.ToString();
        }

        private string KeywordsToString()
        {
            sb.Clear();
            ReadOnlyAssociativeArray<int, Keyword> keywords = GameVariant.Rules.ThrowingKeywords;
            if (!keywords.IsNullOrEmpty())
            {
                for (int i = 0; i < keywords.Count; i++)
                {
                    if (keywords[i].Key == 0)
                    {
                        sb.Append($"-Az utolsó ");
                    }
                    else if (keywords[i].Key == 1)
                    {
                        sb.Append($"-Az utolsó előtti ");
                    }
                    else
                    {
                        sb.Append($"-{keywords[i].Key} ");
                    }
                    sb.AppendLine($"lap ledobása előtt '{keywords[i].Value.Word}'-t kell mondani, különben fel kell húzni {keywords[i].Value.Penalty} lapot.");
                }
            }
            return sb.ToString();
        }

        private string NumberOfDecksToString()
        {
            sb.Clear();

            if (GameVariant.Rules.NumberOfDecks.IsNullOrEmpty())
            {
                sb.AppendLine($"-Játékpaklik száma 2-{GameVariant.Rules.MaxPlayer} játékosnál: 1.");
            }
            else
            {
                sb.Append("-Játékpaklik száma ");
                ReadOnlyAssociativeArray<int, int> numberOfDecks = GameVariant.Rules.NumberOfDecks;
                sb.AppendLine($"2-{numberOfDecks.GetKey(0) - 1} játékosnál: 1.");

                for (int i = 0; i < numberOfDecks.Count - 1; i++)
                {
                    sb.Append("-Játékpaklik száma ");
                    sb.AppendLine($"{numberOfDecks.GetKey(i)}-{numberOfDecks.GetKey(i + 1) - 1} játékosnál: {numberOfDecks.GetValue(i)}.");
                }

                sb.Append("-Játékpaklik száma ");
                sb.AppendLine
                    ($"{numberOfDecks[numberOfDecks.Count - 1].Key}-{GameVariant.Rules.MaxPlayer} játékosnál: {numberOfDecks.GetValue(numberOfDecks.Count - 1)}.");
            }
            return sb.ToString();
        }

        private string RulesSettingsToString()
        {
            sb.Clear();

            if (GameVariant.Rules.Settings.HasFlag(RulesSettings.CanThrowMoreCards))
            {
                sb.AppendLine("-Lehet egyszerre több kártyát dobni.");
            }
            if (GameVariant.Rules.Settings.HasFlag(RulesSettings.AllowSpecialCards))
            {
                sb.AppendLine("-Speciális kártyák engedélyezettek.");
            }
            if (GameVariant.Rules.Settings.HasFlag(RulesSettings.AllowPunishmentStacking))
            {
                sb.AppendLine("-Egymásra rakott büntető kártyák összeadódnak.");
            }
            if (GameVariant.Rules.Settings.HasFlag(RulesSettings.GameEndsWithTheFirstWinner))
            {
                sb.AppendLine("-Első nyertessel véget ér a játék.");
            }
            if (GameVariant.Rules.Settings.HasFlag(RulesSettings.PlayersCanBeCalledBack))
            {
                sb.AppendLine("-Játékost vissza lehet hívni.");
            }
            return sb.ToString();
        }

        private string RulesToString()
        {
            StringBuilder rulesSb = new StringBuilder();
            rulesSb.AppendLine("SZABÁLYOK");
            rulesSb.AppendLine($"-Maximum játékos: {GameVariant.Rules.MaxPlayer}.");
            rulesSb.AppendLine($"-Kezdő osztás: {GameVariant.Rules.StarterHandSize}.");
            rulesSb.Append(NumberOfDecksToString());
            rulesSb.Append(RulesSettingsToString());
            rulesSb.Append(ThrowModifierDurationToString());
            rulesSb.Append(KeywordsToString());
            rulesSb.Append(ThrowingExchangeToString());
            rulesSb.Append(AskRankConditionToString());
            return rulesSb.ToString();
        }

        private string ThrowingExchangeToString()
        {
            ReadOnlyAssociativeArray<int, int> exchanges = GameVariant.Rules.ThrowingExchanges;
            sb.Clear();
            if (!exchanges.IsNullOrEmpty())
            {
                for (int i = 0; i < exchanges.Count; i++)
                {
                    sb.AppendLine($"-{exchanges[i].Key} kártya ledobása után {exchanges[i].Value} kártyát kell húzni.");
                }
            }
            return sb.ToString();
        }

        private string ThrowModifierDurationToString()
        {
            sb.Clear();
            sb.Append("-Szín/rangkérők hatásának időtartama: ");
            if (GameVariant.Rules.ThrowModifierDuration == ThrowModifierDuration.NextValidThrow)
            {
                sb.AppendLine("Következő érvényes dobásig.");
            }
            else if (GameVariant.Rules.ThrowModifierDuration == ThrowModifierDuration.NextPlayer)
            {
                sb.AppendLine("Következő játékosig.");
            }
            else if (GameVariant.Rules.ThrowModifierDuration == ThrowModifierDuration.Turn)
            {
                sb.AppendLine("Körön át.");
            }
            return sb.ToString();
        }

        private void WriteCardsOfDeck()
        {
            Console.WriteLine("AKTÍV KÁRTYÁK");
            if (!missATurnCards.IsEmpty)
            {
                Console.WriteLine("'Kimaradsz' kártyák:");
                ConsoleHelper.WriteCards(missATurnCards, GameVariant.Graphics);
                Console.WriteLine();
            }
            if (!drawExtraCards.IsEmpty)
            {
                Console.WriteLine("Kártyahúzatók:");
                ConsoleHelper.WriteCards(drawExtraCards, GameVariant.Graphics);
                Console.WriteLine();
            }
            if (!askSuitCards.IsEmpty)
            {
                Console.WriteLine("Színkérők:");
                ConsoleHelper.WriteCards(askSuitCards, GameVariant.Graphics);
                Console.WriteLine();
            }
            if (!askRankCards.IsEmpty)
            {
                Console.WriteLine("Rangkérők:");
                ConsoleHelper.WriteCards(askRankCards, GameVariant.Graphics);
                Console.WriteLine();
            }
            if (!reverseCards.IsEmpty)
            {
                Console.WriteLine("Körfordítók:");
                ConsoleHelper.WriteCards(reverseCards, GameVariant.Graphics);
                Console.WriteLine();
            }
            if (!swapHandCards.IsEmpty)
            {
                Console.WriteLine("Kártyacserélők:");
                ConsoleHelper.WriteCards(swapHandCards, GameVariant.Graphics);
                Console.WriteLine();
            }
        }

        public void SetCardGroups()
        {
            drawExtraCards = new DynamicArray<Card>();
            missATurnCards = new DynamicArray<Card>();
            askSuitCards = new DynamicArray<Card>();
            askRankCards = new DynamicArray<Card>();
            reverseCards = new DynamicArray<Card>();
            swapHandCards = new DynamicArray<Card>();
            specialCards = new DynamicArray<Card>();

            DynamicArray<Card> cards;
            if (GameVariant.Rules.Settings.HasFlag(RulesSettings.AllowSpecialCards))
            {
                cards = GameVariant.Rules.Deck.GetDeckUniquesWithSpecial();
            }
            else
            {
                cards = GameVariant.Rules.Deck.GetDeckUniques();
            }

            for (int i = 0; i < cards.Count; i++)
            {
                if (GameVariant.Rules.IsItActiveCard(cards[i], out ActiveCardProperties props))
                {
                    if (props.Ask == Ask.Rank)
                    {
                        askRankCards.Add(cards[i]);
                    }
                    else if (props.Ask == Ask.Suit)
                    {
                        askSuitCards.Add(cards[i]);
                    }

                    if (props.Punishment is MissATurn)
                    {
                        missATurnCards.Add(cards[i]);
                    }
                    else if (props.Punishment is DrawExtraCards)
                    {
                        drawExtraCards.Add(cards[i]);
                    }

                    if (props.GameModifiers.HasFlag(GameModifier.Reverse))
                    {
                        reverseCards.Add(cards[i]);
                    }

                    if (props.GameModifiers.HasFlag(GameModifier.SwapHands))
                    {
                        swapHandCards.Add(cards[i]);
                    }
                    if (cards[i].IsSpecial)
                    {
                        specialCards.Add(cards[i]);
                    }
                }
            }
        }

        public void Write()
        {
            Console.WriteLine(DeckBasicsToString());
            WriteCardsOfDeck();
            Console.WriteLine();
            Console.Write(RulesToString());
        }
    }
}