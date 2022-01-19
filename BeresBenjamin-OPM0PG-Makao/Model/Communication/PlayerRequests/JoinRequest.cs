namespace Makao.Model
{
    public class JoinRequest : PlayerRequest
    {
        public JoinRequest(Player source) : base(source, RequestType.Join)
        {
        }
    }
}