using System.Numerics;
using Raylib_cs;

namespace Uno
{
    internal class Game(string username, string ip, int port)
    {
        private const int _WINDOW_WIDTH = 1366;
        private const int _WINDOW_HEIGHT = 768;

        private Client _client = new Client(username, ip, port);

        public void Play()
        {
            // Connect to the server
            _client.Connect();

            // Create the window
            Raylib.InitWindow(_WINDOW_WIDTH, _WINDOW_HEIGHT, "Uno");

            _client.DrawCards(7);

            // Game loop
            while (!Raylib.WindowShouldClose())
            {
                // Draw the game
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.Beige);
                DrawPlacedReserved();
                Raylib.EndDrawing();
            }

            Raylib.CloseWindow();
        }

        private void DrawCard(Card card, int x, int y)
        {
            // Convert CardColour to raylib's Color
            Color colour = card.Colour switch
            {
                CardColour.None => Color.Black,
                CardColour.Blue => Color.Blue,
                CardColour.Red => Color.Red,
                CardColour.Green => Color.Green,
                CardColour.Yellow => Color.Orange,
            };

            // Card background and border
            Raylib.DrawRectangle(x, (y * 104) + 24, 64, 96, Color.White);
            Raylib.DrawRectangle(x + 4, (y * 104) + 28, 64 - 8, 96 - 8, colour);

            // Convert CardType to a string for a card label
            string label = card.Type switch
            {
                CardType.Skip => "X",
                CardType.Reverse => "R",
                CardType.DrawTwo => "+2",
                CardType.DrawFour => "+4",
                CardType.Wild => "W",
                _ => ((int)card.Type - 1).ToString()
            };

            // Card label
            Vector2 labelDim = Raylib.MeasureTextEx(Raylib.GetFontDefault(), label, 36, (float)3.6);
            Raylib.DrawText(label, (x + 4) + ((56 / 2) - ((int)labelDim.X / 2)), ((y * 104) + 28) + ((88 / 2) - ((int)labelDim.Y / 2)), 36, Color.White);
        }

        private void DrawPlacedReserved() => DrawCard(_client.GetPlacedTop(), 1, 1);
    }
}
