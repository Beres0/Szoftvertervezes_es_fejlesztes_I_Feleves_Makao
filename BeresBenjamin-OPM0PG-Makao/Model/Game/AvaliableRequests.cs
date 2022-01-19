using System;

namespace Makao.Model
{
    [Flags]
    public enum AvaliableRequests
    {
        None = 0, Leave = 1, Join = 2, Throw = 4, Pass = 8, Draw = 16, AskSuit = 32, AskRank = 64
    }
}