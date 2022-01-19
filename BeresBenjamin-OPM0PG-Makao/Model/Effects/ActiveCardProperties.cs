using Makao.Collections;

namespace Makao.Model
{
    public class ActiveCardProperties
    {
        public Ask Ask { get; }
        public GameModifier GameModifiers { get; }
        public Punishment Punishment { get; }
        public Counter Counter { get; }

        public ActiveCardProperties
         (
            Punishment punishment = null,
            Ask askModifier = Ask.None,
            GameModifier gameModifier = GameModifier.None,
            Counter counter = null
            )
        {
            Punishment = punishment;
            Ask = askModifier;
            GameModifiers = gameModifier;
            Counter = counter;
        }

        public bool HasPunishmentCounter(out Counter counter)
        {
            counter = Counter;
            return counter != null;
        }
    }

    public delegate bool Counter(ReadOnlyDynamicArray<Card> discardPile);
}