namespace Makao.Model
{
    public class DrawExtraCards : Punishment
    {
        public DrawExtraCards(int stack) : base(stack)
        {
        }

        protected override Punishment CreateNew(int stack)
        {
            return new DrawExtraCards(stack);
        }
    }
}