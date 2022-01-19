namespace Makao.Model
{
    public class LeaveRequest : PlayerRequest
    {
        public LeaveRequest(Player source) : base(source, RequestType.Leave)
        {
        }
    }
}