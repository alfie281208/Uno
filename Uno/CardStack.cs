// TODO : Fix bug where only 3 wild cards are generated

using System.Drawing;

namespace Uno
{
    internal enum CardColour
    {
        None, Blue, Red, Green, Yellow
    }

    internal enum CardType
    {
        None, Zero, One, Two, Three, Four, Five, Six, Seven, Eight, Nine,
        Skip, Reverse, DrawTwo, DrawFour, Wild
    }

    internal readonly struct Card
    {
        public CardColour Colour { get; }
        public CardType Type { get; }

        public Card(CardColour colour, CardType type)
        {
            Colour = colour;
            Type = type;
        }

        public override string ToString() => $"{(int)Colour},{(int)Type}";

        public static Card ToCard(string sCard)
        {
            string[] sCardData = sCard.Split(",");
            return new Card((CardColour)Convert.ToInt32(sCardData[0]), (CardType)Convert.ToInt32(sCardData[1]));
        }
    }

    internal class CardStack
    {
        private readonly Card[] _cards = new Card[108];
        private int _top = -1;

        public void FillWithCards()
        {
            // Fill in Blue-Yellow Zero/One-DrawTwo
            for (int colour = 1; colour < 5; colour++)
            {
                for (int type = 1; type < 14; type++)
                    Push(new Card((CardColour)colour, (CardType)type));

                for (int type = 2; type < 14; type++)
                    Push(new Card((CardColour)colour, (CardType)type));
            }

            // Fill in DrawFour and Wild
            for (int i = 0; i < 4; i++)
            {
                Push(new Card(CardColour.None, CardType.DrawFour));
                Push(new Card(CardColour.None, CardType.Wild));
            }
        }

        public void FillWithNone()
        {
            for (int i = 0; i < _cards.Length; i++)
                _cards[i] = new Card(CardColour.None, CardType.None);
        }

        public void Shuffle()
        {
            // Fischer-Yates shuffle
            Random rand = new Random();

            for (int i = 0; i < (_cards.Length - 1); i++)
            {
                int rn = i + rand.Next(_cards.Length - i);

                Card card = _cards[i];
                _cards[rn] = _cards[i];
                _cards[i] = card;
            }
        }

        public void Push(Card card) => _cards[++_top] = card;

        public Card[] Remove(int amount)
        {
            Card[] cards = _cards[(_top - amount).._top];
            Array.Clear(cards, _top - amount, _top);
            _top -= amount;

            return cards;
        }

        public Card GetTop() => _cards[_top];
    }
}
