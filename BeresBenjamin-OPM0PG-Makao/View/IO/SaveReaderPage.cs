using System;

namespace Makao.View
{
    public class SaveReaderPage : FileReaderPage
    {
        protected override string DirectoryPath => Application.Settings.SavesDirectory;

        protected override string SearchPattern => $"*." + Application.Settings.SavesExtension;

        public SaveReaderPage(Application application) : base("Mentés betöltése", application)
        {
            SetControllers();
        }

        private void Load()
        {
            try
            {
                ResetErrorFieldsAndDeleteState();
                Application.CurrentPage = new PlayableGamePage(fileSelector.CurrentValue, this);
            }
            catch
            {
                hasLoadError = true;
            }
        }

        protected override void SetControllers()
        {
            base.SetControllers();
            fileSelector.SetCommand(ConsoleKey.Enter, Load, () => !CurrentItems.IsEmpty);
        }
    }
}