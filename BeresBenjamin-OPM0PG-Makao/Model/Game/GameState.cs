using Makao.Collections;
using System;

namespace Makao.Model
{
    public partial class Game
    {
        public abstract class GameState
        {
            protected GameDirection Direction { get => Game.Direction; set => Game.Direction = value; }
            protected ReadOnlyDynamicArray<Card> DiscardPile => Game.DiscardPile;
            protected ReadOnlyDynamicArray<Card> DrawPile => Game.DrawPile;
            protected Rules Rules => Game.Rules;
            public abstract RequestType AvaliableRequests { get; }
            public Game Game { get; }

            public GameState(Game context)
            {
                Game = context;
            }

            protected void ActivePlayerDraw(ReadOnlyDynamicArray<Card> cards)
            {
                Game.ActivePlayerDraw(cards);
            }

            protected void ActivePlayerThrow(ThrowRequest request, out ReadOnlyDynamicArray<Card> throwedCards)
            {
                Game.ActivePlayerThrow(request, out throwedCards);
            }

            protected void AddPlayer(Player player, out Func<ReadOnlyDynamicArray<Card>> hand)
            {
                Game.AddPlayer(player, out hand);
            }

            protected void AddPlayerToWinners(int index)
            {
                Game.AddPlayerToWinners(index);
            }

            protected virtual GameResponse GetLeaveResponse(LeaveRequest request)
            {
                Result result = Result.NotMemberOfTheGame;
                if (Game.ContainsPlayer(request.Source, out int index))
                {
                    RemovePlayer(index);
                    result = Result.Success;
                }

                return new GameResponse(Game, request, result);
            }

            protected void GiveCardsIfForgotToSayKeyword(int countBeforeThrow, ThrowRequest request,
                                                          out ReadOnlyDynamicArray<Keyword> keywords,
                                                          out ReadOnlyDynamicArray<Card> penalties)
            {
                Game.GiveCardsIfForgotToSayKeyword(countBeforeThrow, request, out keywords, out penalties);
            }

            protected void GiveCardsIfHasThrowingExchange(int throwCount, out ReadOnlyDynamicArray<Card> exchange)
            {
                Game.GiveCardsIfHasThrowingExchange(throwCount, out exchange);
            }

            protected Result MemberOfTheGame(PlayerRequest request)
            {
                if (!Game.ContainsPlayer(request.Source, out int index))
                {
                    return Result.NotMemberOfTheGame;
                }
                else return Result.Success;
            }

            protected void NextState(GameResponse lastResponse)
            {
                Game.NextState(lastResponse);
            }

            protected void RemovePlayer(int index)
            {
                Game.RemovePlayer(index);
            }

            protected void Send(Player player, GameResponse message)
            {
                Game.Send(player, message);
            }

            protected void SendBack(GameResponse response)
            {
                Game.SendBack(response);
            }

            protected bool TryDrawCards(int count, out ReadOnlyDynamicArray<Card> cards)
            {
                return Game.TryDrawCards(count, out cards);
            }

            public abstract void Recieve(PlayerRequest request);
        }
    }
}