using Makao.Collections;
using System;

namespace Makao.View
{
    public class MultipleSelector<T> : Selector<T>
    {
        private DynamicArray<int> selection;
        public IDynamicArray<T> Collection { get; }
        public ReadOnlyDynamicArray<int> Indexes { get; }
        public ReadOnlyDynamicArray<int> Selection { get; }

        public MultipleSelector(IDynamicArray<T> collection, string name = null, Func<bool> isActive = null)
            : base(collection, name, isActive)
        {
            Collection = collection;
            selection = new DynamicArray<int>();
            Selection = selection.AsReadOnly();
        }

        public override void ResetController()
        {
            selection.Clear();
            CurrentIndex = 0;
        }

        public MultipleSelector<T> SetClearCommand(ConsoleKey key)
        {
            SetCommand(key, () => selection.Clear(), () => selection.Count > 0);
            return this;
        }

        public new MultipleSelector<T> SetCommand(ConsoleKey key, Action action, Func<bool> isEnabled = null)
        {
            base.SetCommand(key, action, isEnabled);
            return this;
        }

        public new MultipleSelector<T> SetDecreaseCommand(ConsoleKey key)
        {
            SetCommand(key, () => CurrentIndex -= 1, () => CurrentIndex > Min);
            return this;
        }

        public new MultipleSelector<T> SetIncreaseCommand(ConsoleKey key)
        {
            SetCommand(key, () => CurrentIndex += 1, () => CurrentIndex < Max);
            return this;
        }

        public MultipleSelector<T> SetToggleSelectCommand(ConsoleKey key)
        {
            SetCommand(key, ToggleSelect, () => !Collection.IsEmpty);
            return this;
            void ToggleSelect()
            {
                CurrentIndex = Math.Clamp(CurrentIndex, Min, Max);
                if (selection.Contains(CurrentIndex, out int index))
                {
                    selection.RemoveAt(index);
                }
                else
                {
                    selection.Add(CurrentIndex);
                }
            }
        }

        public MultipleSelector<T> SetUndoCommand(ConsoleKey key)
        {
            SetCommand(key, () => selection.TryPopLast(out int item), () => selection.Count > 0);
            return this;
        }
    }
}