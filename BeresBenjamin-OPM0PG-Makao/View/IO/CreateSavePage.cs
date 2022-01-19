namespace Makao.View
{
    public class CreateSavePage : CreateFilePage
    {
        protected override string Directory => Application.Settings.SavesDirectory;

        protected override string ErrorMessage => "Nem sikerült létrehozni a mentést!";

        protected override string Extension => Application.Settings.SavesExtension;

        protected override string FileNameTextBoxName => "Mentés neve";

        protected override string OverwriteMessage => "A mentés már létezik!";

        protected override string SuccessMessage => "A mentés sikeresen létre lett hozva!";

        public CreateSavePage(PlayableGamePage gamePage) : base("Mentés létrehozása", gamePage)
        {
            SetControllers();
        }

        protected override void SetControllers()
        {
            base.SetControllers();
            controllers.AddNavigator(() => GamePage, "Vissza a játékba");
        }
    }
}