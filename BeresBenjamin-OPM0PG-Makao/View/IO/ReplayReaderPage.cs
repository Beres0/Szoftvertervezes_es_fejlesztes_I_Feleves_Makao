using System;

namespace Makao.View
{
    public class ReplayReaderPage : FileReaderPage
    {
        protected override string DirectoryPath => Application.Settings.ReplaysDirectory;

        protected override string SearchPattern => $"*." + Application.Settings.ReplaysExtension;

        public ReplayReaderPage(Application application) : base("Visszajátszás betöltése", application)
        {
            SetControllers();
        }

        private void LoadReplay()
        {
            try
            {
                ResetErrorFieldsAndDeleteState();
                Application.CurrentPage = new ReplayGamePage(fileSelector.CurrentValue, this);
            }
            catch
            {
                hasLoadError = true;
            }
        }

        protected override void SetControllers()
        {
            base.SetControllers();
            fileSelector.SetCommand(ConsoleKey.Enter, LoadReplay, () => !CurrentItems.IsEmpty);
        }
    }
}