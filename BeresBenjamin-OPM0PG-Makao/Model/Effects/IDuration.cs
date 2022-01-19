using System;

namespace Makao.Model
{
    public static class IDurationExtension
    {
        public static T Decrease<T>(this IDuration iDuration, Func<T> createMethod) where T : IDuration
        {
            if (iDuration.Duration > 1)
            {
                return createMethod();
            }
            else return default;
        }
    }

    public interface IDuration
    {
        int Duration { get; }

        IDuration Decrease();
    }
}