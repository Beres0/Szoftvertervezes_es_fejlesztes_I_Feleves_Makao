using Makao.Collections;
using System;

namespace Makao.View
{
    public class ControllerCollection : Controller, IDynamicArray<Controller>
    {
        private DynamicArray<Controller> controllers;
        private int currentIndex;
        public Application Application { get; }

        public override string ControlKeys
        {
            get
            {
                string baseKeys = base.ControlKeys;
                if (CurrentController != null)
                {
                    baseKeys += ", " + CurrentController.ControlKeys;
                }
                return baseKeys;
            }
        }

        public int Count => controllers.Count;

        public Controller CurrentController
        {
            get
            {
                if (!controllers.IsEmpty)
                {
                    CurrentIndex = currentIndex;
                    return controllers[CurrentIndex];
                }
                else return null;
            }
        }

        public int CurrentIndex
        {
            get
            {
                return currentIndex;
            }
            set
            {
                currentIndex = Math.Clamp(value, 0, controllers.Count - 1);
            }
        }

        public bool IsEmpty => controllers.IsEmpty;
        public string Separator { get; set; }
        public Action<Controller, Func<string>> WriteController { get; set; }
        public Controller this[int index] => controllers[index];

        public ControllerCollection(Application application, string name = null) : base(name, null)
        {
            Application = application;
            controllers = new DynamicArray<Controller>();
            WriteMethod = WriteItems;

            WriteController = WriteControllerDefault;
            Separator = "\n";
        }

        private Selector<T> SetSelector<T>(Selector<T> selector, Func<T, string> itemToString)
        {
            selector.SetDecreaseCommand(Application.Settings.LeftKey).SetIncreaseCommand(Application.Settings.RightKey);
            selector.WriteMethod = () => WriteSelector(selector, itemToString);
            controllers.Add(selector);

            return selector;
        }

        private void WriteControllerDefault(Controller controller, Func<string> text = null)
        {
            string textStr = text != null ? text() : controller.Name;

            if (CurrentController == controller)
            {
                ConsoleHelper.WriteColorizedText(textStr, Application.Settings.CurrentColor);
            }
            else
            {
                Console.Write(textStr);
            }
            Console.Write(Separator);
        }

        private void WriteItems()
        {
            for (int i = 0; i < controllers.Count; i++)
            {
                controllers[i].Write();
            }
        }

        public Controller Add(Controller controller)
        {
            if (controller is null)
            {
                throw new ArgumentNullException(nameof(controller));
            }

            controllers.Add(controller);
            return controller;
        }

        public Controller AddController(string name = null, Func<bool> isActive = null, Func<string> toString = null)
        {
            Controller controller = new Controller(name, isActive);
            controller.WriteMethod = () => WriteController(controller, toString);
            controllers.Add(controller);
            return controller;
        }

        public Controller AddExit(string name = null, Func<bool> isActive = null)
        {
            Controller exit = new Controller(name, isActive).SetCommand(Application.Settings.EnterKey, Application.Exit);
            exit.WriteMethod = () => WriteController(exit, () => exit.Name);
            controllers.Add(exit);

            return exit;
        }

        public Controller AddNavigator(Func<Page> page, string name = null, Func<bool> isActive = null, Func<string> toString = null)
        {
            Controller navigator = new Controller(name, isActive).SetCommand(Application.Settings.EnterKey, () => Application.CurrentPage = page());
            navigator.WriteMethod = () => WriteController(navigator, toString);
            controllers.Add(navigator);

            return navigator;
        }

        public Selector<T> AddSelector<T>
            (Func<int> min, Func<int> max, Func<int, T> value, string name = null, Func<bool> isActive = null, Func<T, string> itemToString = null)
        {
            return SetSelector(new Selector<T>(min, max, value, name, isActive), itemToString);
        }

        public Selector<T> AddSelector<T>(IDynamicArray<T> collection, string name = null, Func<bool> isActive = null, Func<T, string> itemToString = null)
        {
            return SetSelector(new Selector<T>(collection, name, isActive), itemToString);
        }

        public TextBox AddTextBox(int maxLength, string defaultText = null, string name = null, Func<bool> isActive = null)
        {
            TextBox textBox = new TextBox(maxLength, defaultText, name, isActive)
                             .SetUndoCommand(Application.Settings.UndoKey)
                             .SetClearCommand(Application.Settings.DeleteKey);

            textBox.WriteMethod = () => WriteController(textBox, () => $"{textBox.Name}: {textBox.Text}");
            controllers.Add(textBox);

            return textBox;
        }

        public bool Contains(Controller item, out int index)
        {
            return controllers.Contains(item, out index);
        }

        public override bool Control(ConsoleKeyInfo key)
        {
            if (CurrentController != null && CurrentController.Control(key))
            {
                ResetController();
                return true;
            }
            if (base.Control(key))
            {
                return true;
            }
            else return false;
        }

        public DynamicArray<Controller> CopyAsDynamicArray()
        {
            return controllers.CopyAsDynamicArray();
        }

        public bool Remove(Controller controller)
        {
            if (Contains(controller, out int index))
            {
                if (index <= CurrentIndex)
                {
                    CurrentIndex--;
                }
                controllers.RemoveAt(index);
                return true;
            }
            else return false;
        }

        public override void ResetController()
        {
            if (!CurrentController.IsActive)
            {
                int i = CurrentIndex - 1;
                while (i >= 0 && !controllers[i].IsActive)
                {
                    i--;
                }
                if (i >= 0)
                {
                    CurrentIndex = i;
                }
                else
                {
                    i = CurrentIndex + 1;
                    while (i < controllers.Count && !controllers[i].IsActive)
                    {
                        i++;
                    }

                    if (i < controllers.Count)
                    {
                        CurrentIndex = i;
                    }
                }
            }
        }

        public new ControllerCollection SetCommand(ConsoleKey key, Action action, Func<bool> isEnabled = null)
        {
            base.SetCommand(key, action, isEnabled);
            return this;
        }

        public ControllerCollection SetNextControllerCommand(ConsoleKey key)
        {
            SetCommand(key, Increase, () => CurrentIndex < controllers.Count - 1);
            return this;

            void Increase()
            {
                int index = CurrentIndex + 1;
                while (index < controllers.Count && !controllers[index].IsActive)
                {
                    index++;
                }
                if (index < controllers.Count)
                {
                    CurrentIndex = index;
                }
            }
        }

        public ControllerCollection SetPrevControllerCommand(ConsoleKey key)
        {
            SetCommand(key, Decrease, () => CurrentIndex > 0);
            return this;
            void Decrease()
            {
                int index = CurrentIndex - 1;
                while (index >= 0 && !controllers[index].IsActive)
                {
                    index--;
                }
                if (index >= 0)
                {
                    CurrentIndex = index;
                }
            }
        }

        public void WriteSelector<T>(Selector<T> selector, Func<T, string> toString = null)
        {
            string text = "";
            if (toString != null)
            {
                text += toString(selector.CurrentValue);
            }
            else
            {
                text += selector.CurrentValue.ToString();
            }

            if (selector.CurrentIndex > selector.Min)
            {
                text = "<< " + text;
            }
            if (selector.CurrentIndex < selector.Max)
            {
                text += " >>";
            }

            WriteController(selector, () => selector.Name + ": " + text);
        }
    }
}