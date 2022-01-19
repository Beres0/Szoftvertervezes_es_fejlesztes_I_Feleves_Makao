namespace Makao.Model
{
    public class PassRequest : PlayerRequest
    {
        public PassRequest(Player source) : base(source, RequestType.Pass)
        {
        }
    }
}