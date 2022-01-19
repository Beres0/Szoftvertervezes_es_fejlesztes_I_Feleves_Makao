using Makao.Collections;
using System;

namespace Makao.Model
{
    public class SuccessfulThrowResponse : GameResponse
    {
        public ReadOnlyDynamicArray<Card> KeywordPunishment { get; }
        public ReadOnlyDynamicArray<Keyword> Keywords { get; }
        public ReadOnlyDynamicArray<Card> ThrowedCards { get; }
        public ReadOnlyDynamicArray<Card> ThrowExchange { get; }

        public SuccessfulThrowResponse(Game game, ThrowRequest request,
                             ReadOnlyDynamicArray<Card> throwedCards,
                             ReadOnlyDynamicArray<Card> keywordPunishment,
                             ReadOnlyDynamicArray<Keyword> keywords,
                             ReadOnlyDynamicArray<Card> throwExchange) : base(game, request, Result.Success)
        {
            ThrowedCards = throwedCards ?? throw new ArgumentNullException(nameof(throwedCards));
            ThrowExchange = throwExchange;
            KeywordPunishment = keywordPunishment;
            Keywords = keywords;
        }
    }
}