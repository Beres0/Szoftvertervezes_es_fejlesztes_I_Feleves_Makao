namespace Makao.Model
{
    public class DrawRequest : PlayerRequest
    {
        public DrawRequest(Player source) : base(source, RequestType.Draw)
        {
        }
    }
}