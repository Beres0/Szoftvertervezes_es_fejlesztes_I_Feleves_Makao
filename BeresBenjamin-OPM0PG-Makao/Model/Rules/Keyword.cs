namespace Makao.Model
{
    public class Keyword
    {
        public int Penalty { get; }

        public string Word { get; }

        public Keyword(string word, int penalty)
        {
            Word = FormatKeyword(word);
            Penalty = penalty;
        }

        public static string FormatKeyword(string keyword)
        {
            return keyword.ToUpper().Trim();
        }
    }
}