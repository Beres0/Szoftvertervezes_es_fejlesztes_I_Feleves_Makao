using System;

namespace Makao.View
{
    public abstract class Page : IWriter, IController
    {
        protected readonly ControllerCollection controllers;
        public Application Application { get; }
        public string ControlKeys => controllers.ControlKeys;
        public string Name { get; }

        protected Page(string name, Application application)
        {
            Name = name;
            Application = application;
            controllers = new ControllerCollection(Application)
                         .SetPrevControllerCommand(Application.Settings.UpKey)
                         .SetNextControllerCommand(Application.Settings.DownKey);
        }

        protected abstract void SetControllers();

        protected abstract void WriteContent();

        protected virtual void WriteControlKeys()
        {
            Console.WriteLine(ControlKeys);
        }

        protected virtual void WriteHeader()
        {
            ConsoleHelper.WriteColorizedTextCenter(Name + '\n', Application.Settings.HighlightedColor);
        }

        protected void WriteSeparatorLine()
        {
            ConsoleHelper.WriteSepartorLine(Application.Settings.SepLineChar);
        }

        public virtual bool Control(ConsoleKeyInfo key)
        {
            if (controllers.Control(key))
            {
                Console.Clear();
                Application.CurrentPage.Write();
                return true;
            }
            else return false;
        }

        public virtual void Write()
        {
            WriteHeader();
            WriteSeparatorLine();
            WriteContent();
            WriteSeparatorLine();
            WriteControlKeys();
        }
    }
}