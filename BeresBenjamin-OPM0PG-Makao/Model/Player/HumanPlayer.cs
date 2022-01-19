using Makao.Collections;
using System;

namespace Makao.Model
{
    public class HumanPlayer : Player
    {
        public HumanPlayer(string name) : base(name)
        {
        }

        private ReadOnlyDynamicArray<string> FormatKeywords(string keywords)
        {
            if (!string.IsNullOrWhiteSpace(keywords))
            {
                string[] splitted = keywords.Split(',');

                DynamicArray<string> kwords = new DynamicArray<string>();

                for (int i = 0; i < splitted.Length; i++)
                {
                    kwords.Add(Keyword.FormatKeyword(splitted[i]));
                }
                return kwords.AsReadOnly();
            }
            else return ReadOnlyDynamicArray<string>.Empty;
        }

        public void AskRank(int rank)
        {
            CheckIsIngame();
            Send(Game, new RankRequest(this, rank));
        }

        public void AskSuit(int suit)
        {
            CheckIsIngame();
            Send(Game, new SuitRequest(this, suit));
        }

        public void Draw()
        {
            CheckIsIngame();
            Send(Game, new DrawRequest(this));
        }

        public void Pass()
        {
            CheckIsIngame();
            Send(Game, new PassRequest(this));
        }

        public void Send(string requestString)
        {
            CheckIsIngame();
            try
            {
                Send(Game, PlayerRequest.Parse(this, requestString));
            }
            catch (FormatException)
            {
                throw;
            }
        }

        public void Throw(IDynamicArray<int> selection, string keywords)
        {
            CheckIsIngame();
            Send(Game, new ThrowRequest(this, selection, FormatKeywords(keywords)));
        }
    }
}