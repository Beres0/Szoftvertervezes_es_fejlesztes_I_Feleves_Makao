using Makao.Model;
using System;

namespace Makao.View
{
    public class DeckGraphicsBuilder
    {
        private CardGraphics faceDown;

        protected static readonly CardGraphics DefaultFacedown = new CardGraphics
                  (new SuitDescriptor('░', "Facedown", new ConsoleColorPair(ConsoleColor.Yellow, ConsoleColor.DarkGray)),
           new RankDescriptor('░', "Facedown"));

        public Deck Deck { get; }

        public CardGraphics Facedown
        {
            get
            {
                if (faceDown == null)
                {
                    return DefaultFacedown;
                }
                else return faceDown;
            }
            set
            {
                faceDown = value;
            }
        }

        public RankDescriptor[] Ranks { get; }
        public CardGraphics[] SpecialCards { get; }
        public SuitDescriptor[] Suits { get; }

        public DeckGraphicsBuilder(Deck deck)
        {
            Deck = deck;
            Ranks = new RankDescriptor[deck.NumberOfRanks];
            Suits = new SuitDescriptor[deck.NumberOfSuits];
            SpecialCards = new CardGraphics[deck.NumberOfSpecialRanks];
        }

        public DeckGraphics ToDeckGraphics()
        {
            return new DeckGraphics(this);
        }
    }
}