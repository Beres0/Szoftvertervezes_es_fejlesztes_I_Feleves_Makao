using System;

namespace Makao.View
{
    public interface IController
    {
        string ControlKeys { get; }

        bool Control(ConsoleKeyInfo key);
    }
}