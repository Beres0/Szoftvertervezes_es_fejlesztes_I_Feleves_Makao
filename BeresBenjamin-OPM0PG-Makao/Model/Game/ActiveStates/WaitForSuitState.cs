namespace Makao.Model
{
    public class WaitForSuitState : ActiveState
    {
        public override RequestType AvaliableRequests => RequestType.Leave | RequestType.Suit;

        public WaitForSuitState(Game context) : base(context)
        {
        }

        private GameResponse GetNotSuitRequestResponse(PlayerRequest request)
        {
            Result result = IsActivePlayer(request);
            if (result == Result.Success)
            {
                result = Result.NotSuitRequest;
            }

            return new GameResponse(Game, request, result);
        }

        private GameResponse GetSuitResponse(SuitRequest request)
        {
            Result result = IsActivePlayer(request);
            if (result == Result.Success)
            {
                if (request.Suit >= Rules.Deck.NumberOfSuits || request.Suit < 0)
                {
                    result = Result.SuitDoesNotExists;
                }
            }
            return new GameResponse(Game, request, result);
        }

        public override void Recieve(PlayerRequest request)
        {
            base.Recieve(request);

            if (request is LeaveRequest leaveReq)
            {
                GameResponse response = GetLeaveResponse(leaveReq);
                SendBack(response);
                if (response.Result == Result.Success
                   && request.Source == Game.ActivePlayer)
                {
                    NextState(response);
                }
            }
            else if (request is SuitRequest askSuitReq)
            {
                GameResponse response = GetSuitResponse(askSuitReq);
                SendBack(response);
                if (response.Result == Result.Success)
                {
                    NextState(response);
                }
            }
            else
            {
                SendBack(GetNotSuitRequestResponse(request));
            }
        }
    }
}