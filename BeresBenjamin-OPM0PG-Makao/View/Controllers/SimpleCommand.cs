using System;

namespace Makao.View
{
    public class SimpleCommand
    {
        private readonly Action action;
        private readonly Func<bool> isEnabled;

        public SimpleCommand(Action action, Func<bool> isEnabled = null)
        {
            if (isEnabled == null)
            {
                this.isEnabled = () => true;
            }
            else this.isEnabled = isEnabled;
            this.action = action ?? throw new ArgumentNullException(nameof(action));
        }

        public void Invoke()
        {
            action.Invoke();
        }

        public bool IsEnabled()
        {
            return isEnabled();
        }
    }
}