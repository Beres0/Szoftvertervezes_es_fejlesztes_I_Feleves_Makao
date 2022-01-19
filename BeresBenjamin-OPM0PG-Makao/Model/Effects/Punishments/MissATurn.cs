namespace Makao.Model
{
    public class MissATurn : Punishment, IDuration
    {
        int IDuration.Duration => Stack;

        public MissATurn(int stack) : base(stack)
        {
        }

        protected override Punishment CreateNew(int stack)
        {
            return new MissATurn(stack);
        }

        public MissATurn Decrease()
        {
            return this.Decrease(() => new MissATurn(Stack - 1));
        }

        IDuration IDuration.Decrease()
        {
            return Decrease();
        }
    }
}