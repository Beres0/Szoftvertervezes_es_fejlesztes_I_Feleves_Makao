namespace Makao.Model
{
    public class RankModifier : IDuration
    {
        public int Duration { get; }
        public int Rank { get; }

        public RankModifier(int duration, int rank)
        {
            Duration = duration;
            Rank = rank;
        }

        public RankModifier Decrease()
        {
            return this.Decrease(() => new RankModifier(Duration - 1, Rank));
        }

        public bool ItIsFit(Card card)
        {
            return card.Rank == Rank;
        }

        IDuration IDuration.Decrease()
        {
            return Decrease();
        }
    }
}