using Makao.Collections;
using Makao.GameVariants;

namespace Makao.View
{
    public class GameVariantsPage : Page
    {
        private Selector<GameVariantsWriter> variantSelector;
        public MainMenu MainMenu { get; }
        public GameVariant SelectedVariant => variantSelector.CurrentValue.GameVariant;

        public GameVariantsPage(MainMenu mainMenuPage) : base("Játéktípus választás", mainMenuPage.Application)
        {
            MainMenu = mainMenuPage;
            SetControllers();
        }

        protected override void SetControllers()
        {
            DynamicArray<GameVariantsWriter> writers = new DynamicArray<GameVariantsWriter>();
            for (int i = 0; i < Application.GameVariants.Count; i++)
            {
                writers.Add(new GameVariantsWriter(Application.GameVariants[i]));
            }
            variantSelector = controllers.AddSelector(writers, "Játéktípus", null, (writer) => writer.GameVariant.Name)
                              .SetCommand(Application.Settings.EnterKey, () => Application.CurrentPage = new GameSettingsPage(this));

            controllers.AddNavigator(() => MainMenu, "Vissza");
        }

        protected override void WriteContent()
        {
            controllers.Write();
            WriteSeparatorLine();
            variantSelector.CurrentValue.Write();
        }
    }
}