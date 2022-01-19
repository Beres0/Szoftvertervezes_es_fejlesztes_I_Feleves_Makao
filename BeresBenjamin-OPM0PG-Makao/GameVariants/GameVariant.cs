using Makao.Model;
using Makao.View;
using System;

namespace Makao.GameVariants
{
    public class GameVariant
    {
        public DeckGraphics Graphics { get; }
        public string Name { get; }
        public Rules Rules { get; }

        public GameVariant(string name, Rules rules, DeckGraphics graphics)
        {
            if (rules.Deck != graphics.Deck)
            {
                throw new ArgumentException("Decks are not same!", $"{nameof(rules)},{nameof(graphics)}");
            }

            Name = name;
            Rules = rules;
            Graphics = graphics;
        }

        public GameVariant(string name, RulesBuilder rules, DeckGraphicsBuilder graphics) : this(name, rules.ToRules(), graphics.ToDeckGraphics())
        {
        }
    }
}