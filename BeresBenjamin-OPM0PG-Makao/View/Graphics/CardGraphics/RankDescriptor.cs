using System;

namespace Makao.View
{
    public class RankDescriptor : VisualDescriptor
    {
        public static readonly RankDescriptor Error = new RankDescriptor('#', "ERROR");
        public string CustomText { get; }

        public RankDescriptor(char mark, string name) : base(mark, name)
        {
        }

        public RankDescriptor(string customText, string name) : base(' ', name)
        {
            if (customText.Length != 2 || customText is null)
            {
                throw new ArgumentException("Length of custom text must be equal to 2!", nameof(customText));
            }
            CustomText = customText;
        }
    }
}