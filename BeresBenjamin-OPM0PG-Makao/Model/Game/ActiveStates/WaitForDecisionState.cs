using Makao.Collections;

namespace Makao.Model
{
    public class WaitForDecisionState : ActiveState
    {
        public override RequestType AvaliableRequests
        {
            get
            {
                RequestType request = RequestType.Throw | RequestType.Leave;
                if (Game.MaxDrawableCardsCount == 0 || Game.Punishment is MissATurn)
                {
                    request |= RequestType.Pass;
                }
                else
                {
                    request |= RequestType.Draw;
                }
                return request;
            }
        }

        public WaitForDecisionState(Game context) : base(context)
        {
        }

        private GameResponse GetCantAskRankResponse(RankRequest request)
        {
            Result result = IsActivePlayer(request);
            if (result == Result.Success)
            {
                result = Result.CantAskRank;
            }

            return new GameResponse(Game, request, result);
        }

        private GameResponse GetCantAskSuitResponse(SuitRequest request)
        {
            Result result = IsActivePlayer(request);
            if (result == Result.Success)
            {
                result = Result.CantAskSuit;
            }

            return new GameResponse(Game, request, result);
        }

        public GameResponse GetDrawResponse(DrawRequest request)
        {
            Result result = IsActivePlayer(request);
            if (result == Result.Success)
            {
                if (Game.Punishment is MissATurn)
                {
                    return new GameResponse(Game, request, Result.MissATurn);
                }
                else if (TryDrawCards(Game.ActualDrawCount, out ReadOnlyDynamicArray<Card> cards))
                {
                    ActivePlayerDraw(cards);
                    return new SuccessfulDrawResponse(Game, request, cards);
                }
                else return new GameResponse(Game, request, Result.GameOutOfDrawableCards);
            }
            else return new GameResponse(Game, request, result);
        }

        public GameResponse GetPassResponse(PassRequest request)
        {
            Result result = IsActivePlayer(request);
            if (result == Result.Success)
            {
                if (Game.MaxDrawableCardsCount == 0 || Game.Punishment is MissATurn)
                {
                    return new GameResponse(Game, request, result);
                }
                else return new GameResponse(Game, request, Result.CantPass);
            }
            else return new GameResponse(Game, request, result);
        }

        public GameResponse GetThrowResponse(ThrowRequest request)
        {
            Result result = IsActivePlayer(request);
            if (result == Result.Success)
            {
                result = Game.ThrowValidator.IsValidThrow(request.Source.Hand, request.Selection);
                if (result == Result.Success)
                {
                    int beforeCount = request.Source.Hand.Count;
                    ActivePlayerThrow(request, out ReadOnlyDynamicArray<Card> throwedCards);
                    GiveCardsIfForgotToSayKeyword(beforeCount, request,
                        out ReadOnlyDynamicArray<Keyword> keywords,
                        out ReadOnlyDynamicArray<Card> punishmentDraw);
                    GiveCardsIfHasThrowingExchange(throwedCards.Count, out ReadOnlyDynamicArray<Card> throwExchange);

                    return new SuccessfulThrowResponse(Game, request, throwedCards, punishmentDraw, keywords, throwExchange);
                }
            }
            return new GameResponse(Game, request, result);
        }

        public override void Recieve(PlayerRequest request)
        {
            base.Recieve(request);

            GameResponse response = null;
            if (request is ThrowRequest thrw)
            {
                response = GetThrowResponse(thrw);
                SendBack(response);
            }
            else if (request is DrawRequest draw)
            {
                response = GetDrawResponse(draw);
                SendBack(response);
            }
            else if (request is PassRequest pass)
            {
                response = GetPassResponse(pass);
                SendBack(response);
            }
            else if (request is RankRequest askRank)
            {
                SendBack(GetCantAskRankResponse(askRank));
            }
            else if (request is SuitRequest askSuit)
            {
                SendBack(GetCantAskSuitResponse(askSuit));
            }
            else if (request is LeaveRequest leave)
            {
                if (leave.Source == Game.ActivePlayer)
                {
                    response = GetLeaveResponse(leave);
                    SendBack(response);
                }
            }

            if (response != null && response.Result == Result.Success)
            {
                NextState(response);
            }
        }
    }
}