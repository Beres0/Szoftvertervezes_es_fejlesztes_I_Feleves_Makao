using Makao.Collections;
using System;

namespace Makao.View
{
    public class Selector<T> : Controller
    {
        private int currentIndex;
        protected Func<int> max;
        protected Func<int> min;
        protected Func<int, T> value;

        public int CurrentIndex
        {
            get => currentIndex;
            set
            {
                if (min() <= max())
                {
                    value = Math.Clamp(value, min(), max());
                    if (value != currentIndex)
                    {
                        currentIndex = value;
                        SelectionChanged?.Invoke(this, EventArgs.Empty);
                    }
                }
            }
        }

        public virtual T CurrentValue => value(CurrentIndex);
        public int Max => max();
        public int Min => min();

        public Selector(Func<int> min, Func<int> max, Func<int, T> value, string name = null, Func<bool> isActive = null) : base(name, isActive)
        {
            this.min = min;
            this.max = max;
            currentIndex = Min;
            this.value = value;
        }

        public Selector(IDynamicArray<T> collection, string name = null, Func<bool> isActive = null) : this
            (() => 0, () => collection.Count - 1, (i) => collection[i], name, isActive)
        {
        }

        public event EventHandler SelectionChanged;

        public override void ResetController()
        {
            CurrentIndex = Min;
        }

        public new Selector<T> SetCommand(ConsoleKey key, Action action, Func<bool> isEnabled = null)
        {
            base.SetCommand(key, action, isEnabled);
            return this;
        }

        public Selector<T> SetDecreaseCommand(ConsoleKey key)
        {
            SetCommand(key, () => CurrentIndex -= 1, () => CurrentIndex > Min);
            return this;
        }

        public Selector<T> SetIncreaseCommand(ConsoleKey key)
        {
            SetCommand(key, () => CurrentIndex += 1, () => CurrentIndex < Max);
            return this;
        }
    }
}