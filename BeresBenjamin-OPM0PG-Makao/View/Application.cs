using Makao.Collections;
using Makao.GameVariants;
using System;
using System.IO;

namespace Makao.View
{
    public class Application
    {
        private DynamicArray<GameVariant> gameVariants;
        private bool run;
        public Page CurrentPage { get; set; }
        public ReadOnlyDynamicArray<GameVariant> GameVariants { get; private set; }
        public MainMenu MainMenu { get; private set; }
        public bool ReplaysExists { get; }
        public bool SavesExists { get; }
        public ApplicationSettings Settings { get; }

        public Application()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Settings = new ApplicationSettings();

            MainMenu = new MainMenu(this);
            CurrentPage = MainMenu;
            run = true;
            SetGameVariants();
            SavesExists = CreateDirectory(Settings.SavesDirectory);
            ReplaysExists = CreateDirectory(Settings.ReplaysDirectory);
        }

        private void Control(ConsoleKeyInfo key)
        {
            CurrentPage.Control(key);
        }

        private void SetGameVariants()
        {
            gameVariants = new DynamicArray<GameVariant>();
            DeckGraphicsBuilder hungarian = new HungarianGraphicsBuilder();

            gameVariants.Add(new GameVariant("Magyar-Makaó-01", new HungarianRulesBuilder(), hungarian));
            gameVariants.Add(new GameVariant("Magyar-Makaó-02", new HungarianAlterRulesBuilder(), hungarian));
            gameVariants.Add(new GameVariant("Francia-Makaó", new FrenchRulesBuilder(), new FrenchGraphicsBuilder()));
            gameVariants.Add(new GameVariant("UNO", new UnoRulesBuilder(), new UnoGraphicsBuilder()));
            GameVariants = gameVariants.AsReadOnly();
        }

        private bool CreateDirectory(string directory)
        {
            if (!Directory.Exists(directory))
            {
                try
                {
                    Directory.CreateDirectory(directory);
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        public void Exit()
        {
            run = false;
        }

        public void Run()
        {
            CurrentPage.Write();
            while (run)
            {
                Control(Console.ReadKey(true));
            }
        }
    }
}