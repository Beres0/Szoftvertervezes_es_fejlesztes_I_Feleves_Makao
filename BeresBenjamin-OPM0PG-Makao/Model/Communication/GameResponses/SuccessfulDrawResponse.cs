using Makao.Collections;

namespace Makao.Model
{
    public class SuccessfulDrawResponse : GameResponse
    {
        public ReadOnlyDynamicArray<Card> DrawedCards { get; }

        public SuccessfulDrawResponse(Game game, PlayerRequest request, ReadOnlyDynamicArray<Card> drawedCards)
            : base(game, request, Result.Success)
        {
            DrawedCards = drawedCards;
        }
    }
}