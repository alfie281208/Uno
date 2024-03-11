// TODO : Fix bug where only 3 wild cards are generated

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

    internal readonly struct Card(CardColour colour, CardType type)
    {
        public CardColour Colour { get; } = colour;
        public CardType Type { get; } = type;

        public override string ToString() => $"{(int)Colour},{(int)Type}";

        public static Card ToCard(string sCard)
        {
            string[] sCardData = sCard.Split(",");
            return new Card((CardColour)Convert.ToInt32(sCardData[0]), (CardType)Convert.ToInt32(sCardData[1]));
        }
    }

    internal class CardStack
    {
        public const int CARD_COUNT = 108;
        private readonly Card[] _cards = new Card[CARD_COUNT];
        private int _top = 0;

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
            for (int i = 0; i < CARD_COUNT; i++)
                _cards[i] = new Card(CardColour.None, CardType.None);
        }

        public void Shuffle()
        {
            // Fischer-Yates shuffle
            Random rand = new Random();

            for (int i = CARD_COUNT - 1; i > 0; i--)
            {
                int rn = rand.Next(i + 1);
                Card card = _cards[i];
                _cards[rn] = _cards[i];
                _cards[i] = card;
            }
        }

        public void Push(Card card) => _cards[_top++] = card;

        public Card[] Remove(int amount)
        {
            Console.WriteLine(_cards[100].ToString());

            Card[] cards = _cards[((_top - 1) - amount)..(_top - 1)];
            //Array.Clear(_cards, (_top - 1) - amount, (_top - 1));
            _top -= amount;

            return cards;
        }

        public Card GetTop() => _cards[_top - 1];

        public void Print()
        {
            Console.WriteLine("Cards!");

            foreach (Card card in _cards)
                Console.Write($"[{card.Colour} : {card.Type}] ");
            
            Console.WriteLine();
        }

        private void Error(string msg)
        {
            Console.Write($"CardStack Error : {msg}");
            Environment.Exit(1);
        }
    }
}
