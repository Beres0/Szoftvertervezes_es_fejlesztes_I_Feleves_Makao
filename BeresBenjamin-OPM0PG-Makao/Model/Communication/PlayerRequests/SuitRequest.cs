namespace Makao.Model
{
    public class SuitRequest : PlayerRequest
    {
        public int Suit { get; }

        public SuitRequest(Player source, int suit) : base(source, RequestType.Suit)
        {
            Suit = suit;
        }

        public override string ToString()
        {
            return base.ToString() + "-" + Suit;
        }
    }
}