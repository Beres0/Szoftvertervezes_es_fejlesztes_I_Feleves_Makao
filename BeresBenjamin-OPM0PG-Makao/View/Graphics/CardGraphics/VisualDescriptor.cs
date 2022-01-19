namespace Makao.View
{
    public abstract class VisualDescriptor
    {
        public char Mark { get; }
        public string Name { get; }

        public VisualDescriptor(char mark, string name)
        {
            Mark = mark;
            Name = name;
        }
    }
}