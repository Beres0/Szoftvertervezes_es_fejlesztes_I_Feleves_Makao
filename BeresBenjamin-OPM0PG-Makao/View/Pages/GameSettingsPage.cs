using Makao.GameVariants;
using System;

namespace Makao.View
{
    public class GameSettingsPage : Page
    {
        private TextBox gameName;
        private Selector<int> numberOfPlayers;
        private TextBox playerName;
        public string GameName => gameName.Text;
        public GameVariant GameVariant => GameVariantsPage.SelectedVariant;
        public GameVariantsPage GameVariantsPage { get; }
        public int NumberOfPlayers => numberOfPlayers.CurrentIndex;
        public string PlayerName => playerName.Text;

        public GameSettingsPage(GameVariantsPage variantsPage) : base("Játék beállítások", variantsPage.Application)
        {
            GameVariantsPage = variantsPage;
            SetControllers();
        }

        protected override void SetControllers()
        {
            controllers.AddNavigator(() => new PlayableGamePage(this), "Kezdés");
            playerName = controllers.AddTextBox(100, "Játékos", "Játékos neve");
            numberOfPlayers = controllers.AddSelector(() => 2, () => GameVariant.Rules.MaxPlayer, (i) => i, "Játékosok száma");
            gameName = controllers.AddTextBox(100, $"{GameVariant.Name}", "Játék neve");
            controllers.AddNavigator(() => GameVariantsPage, "Vissza");
        }

        protected override void WriteContent()
        {
            Console.WriteLine($"Játéktípus: {GameVariant.Name}");
            WriteSeparatorLine();
            controllers.Write();
        }
    }
}