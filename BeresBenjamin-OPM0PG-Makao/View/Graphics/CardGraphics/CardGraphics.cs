namespace Makao.View
{
    public class CardGraphics
    {
        public static readonly CardGraphics Error = new CardGraphics(SuitDescriptor.Error, RankDescriptor.Error);

        public string Name => Suit.Name + "," + Rank.Name;
        public RankDescriptor Rank { get; }
        public SuitDescriptor Suit { get; }
        public string Text => Rank.CustomText != null ? Rank.CustomText : $"{Rank.Mark}{Suit.Mark}";

        public CardGraphics(SuitDescriptor suit, RankDescriptor rank)
        {
            Suit = suit;
            Rank = rank;
        }
    }
}