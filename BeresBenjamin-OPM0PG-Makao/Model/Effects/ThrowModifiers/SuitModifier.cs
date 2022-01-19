namespace Makao.Model
{
    public class SuitModifier : IDuration
    {
        public int Duration { get; }
        public int Suit { get; }

        public SuitModifier(int duration, int suit)
        {
            Duration = duration;
            Suit = suit;
        }

        public SuitModifier Decrease()
        {
            return this.Decrease(() => new SuitModifier(Duration - 1, Suit));
        }

        public bool ItIsFit(Card card)
        {
            return card.IsSpecial || card.Suit == Suit;
        }

        IDuration IDuration.Decrease()
        {
            return Decrease();
        }
    }
}