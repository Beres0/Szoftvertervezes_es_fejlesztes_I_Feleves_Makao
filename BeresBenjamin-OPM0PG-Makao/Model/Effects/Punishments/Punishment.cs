using System;

namespace Makao.Model
{
    public abstract class Punishment
    {
        public int Stack { get; }

        public Punishment(int stack)
        {
            if (stack < 1) throw new ArgumentException("Stack must be greater than zero!", nameof(stack));
            Stack = stack;
        }

        protected abstract Punishment CreateNew(int stack);

        public Punishment Increase(int stack)
        {
            return CreateNew(Stack + stack);
        }
    }
}