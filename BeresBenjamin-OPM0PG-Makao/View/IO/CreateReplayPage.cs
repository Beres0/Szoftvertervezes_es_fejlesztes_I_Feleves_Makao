namespace Makao.View
{
    public class CreateReplayPage : CreateFilePage
    {
        protected override string Directory => Application.Settings.ReplaysDirectory;

        protected override string ErrorMessage => "Nem sikerült létrehozni a visszajátszást!";

        protected override string Extension => Application.Settings.ReplaysExtension;

        protected override string FileNameTextBoxName => "Visszajátszás neve";

        protected override string OverwriteMessage => "Ilyen nevű visszajátszás már létezik!";

        protected override string SuccessMessage => "A visszajátszás sikeresen létre lett hozva!";

        public CreateReplayPage(PlayableGamePage gamePage) : base("Visszajátszás létrehozása", gamePage)
        {
            SetControllers();
        }

        protected override void SetControllers()
        {
            base.SetControllers();
            controllers.AddNavigator(() => Application.MainMenu, "Vissza a főmenűbe");
        }
    }
}