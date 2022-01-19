using Makao.Collections;
using Makao.GameVariants;
using Makao.Model;
using System;

namespace Makao.View
{
    public class PlayableGamePage : GamePage
    {
        private MultipleSelector<Card> cardSelector;
        private TextBox keywords;
        private Selector<RankDescriptor> rankSelector;
        private Selector<KeyValuePair<RequestType, string>> requestSelector;
        private Selector<SuitDescriptor> suitSelector;

        public PlayableGamePage(GameSettingsPage settingsPage) :
            base(settingsPage.GameName,
                 settingsPage,
                 settingsPage.PlayerName,
                 new DynamicArray<string>(),
                 settingsPage.NumberOfPlayers,
                 settingsPage.GameVariant
                )
        {
            SetControllers();
            Game.StartTheGame();
        }

        public PlayableGamePage(MakaoFile file, FileReaderPage readerPage) :
            base(file, readerPage)
        {
            SetControllers();
            LoadGame();
        }

        private bool IsHumanTurn()
        {
            return Human == Game.ActivePlayer && Phase == GamePhase.PlayerTurn;
        }

        private void ResetControllers()
        {
            requestSelector.ResetController();
            keywords.ResetController();
            cardSelector.ResetController();
            rankSelector.ResetController();
            suitSelector.ResetController();
        }

        private void SetForfeit()
        {
            controllers.AddController("Feladom", IsHumanTurn).SetCommand(Application.Settings.EnterKey, Forfeit);

            void Forfeit()
            {
                Human.Leave();
                humanPlayerRequests.Add(Human.LastResponse.Request.ToString());
                while (!Game.IsEndOfGame)
                {
                    (Game.ActivePlayer as BotPlayer).DoSomething();
                }
            }
        }

        private void SetNext()
        {
            controllers.AddController("Tovább", () => !IsHumanTurn() && !Game.IsEndOfGame).SetCommand(ConsoleKey.Enter, Next);

            void Next()
            {
                if (Game.Winners.Contains(Human, out int index))
                {
                    while (!Game.IsEndOfGame)
                    {
                        (Game.ActivePlayer as BotPlayer).DoSomething();
                    }
                }
                else if (CurrentPlayer != Game.ActivePlayer)
                {
                    Phase = GamePhase.IsNext;
                    CurrentPlayer = Game.ActivePlayer;
                }
                else
                {
                    if (CurrentPlayer is BotPlayer bot)
                    {
                        Phase = GamePhase.BotTurn;
                        bot.DoSomething();
                    }
                    else if (CurrentPlayer == Human)
                    {
                        Phase = GamePhase.PlayerTurn;
                    }
                }
            }
        }

        private void SetPostGameControllers()
        {
            controllers.AddNavigator(() => new CreateReplayPage(this), "Visszajátszás készítése", () => Game.IsEndOfGame);
            controllers.AddNavigator(() => Application.MainMenu, "Visssza a főmenübe", () => Game.IsEndOfGame);
        }

        private void SetRankSelector()
        {
            rankSelector = controllers.AddSelector
                 (GameVariant.Graphics.Ranks,
                 "Rang",
                 () => IsHumanTurn() && requestSelector.CurrentValue.Key == RequestType.Rank,
                 (i) => i.Name);
        }

        private void SetRequestSelector()
        {
            AssociativeArray<RequestType, string> requests = new AssociativeArray<RequestType, string>();
            requests.Add(RequestType.Throw, "Dobok");
            requests.Add(RequestType.Draw, "Húzok");
            requests.Add(RequestType.Pass, "Passzolok");
            requests.Add(RequestType.Suit, "Színt kérek");
            requests.Add(RequestType.Rank, "Rangot kérek");

            requestSelector = controllers.AddSelector(requests, "Döntés", IsHumanTurn, (r) => r.Value)
                .SetCommand(Application.Settings.EnterKey, SendRequest);
            void SendRequest()
            {
                switch (requestSelector.CurrentValue.Key)
                {
                    case RequestType.Draw:
                        Human.Draw();
                        break;

                    case RequestType.Pass:
                        Human.Pass();
                        break;

                    case RequestType.Rank:
                        Human.AskRank(rankSelector.CurrentIndex);
                        break;

                    case RequestType.Suit:
                        Human.AskSuit(suitSelector.CurrentIndex);
                        break;

                    case RequestType.Throw:
                        Human.Throw(cardSelector.Selection, keywords.Text);
                        break;
                }
                humanPlayerRequests.Add(Human.LastResponse.Request.ToString());

                ResetControllers();
            }
        }

        private void SetSave()
        {
            controllers.AddNavigator(() => new CreateSavePage(this), "Mentés", IsHumanTurn);
        }

        private void SetSuitSelector()
        {
            suitSelector = controllers.AddSelector
                (GameVariant.Graphics.Suits,
                "Szín",
                () => IsHumanTurn() && requestSelector.CurrentValue.Key == RequestType.Suit,
                (i) => i.Name);
        }

        private void SetThrowControllers()
        {
            cardSelector = new MultipleSelector<Card>(Human.Hand, isActive: IsThrowActive)
                            .SetIncreaseCommand(Application.Settings.RightKey)
                            .SetDecreaseCommand(Application.Settings.LeftKey)
                            .SetToggleSelectCommand(Application.Settings.SelectKey)
                            .SetClearCommand(Application.Settings.DeleteKey)
                            .SetUndoCommand(Application.Settings.UndoKey);

            cardSelector.WriteMethod = WriteCardSelector;
            controllers.Add(cardSelector);

            keywords = controllers.AddTextBox(100, null, "Kulcsszó", IsThrowActive);

            bool IsThrowActive()
            {
                return IsHumanTurn() && requestSelector.CurrentValue.Key == RequestType.Throw;
            }
            void WriteCardSelector()
            {
                WriteCardSelectorTitles("Kártyáim:");
                for (int i = 0; i < cardSelector.Collection.Count; i++)
                {
                    WriteCard(i);
                    Console.Write(" ");
                }
                Console.WriteLine();

                WriteCardSelectorTitles("Szelekció:");
                for (int i = 0; i < cardSelector.Selection.Count; i++)
                {
                    WriteCard(cardSelector.Selection[i]);
                    Console.Write(" ");
                }
                Console.WriteLine();
            }
            void WriteCardSelectorTitles(string title)
            {
                if (controllers.CurrentController == cardSelector)
                {
                    ConsoleHelper.WriteColorizedText(title, Application.Settings.CurrentColor);
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine(title);
                }
            }
            void WriteCard(int index)
            {
                CardGraphics card = GameVariant.Graphics[cardSelector.Collection[index]];
                if (index == cardSelector.CurrentIndex && controllers.CurrentController == cardSelector)
                {
                    ConsoleHelper.WriteColorizedText(card.Text, ConsoleColor.Black, ConsoleColor.Cyan);
                }
                else
                {
                    ConsoleHelper.WriteCard(card);
                }
            }
        }

        protected override void SetControllers()
        {
            SetNext();
            SetRequestSelector();
            SetThrowControllers();
            SetSuitSelector();
            SetRankSelector();
            SetSave();
            SetForfeit();
            SetPostGameControllers();
        }

        public void LoadGame()
        {
            Game.StartTheGame();
            Playback(Game);

            CurrentPlayer = Human;
            Phase = GamePhase.IsNext;
        }
    }
}