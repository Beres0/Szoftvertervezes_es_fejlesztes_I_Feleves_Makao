using Makao.Collections;
using System;

namespace Makao.View
{
    public class Controller : IController, IWriter
    {
        private readonly Func<bool> isActive;
        protected AssociativeArray<ConsoleKey, SimpleCommand> commands;
        public ReadOnlyAssociativeArray<ConsoleKey, SimpleCommand> Commands { get; }

        public virtual string ControlKeys
        {
            get
            {
                if (IsActive)
                {
                    string keys = "";
                    string separator = ", ";

                    AssociativeArray<ConsoleKey, SimpleCommand> enabled = new AssociativeArray<ConsoleKey, SimpleCommand>();
                    for (int i = 0; i < commands.Count; i++)
                    {
                        if (commands[i].Value.IsEnabled())
                        {
                            enabled.Add(commands[i]);
                        }
                    }
                    if (!enabled.IsEmpty)
                    {
                        keys = enabled.Join((c) => c.Key.ToString(), separator);
                    }

                    if (CustomCommand != null && CustomCommand.IsEnabled())
                    {
                        if (!enabled.IsEmpty)
                        {
                            keys += separator + CustomCommand.ControlKeys;
                        }
                        else
                        {
                            return CustomCommand.ControlKeys;
                        }
                    }
                    return keys;
                }
                else return "Not Active";
            }
        }

        public CustomCommand CustomCommand { get; set; }
        public bool IsActive => isActive();
        public string Name { get; }
        public Action WriteMethod { get; set; }

        public Controller(string name = null, Func<bool> isActive = null)
        {
            Name = name;
            commands = new AssociativeArray<ConsoleKey, SimpleCommand>();
            Commands = commands.AsReadOnly();
            if (isActive == null)
            {
                this.isActive = () => true;
            }
            else this.isActive = isActive;
        }

        public Controller ClearCommands()
        {
            commands.Clear();
            return this;
        }

        public virtual bool Control(ConsoleKeyInfo key)
        {
            if (IsActive)
            {
                if (commands.TryGetValue(key.Key, out SimpleCommand command) && command.IsEnabled())
                {
                    command.Invoke();
                    return true;
                }
                else if (CustomCommand != null)
                {
                    return CustomCommand.Control(key);
                }
            }
            return false;
        }

        public Controller RemoveCommand(ConsoleKey key)
        {
            commands.Remove(key);
            return this;
        }

        public virtual void ResetController()
        { }

        public Controller SetCommand(ConsoleKey key, Action action, Func<bool> isEnabled = null)
        {
            commands.SetValue(key, new SimpleCommand(action, isEnabled));
            return this;
        }

        public void Write()
        {
            if (IsActive)
            {
                WriteMethod?.Invoke();
            }
        }
    }
}