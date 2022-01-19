using System;

namespace Makao.Model
{
    [Flags]
    public enum RulesSettings
    {
        None = 0, CanThrowMoreCards = 1, AllowSpecialCards = 2, AllowPunishmentStacking = 4, GameEndsWithTheFirstWinner = 8, PlayersCanBeCalledBack = 16
    }
}