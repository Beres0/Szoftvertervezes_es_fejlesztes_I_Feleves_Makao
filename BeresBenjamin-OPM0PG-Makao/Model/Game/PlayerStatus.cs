namespace Makao.Model
{
    public class PlayerStatus
    {
        public enum Type
        {
            Waiting, Playing, Winner, Leaver
        }

        public Hand Hand { get; set; }

        public Player Player { get; set; }

        public Type Status { get; set; }

        public PlayerStatus(Hand hand, Player player, Type status)
        {
            Hand = hand;
            Player = player;
            Status = status;
        }
    }
}