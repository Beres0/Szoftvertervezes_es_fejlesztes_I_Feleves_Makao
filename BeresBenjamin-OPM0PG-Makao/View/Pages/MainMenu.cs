namespace Makao.View
{
    public class MainMenu : Page
    {
        public MainMenu(Application application) : base("Főmenű", application)
        {
            SetControllers();
        }

        protected override void SetControllers()
        {
            controllers.AddNavigator(() => new GameVariantsPage(this), "Új játék");
            controllers.AddNavigator(() => new SaveReaderPage(Application), "Mentések");
            controllers.AddNavigator(() => new ReplayReaderPage(Application), "Visszajátszások");
            controllers.AddExit("Kilépés");
        }

        protected override void WriteContent()
        {
            controllers.Write();
        }
    }
}