namespace Makao.Model
{
    public class PostGameState : Game.GameState
    {
        public override RequestType AvaliableRequests => RequestType.Leave;

        public PostGameState(Game context) : base(context)
        {
        }

        private Result GameHasAlreadyEnded(PlayerRequest request)
        {
            Result result = MemberOfTheGame(request);
            if (result == Result.Success) return Result.GameHasAlreadyEnded;
            else return result;
        }

        public override void Recieve(PlayerRequest request)
        {
            if (request is LeaveRequest leaveReq)
            {
                Send(request.Source, GetLeaveResponse(leaveReq));
            }
            else
            {
                Send(request.Source, new GameResponse(Game, request, GameHasAlreadyEnded(request)));
            }
        }
    }
}