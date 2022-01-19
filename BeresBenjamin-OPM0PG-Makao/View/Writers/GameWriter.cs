using Makao.Collections;
using Makao.Model;
using System;

namespace Makao.View
{
    public class GameWriter : IWriter
    {
        private ResponseWriter responseWriter;
        public Game Game => GamePage.Game;
        public GamePage GamePage { get; }
        public DeckGraphics Graphics => GamePage.GameVariant.Graphics;

        public bool PublicWrite
        {
            get; set;
        }

        public ApplicationSettings Settings => GamePage.Application.Settings;

        public GameWriter(GamePage page)
        {
            GamePage = page;
            responseWriter = new ResponseWriter(this);
        }

        public void Write()
        {
            WritePlayers();
            ConsoleHelper.WriteSepartorLine(Settings.SepLineChar);
            WriteTopOfDiscardPile();
            ConsoleHelper.WriteSepartorLine(Settings.SepLineChar);
            WriteActiveEffects();
            WriteMessage();
        }

        public void WriteActiveEffects()
        {
            if (!Game.IsEndOfGame)
            {
                Console.Write("Érvényben lévő hatások: ");

                DynamicArray<string> effects = new DynamicArray<string>();

                if (Game.Punishment is MissATurn)
                {
                    effects.Add($"Kimaradsz a körből({Game.Punishment.Stack})");
                }
                else if (Game.Punishment is DrawExtraCards)
                {
                    effects.Add($"Húzz extra kártyát({Game.Punishment.Stack})");
                }

                if (Game.RankModifier != null)
                {
                    string rank = Graphics.Ranks[Game.RankModifier.Rank].Name;
                    string duration = Game.Rules.ThrowModifierDuration != ThrowModifierDuration.NextValidThrow ?
                        $"({Game.RankModifier.Duration})" : "";
                    effects.Add($"Rang módosító: {rank}{duration}");
                }
                if (Game.SuitModifier != null)
                {
                    string suit = Graphics.Suits[Game.SuitModifier.Suit].Name;
                    string duration = Game.Rules.ThrowModifierDuration != ThrowModifierDuration.NextValidThrow ?
                        $"({Game.SuitModifier.Duration})" : "";
                    effects.Add($"Szín módosító: {suit}{duration}");
                }

                if (effects.IsEmpty)
                {
                    Console.WriteLine("Nincs");
                }
                else
                {
                    ConsoleHelper.WriteColorizedText(string.Join(", ", effects) + "\n", GamePage.Application.Settings.HighlightedColor);
                }
                ConsoleHelper.WriteSepartorLine(GamePage.Application.Settings.SepLineChar);
            }
        }

        public void WriteMessage()
        {
            if (GamePage.Phase == GamePhase.IsNext && GamePage.CurrentPlayer == null)
            {
                ConsoleHelper.WriteColorizedText("Játék indul!\n", GamePage.Application.Settings.HighlightedColor);
                ConsoleHelper.WriteSepartorLine(Settings.SepLineChar);
            }
            else if (GamePage.Phase == GamePhase.IsNext)
            {
                ConsoleHelper.WriteColorizedText($"{Game.ActivePlayer.Name}! Te következel!\n", Settings.HighlightedColor);
                ConsoleHelper.WriteSepartorLine(Settings.SepLineChar);
            }
            else if (Game.LastResponse.Request.Source == GamePage.CurrentPlayer)
            {
                responseWriter.Write(Game.LastResponse);
                ConsoleHelper.WriteSepartorLine(Settings.SepLineChar);
            }
        }

        public void WritePlayerAndHand(Player player, PlayerStatus.Type status)
        {
            if (status == PlayerStatus.Type.Playing)
            {
                if (!Game.IsEndOfGame)
                {
                    if (player == GamePage.CurrentPlayer)
                    {
                        ConsoleHelper.WriteColorizedText(player.Name + " ", Settings.CurrentPlayerColor);
                    }
                    else
                    {
                        ConsoleHelper.WriteColorizedText(player.Name + " ", Settings.HighlightedColor);
                    }

                    if (PublicWrite || player is HumanPlayer)
                    {
                        ConsoleHelper.WriteCards(player.Hand, Graphics);
                    }
                    else
                    {
                        for (int i = 0; i < player.Hand.Count; i++)
                        {
                            ConsoleHelper.WriteCard(Graphics.Facedown);
                            Console.Write(' ');
                        }
                    }
                }
                else
                {
                    ConsoleHelper.WriteColorizedText(player.Name + " ", Settings.LeaverColor);
                }

                Console.WriteLine();
            }
            else if (status == PlayerStatus.Type.Leaver)
            {
                ConsoleHelper.WriteColorizedText(player.Name + " feladta\n", Settings.LeaverColor);
            }
            else if (status == PlayerStatus.Type.Winner)
            {
                Game.Winners.Contains(player, out int index);
                ConsoleHelper.WriteColorizedText($"{player.Name} {index + 1}. nyertes\n", Settings.WinnerColor);
            }
        }

        public void WritePlayers()
        {
            WritePlayerAndHand(Game.GetPlayer(0), Game.GetPlayerStatus(0));

            for (int i = 1; i < Game.PlayersCount; i++)
            {
                ConsoleHelper.WriteSepartorLine('-');
                WritePlayerAndHand(Game.GetPlayer(i), Game.GetPlayerStatus(i));
            }
        }

        public void WriteTopOfDiscardPile()
        {
            if (!Game.IsEndOfGame)
            {
                Console.Write("Utolsó kártya: ");
                CardGraphics cardGraph = Graphics[Game.TopOfDiscardPile.Value];
                ConsoleHelper.WriteColorizedText($"{cardGraph.Name} ", Settings.HighlightedColor);
                ConsoleHelper.WriteCard(cardGraph);
                Console.WriteLine();
            }
        }
    }
}