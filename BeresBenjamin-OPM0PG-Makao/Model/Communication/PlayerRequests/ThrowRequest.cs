using Makao.Collections;
using System;

namespace Makao.Model
{
    public class ThrowRequest : PlayerRequest
    {
        public ReadOnlyDynamicArray<string> Keywords { get; }

        public ReadOnlyDynamicArray<int> Selection { get; }

        public ThrowRequest(Player source,
            IDynamicArray<int> selection,
            IDynamicArray<string> keywords
            ) : base(source, RequestType.Throw)
        {
            if (selection is null)
            {
                throw new ArgumentNullException(nameof(selection));
            }

            Selection = selection.CopyAsDynamicArray().AsReadOnly();
            Keywords = keywords.CopyAsDynamicArray().AsReadOnly();
        }

        public static ReadOnlyDynamicArray<string> FormatKeywords(string keywords)
        {
            if (!string.IsNullOrWhiteSpace(keywords))
            {
                string[] splitted = keywords.Split(',');

                DynamicArray<string> kwords = new DynamicArray<string>();

                for (int i = 0; i < splitted.Length; i++)
                {
                    kwords.Add(Keyword.FormatKeyword(splitted[i]));
                }
                return kwords.AsReadOnly();
            }
            else return ReadOnlyDynamicArray<string>.Empty;
        }

        public override string ToString()
        {
            return $"{base.ToString()}-{Selection.Join(",")}-{Keywords.Join(",")}";
        }
    }
}