using Makao.Collections;
using System;

namespace Makao.Model
{
    public abstract class PlayerRequest
    {
        public Player Source { get; }

        public RequestType Type { get; }

        public PlayerRequest(Player source, RequestType type)
        {
            Source = source;
            Type = type;
        }

        public static PlayerRequest Parse(Player player, string requestString)
        {
            string[] splitted = requestString.Split("-");

            if (Enum.TryParse(splitted[0], out RequestType type))
            {
                if (splitted.Length == 1)
                {
                    switch (type)
                    {
                        case RequestType.Draw: return new DrawRequest(player);
                        case RequestType.Leave: return new LeaveRequest(player);
                        case RequestType.Pass: return new PassRequest(player);
                    }
                }
                else if (splitted.Length == 2 && int.TryParse(splitted[1], out int value))
                {
                    switch (type)
                    {
                        case RequestType.Rank: return new RankRequest(player, value);
                        case RequestType.Suit: return new SuitRequest(player, value);
                    }
                }
                else if (splitted.Length == 3 && type == RequestType.Throw)
                {
                    DynamicArray<int> selection = new DynamicArray<int>();
                    bool tryParseSuccess = true;

                    if (!string.IsNullOrEmpty(splitted[1]))
                    {
                        string[] selectionStr = splitted[1].Split(',');
                        int i = 0;

                        while (i < selectionStr.Length && tryParseSuccess)
                        {
                            if (int.TryParse(selectionStr[i], out int index))
                            {
                                selection.Add(index);
                            }
                            else tryParseSuccess = false;
                            i++;
                        }
                    }

                    if (tryParseSuccess)
                    {
                        return new ThrowRequest(player, selection, ThrowRequest.FormatKeywords(splitted[2]));
                    }
                }
            }
            throw new FormatException($"{nameof(requestString)} is not well-formed! \"{requestString}\"");
        }

        public bool SentByTheSource()
        {
            return Source.Auth == this;
        }

        public override string ToString()
        {
            return Type.ToString();
        }
    }

    [Flags]
    public enum RequestType
    {
        Draw = 1, Join = 2, Leave = 4, Pass = 8, Rank = 16, Suit = 32, Throw = 64
    }
}