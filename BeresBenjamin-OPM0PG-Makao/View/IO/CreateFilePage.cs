using System;
using System.IO;

namespace Makao.View
{
    public abstract class CreateFilePage : Page
    {
        public enum IOState
        {
            None, WriteSuccess, Overwrite, Error
        }

        protected TextBox fileName;
        protected Selector<bool> overwriteSelector;
        protected IOState state;
        protected abstract string Directory { get; }
        protected abstract string ErrorMessage { get; }
        protected abstract string Extension { get; }
        protected abstract string FileNameTextBoxName { get; }
        protected abstract string OverwriteMessage { get; }
        protected string Path => Directory + "\\" + fileName.Text + "." + Extension;
        protected abstract string SuccessMessage { get; }
        public PlayableGamePage GamePage { get; }

        public CreateFilePage(string name, PlayableGamePage gamePage) : base(name, gamePage.Application)
        {
            GamePage = gamePage;
            state = IOState.None;
        }

        private void ResetIOState(object sender, EventArgs e)
        {
            if (state != IOState.None)
            {
                state = IOState.None;
            }
        }

        private void SetOverwriteSelector()
        {
            overwriteSelector = controllers.AddSelector
               (() => 0,
               () => 1,
               (i) => i > 0,
               "Felülírod:",
               () => state == IOState.Overwrite,
               (b) => b ? "Igen" : "Nem");
        }

        private void SetSave()
        {
            controllers.AddController("Létrehoz").SetCommand(ConsoleKey.Enter, Save);

            void Save()
            {
                if (state == IOState.Overwrite && overwriteSelector.CurrentValue)
                {
                    OverwriteOrSave();
                }
                else if (!File.Exists(Path))
                {
                    MakaoFile file = new MakaoFile(Path, GamePage);
                    OverwriteOrSave();
                }
                else
                {
                    state = IOState.Overwrite;
                }
            }
        }

        protected void OverwriteOrSave()
        {
            MakaoFile file = new MakaoFile(Path, GamePage);
            try
            {
                file.WriteToFile(Path);
                state = IOState.WriteSuccess;
            }
            catch (IOException)
            {
                state = IOState.Error;
            }
            overwriteSelector.ResetController();
        }

        protected override void SetControllers()
        {
            string defaultText = $"{GamePage.Name}_{DateTime.Now:yyyy_MM_dd_HH_mm_ss}";
            fileName = controllers.AddTextBox(100, defaultText, FileNameTextBoxName);
            fileName.TextBoxChanged += ResetIOState;

            SetOverwriteSelector();
            SetSave();
        }

        protected override void WriteContent()
        {
            WriteMessage();
            controllers.Write();
        }

        protected void WriteMessage()
        {
            if (state == IOState.Error)
            {
                Console.WriteLine(ErrorMessage);
                WriteSeparatorLine();
            }
            else if (state == IOState.WriteSuccess)
            {
                Console.WriteLine(SuccessMessage);
                WriteSeparatorLine();
            }
            else if (state == IOState.Overwrite)
            {
                Console.WriteLine(OverwriteMessage);
                WriteSeparatorLine();
            }
        }
    }
}