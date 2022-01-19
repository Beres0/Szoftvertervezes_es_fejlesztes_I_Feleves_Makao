namespace Makao.Model
{
    public class WaitForRankState : ActiveState
    {
        public override RequestType AvaliableRequests => RequestType.Leave | RequestType.Rank;

        public WaitForRankState(Game context) : base(context)
        {
        }

        private GameResponse GetNotRankRequestResponse(PlayerRequest request)
        {
            Result result = IsActivePlayer(request);
            if (result == Result.Success)
            {
                result = Result.NotRankRequest;
            }
            return new GameResponse(Game, request, result);
        }

        private GameResponse GetRankResponse(RankRequest request)
        {
            Result result = IsActivePlayer(request);
            if (result == Result.Success)
            {
                if (request.Rank >= Rules.Deck.NumberOfRanks || request.Rank < 0)
                {
                    result = Result.RankDoesNotExists;
                }
                else if (Rules.HasAskRankCondition(request.Rank, out RankRequestCondition condition) &&
                   !condition.IsTrue(request.Source.Hand.Count))
                {
                    return new RankRequestConditionFailResponse(Game, request, condition);
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
            else if (request is RankRequest askRankReq)
            {
                GameResponse response = GetRankResponse(askRankReq);
                SendBack(response);
                if (response.Result == Result.Success)
                {
                    NextState(response);
                }
            }
            else
            {
                SendBack(GetNotRankRequestResponse(request));
            }
        }
    }
}