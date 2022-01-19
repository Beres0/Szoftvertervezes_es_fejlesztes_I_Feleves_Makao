namespace Makao.Model
{
    public class RankRequest : PlayerRequest
    {
        public int Rank { get; }

        public RankRequest(Player source, int rank) : base(source, RequestType.Rank)
        {
            Rank = rank;
        }

        public override string ToString()
        {
            return base.ToString() + "-" + Rank;
        }
    }
}