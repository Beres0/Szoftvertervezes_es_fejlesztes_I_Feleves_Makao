using Makao.Collections;
using Makao.GameVariants;
using Makao.Model;
using System;

namespace Makao.View
{
    public abstract class GamePage : Page
    {
        protected GameWriter gameWriter;
        protected DynamicArray<string> humanPlayerRequests;
        public Player CurrentPlayer { get; protected set; }
        public Game Game { get; }
        public GameVariant GameVariant { get; }
        public HumanPlayer Human { get; }
        public ReadOnlyDynamicArray<string> HumanPlayerRequests { get; }
        public int NumberOfPlayers { get; }
        public GamePhase Phase { get; protected set; }
        public Page PreviousPage { get; }

        public GamePage(string name,
                       Page previousPage,
                       string humanPlayerName,
                       DynamicArray<string> requests,
                       int numberOfPlayers,
                       GameVariant gameVariant,
                       int? seed = null)
         : base(string.IsNullOrEmpty(name) ? "Névtelen játék" : name, previousPage.Application)
        {
            PreviousPage = previousPage;
            GameVariant = gameVariant;
            NumberOfPlayers = numberOfPlayers;
            Human = new HumanPlayer(string.IsNullOrEmpty(humanPlayerName) ? "Névtelen" : humanPlayerName);
            Game = seed.HasValue ? new Game(GameVariant.Rules, seed.Value) : new Game(GameVariant.Rules);
            humanPlayerRequests = requests;
            HumanPlayerRequests = requests.AsReadOnly();
            gameWriter = new GameWriter(this);
            JoinPlayers();
        }

        public GamePage(MakaoFile file, FileReaderPage readerPage) :
          this(file.GameName,
               readerPage,
               file.HumanPlayerName,
               file.HumanPlayerRequests.CopyAsDynamicArray(),
               file.NumberOfPlayer,
               file.GameVariant,
               file.Seed
               )
        {
        }

        protected void JoinPlayers()
        {
            Human.Join(Game);
            for (int i = 1; i < NumberOfPlayers; i++)
            {
                new BotPlayer($"Gép-{i:D2}").Join(Game);
            }
        }

        protected void Playback(Game game)
        {
            game.StartTheGame();

            int i = 0;
            while (i < humanPlayerRequests.Count || game.ActivePlayer is BotPlayer)
            {
                if (game.ActivePlayer is BotPlayer bot)
                {
                    bot.DoSomething();
                }
                else if (game.ActivePlayer is HumanPlayer human)
                {
                    try
                    {
                        human.Send(humanPlayerRequests[i]);
                        i++;
                    }
                    catch (FormatException)
                    {
                        throw;
                    }
                }
            }
        }

        protected override void WriteContent()
        {
            gameWriter.Write();
            controllers.Write();
        }
    }

    public enum GamePhase
    {
        IsNext, BotTurn, PlayerTurn
    }
}