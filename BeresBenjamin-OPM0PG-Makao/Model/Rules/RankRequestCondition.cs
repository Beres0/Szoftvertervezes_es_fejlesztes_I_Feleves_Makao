namespace Makao.Model
{
    public class RankRequestCondition
    {
        public enum RelationType
        {
            Equal, Greater, Lower
        }

        public int HandCount { get; }
        public RelationType Relation { get; }

        public RankRequestCondition(RelationType condition, int handCount)
        {
            HandCount = handCount;
            Relation = condition;
        }

        public bool IsTrue(int playersHandCount)
        {
            switch (Relation)
            {
                case RelationType.Equal: return HandCount == playersHandCount;
                case RelationType.Greater: return HandCount < playersHandCount;
                case RelationType.Lower: return HandCount > playersHandCount;
                default: return false;
            }
        }
    }
}