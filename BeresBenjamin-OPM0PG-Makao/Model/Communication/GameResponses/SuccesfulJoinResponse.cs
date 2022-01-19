using Makao.Collections;
using System;

namespace Makao.Model
{
    public class SuccesfulJoinResponse : GameResponse
    {
        public Func<ReadOnlyDynamicArray<Card>> Hand { get; }

        public SuccesfulJoinResponse(Game game, PlayerRequest request, Func<ReadOnlyDynamicArray<Card>> hand)
            : base(game, request, Result.Success)
        {
            Hand = hand;
        }
    }
}