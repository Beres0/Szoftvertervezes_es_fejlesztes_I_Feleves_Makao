using Makao.GameVariants;
using Makao.Model;
using System;

namespace Makao.View
{
    public class ReplayGamePage : GamePage
    {
        private int requestIndex;

        public ReplayGamePage(MakaoFile file, FileReaderPage readerPage) :
          base(file, readerPage)
        {
            gameWriter.PublicWrite = true;
            SetControllers();
            TestWalktrough();
            Game.StartTheGame();
        }

        private void SetNext()
        {
            controllers.AddController("Tovább", () => !Game.IsEndOfGame).SetCommand(ConsoleKey.Enter, Next);

            void Next()
            {
                if (CurrentPlayer != Game.ActivePlayer)
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
                        Human.Send(humanPlayerRequests[requestIndex]);
                        requestIndex++;
                    }
                }
            }
        }

        private void TestWalktrough()
        {
            Game game = new Game(GameVariant.Rules, Game.Seed);

            new HumanPlayer("temp").Join(game);
            for (int i = 1; i < NumberOfPlayers; i++)
            {
                new BotPlayer("temp").Join(game);
            }
            Playback(game);

            if (!game.IsEndOfGame)
            {
                throw new FormatException("Replay file is corrupt!");
            }
        }

        protected override void SetControllers()
        {
            SetNext();
            controllers.AddNavigator(() => Application.MainMenu, "Vissza a főmenűbe");
        }
    }
}