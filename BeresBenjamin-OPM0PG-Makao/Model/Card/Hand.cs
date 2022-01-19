using Makao.Collections;

namespace Makao.Model
{
    public class Hand
    {
        public int Count => ForGame.Count;

        public DynamicArray<Card> ForGame { get; private set; }

        public ReadOnlyDynamicArray<Card> ForPlayer { get; private set; }

        public Card this[int index] => ForGame[index];

        public Hand()
        {
            ForGame = new DynamicArray<Card>();
            ForPlayer = new ReadOnlyDynamicArray<Card>(ForGame);
        }

        public override string ToString()
        {
            return ForGame.ToString();
        }
    }
}