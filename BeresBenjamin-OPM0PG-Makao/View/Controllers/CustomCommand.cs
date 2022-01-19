using System;

namespace Makao.View
{
    public class CustomCommand : IController
    {
        private readonly Func<ConsoleKeyInfo, bool> control;
        private readonly string controlKeys;
        private readonly Func<bool> isEnabled;

        public string ControlKeys
        {
            get
            {
                if (isEnabled())
                {
                    return controlKeys;
                }
                else return string.Empty;
            }
        }

        public CustomCommand(Func<ConsoleKeyInfo, bool> control, string controlKeys, Func<bool> isEnabled = null)
        {
            if (string.IsNullOrEmpty(controlKeys))
            {
                throw new ArgumentException($"'{nameof(controlKeys)}' cannot be null or empty.", nameof(controlKeys));
            }
            this.control = control ?? throw new ArgumentNullException(nameof(control));
            this.controlKeys = controlKeys;
            if (isEnabled == null)
            {
                this.isEnabled = () => true;
            }
            else
            {
                this.isEnabled = isEnabled;
            }
        }

        public bool Control(ConsoleKeyInfo key)
        {
            if (isEnabled())
            {
                return control(key);
            }
            else return false;
        }

        public bool IsEnabled()
        {
            return isEnabled();
        }
    }
}