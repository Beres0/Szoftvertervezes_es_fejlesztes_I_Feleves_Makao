using Makao.Collections;
using System;

namespace Makao.Model
{
    public class PreGameState : Game.GameState
    {
        public override RequestType AvaliableRequests => RequestType.Join;

        public PreGameState(Game context) : base(context)
        {
        }

        private GameResponse GetGameHasNotStartedYetResponse(PlayerRequest request)
        {
            Result result = MemberOfTheGame(request);
            if (result == Result.Success)
            {
                result = Result.GameHasNotStartedYet;
            }
            return new GameResponse(Game, request, result);
        }

        private GameResponse GetJoinResponse(JoinRequest request)
        {
            if (request.Source.IsInGame)
            {
                return new GameResponse(Game, request, Result.PlayerIsAlreadyInGame);
            }
            else if (Rules.MaxPlayer <= Game.ActivePlayersCount)
            {
                return new GameResponse(Game, request, Result.GameIsFull);
            }
            else
            {
                AddPlayer(request.Source, out Func<ReadOnlyDynamicArray<Card>> hand);
                return new SuccesfulJoinResponse(Game, request, hand);
            }
        }

        public override void Recieve(PlayerRequest request)
        {
            if (request is JoinRequest joinReq)
            {
                SendBack(GetJoinResponse(joinReq));
            }
            else if (request is LeaveRequest leaveReq)
            {
                SendBack(GetLeaveResponse(leaveReq));
            }
            else
            {
                SendBack(GetGameHasNotStartedYetResponse(request));
            }
        }
    }
}