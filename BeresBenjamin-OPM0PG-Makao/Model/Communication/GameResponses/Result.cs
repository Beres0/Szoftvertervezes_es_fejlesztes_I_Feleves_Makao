namespace Makao.Model
{
    public enum Result
    {
        Success,

        NotMemberOfTheGame,
        NotActivePlayer,
        GameHasAlreadyStarted,
        GameHasNotStartedYet,
        GameHasAlreadyEnded,
        GameIsFull,
        SuitDoesNotExists,
        CantAskSuit,
        NotSuitRequest,

        PlayerIsAlreadyInGame,

        RankDoesNotExists,
        RankRequestConditionFail,
        CantAskRank,
        NotRankRequest,

        CardDoesNotExists,
        NotAllCardsAreTheSameRank,
        CantThrowMoreCardsAtOnce,
        SuitIsNotFitForModifier,
        RankIsNotFitForModifier,
        RankAndSuitAreNotFitForModifiers,
        ThrowSelectionIsEmpty,
        NotCounter,
        CantThrowAllCardsAtOnce,
        NotSameRankOrSuitOrSpecial,
        DuplicationInSelection,

        MissATurn,
        GameOutOfDrawableCards,
        CantPass,
    }
}