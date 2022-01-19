namespace Makao.Model
{
    public class RankRequestConditionFailResponse : GameResponse
    {
        public RankRequestCondition Condition { get; }

        public RankRequestConditionFailResponse(Game game, RankRequest request, RankRequestCondition condition)
            : base(game, request, Result.RankRequestConditionFail)
        {
            Condition = condition;
        }
    }
}