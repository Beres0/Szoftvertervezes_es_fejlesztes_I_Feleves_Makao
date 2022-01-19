using System;
using System.Text;

namespace Makao.View
{
    public class TextBox : Controller
    {
        private StringBuilder sb;
        public string DefaultText { get; }
        public int MaxLength { get; }
        public string Text => sb.ToString();

        public TextBox(int maxLength, string defaultText = null, string name = null, Func<bool> isActive = null) : base(name, isActive)
        {
            DefaultText = defaultText;
            if (DefaultText != null)
            {
                sb = new StringBuilder(defaultText);
            }
            else
            {
                sb = new StringBuilder();
            }

            MaxLength = maxLength;

            CustomCommand = new CustomCommand(Input, "Szövegbevitel", () => sb.Length < maxLength);
        }

        public event EventHandler TextBoxChanged;

        private void RaiseTextBoxChanged()
        {
            TextBoxChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Clear()
        {
            sb.Clear();
            RaiseTextBoxChanged();
        }

        public bool Input(ConsoleKeyInfo key)
        {
            if (key.Key != ConsoleKey.Enter && (char.IsLetterOrDigit(key.KeyChar) || char.IsPunctuation(key.KeyChar) || char.IsWhiteSpace(key.KeyChar)))
            {
                sb.Append(key.KeyChar);
                RaiseTextBoxChanged();
                return true;
            }
            else return false;
        }

        public override void ResetController()
        {
            sb.Clear();
            if (DefaultText != null)
            {
                sb.Append(DefaultText);
            }
            RaiseTextBoxChanged();
        }

        public TextBox SetClearCommand(ConsoleKey key)
        {
            SetCommand(key, () => sb.Clear(), () => sb.Length > 0);
            return this;
        }

        public new TextBox SetCommand(ConsoleKey key, Action action, Func<bool> isEnabled = null)
        {
            base.SetCommand(key, action, isEnabled);
            return this;
        }

        public TextBox SetUndoCommand(ConsoleKey key)
        {
            SetCommand(key, () => sb.Remove(sb.Length - 1, 1), () => sb.Length > 0);
            return this;
        }

        public void Undo()
        {
            sb.Remove(sb.Length - 1, 1);
            RaiseTextBoxChanged();
        }
    }
}