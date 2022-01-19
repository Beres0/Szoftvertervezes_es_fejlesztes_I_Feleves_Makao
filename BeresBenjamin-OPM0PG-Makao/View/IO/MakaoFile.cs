using Makao.Collections;
using Makao.GameVariants;
using System;
using System.IO;
using System.Text;

namespace Makao.View
{
    public class MakaoFile
    {
        private const int firstLineMembersLength = 6;
        private const char Sep = ';';
        public Application Application { get; }
        public string GameName { get; private set; }
        public GameVariant GameVariant { get; private set; }
        public bool HasError { get; private set; }
        public string HumanPlayerName { get; private set; }
        public ReadOnlyDynamicArray<string> HumanPlayerRequests { get; private set; }
        public int NumberOfPlayer { get; private set; }
        public string Path { get; }
        public int Seed { get; private set; }

        public MakaoFile(string path, PlayableGamePage page)
        {
            Path = path;
            Application = page.Application;
            GameName = page.Name;
            Seed = page.Game.Seed;
            GameVariant = page.GameVariant;
            NumberOfPlayer = page.NumberOfPlayers;
            HumanPlayerName = page.Human.Name;
            HumanPlayerRequests = page.HumanPlayerRequests.CopyAsDynamicArray().AsReadOnly();
            HasError = false;
        }

        public MakaoFile(string path, Application application)
        {
            try
            {
                HasError = false;
                Path = path;
                Application = application;
                string[] lines = File.ReadAllLines(Path);
                if (!NoError(lines))
                {
                    HasError = true;
                }
            }
            catch (IOException)
            {
                HasError = true;
            }
        }

        private object[] GetMembers()
        {
            return new object[]
            {
                GameName,
                Seed,
                GameVariant.Name,
                NumberOfPlayer,
                HumanPlayerName,
                HumanPlayerRequests.Count
            };
        }

        private bool IsFirstLineRight(string[] firstLine)
        {
            if (firstLine.Length == firstLineMembersLength)
            {
                GameName = firstLine[0];
                return IsSeedRight(firstLine);
            }
            else return false;
        }

        private bool IsGameVariantRight(string[] firstLine)
        {
            if (Application.GameVariants.Contains(g => g.Name == firstLine[2], out int index))
            {
                GameVariant = Application.GameVariants[index];
                return IsNumberOfPlayerRight(firstLine);
            }
            else return false;
        }

        private bool IsNumberOfPlayerRight(string[] firstLine)
        {
            if (int.TryParse(firstLine[3], out int result))
            {
                NumberOfPlayer = result;
                HumanPlayerName = firstLine[4];
                return true;
            }
            else return false;
        }

        private bool IsSeedRight(string[] firstLine)
        {
            if (int.TryParse(firstLine[1], out int result))
            {
                Seed = result;
                return IsGameVariantRight(firstLine);
            }
            else return false;
        }

        private bool NoError(string[] lines)
        {
            string[] firstLine = lines[0].Split(Sep);
            if (IsFirstLineRight(firstLine) && int.TryParse(firstLine[5], out int requestsCount) && requestsCount == lines.Length - 1)
            {
                DynamicArray<string> requests = new DynamicArray<string>();
                for (int i = 1; i < lines.Length; i++)
                {
                    requests.Add(lines[i]);
                }
                HumanPlayerRequests = requests.AsReadOnly();
                return true;
            }
            else return false;
        }

        public void WriteToFile(string path)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Join(Sep, GetMembers()));
            sb.Append(HumanPlayerRequests.Join("\n"));
            try
            {
                File.WriteAllText(path, sb.ToString());
            }
            catch (IOException)
            {
                throw;
            }
        }
    }

}