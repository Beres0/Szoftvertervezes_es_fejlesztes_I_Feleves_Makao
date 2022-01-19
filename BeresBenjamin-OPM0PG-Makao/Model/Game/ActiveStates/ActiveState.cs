namespace Makao.Model
{
    public abstract class ActiveState : Game.GameState
    {
        protected ActiveState(Game context) : base(context)
        {
        }

        protected override GameResponse GetLeaveResponse(LeaveRequest request)
        {
            GameResponse response = base.GetLeaveResponse(request);
            if (response.Result == Result.Success && Game.ActivePlayersCount == 1)
            {
                AddPlayerToWinners(0);
            }
            return response;
        }

        protected Result IsActivePlayer(PlayerRequest request)
        {
            Result result = MemberOfTheGame(request);
            if (request.Source != Game.ActivePlayer)
            {
                return Result.NotActivePlayer;
            }
            else return result;
        }

        public override void Recieve(PlayerRequest request)
        {
            if (request is JoinRequest joinReq)
            {
                Send(request.Source, new GameResponse(Game, request, Result.GameHasAlreadyStarted));
                return;
            }
        }
    }
}