using System.IO;
using CultLib;
using SFML.Audio;
using SFML.Window;

namespace ld25
{
    class Ld25
    {
        public static World Game;
        public static World UserInterface;
        public static World Menu;
        public static World GameOver;
        static void Main()
        {
            var cult = new Cult("Inversetroids") {ClearColor = Colors.Black};

            Fonts.Init();

            InitGame(cult);
            InitUI(cult);
            InitMainMenu(cult);
            InitGameOver(cult);

            cult.Undrawn = new World(cult);

            var music = new Music("rsc/music/Beat1.wav");
            music.Play();

            new Ticker(cult.Undrawn, music.Duration, delegate { music.Stop(); music.Play(); });

            cult.CurrentWorld = Menu;
            cult.Run();
        }

        public static void InitGame(Cult cult)
        {
            Game = new World(cult);
            new Ship((Points.GameBottomLeft + Points.GameTopRight)/2, Game);
        }

        public static void InitUI(Cult cult)
        {
            UserInterface = new World(cult);
            new Entity(Rsc.Tex("rsc/UserInterface.png"), new Vector2f(0, 0), UserInterface);
            new Thrower(new Vector2f(10, 10), UserInterface);
            new SmallAsteroidSpawner(UserInterface);
            new MediumAsteroidSpawner(UserInterface);
            new LargeAsteroidSpawner(UserInterface);
            new GiantAsteroidSpawner(UserInterface);
            new UFOSpawner(UserInterface);
            new DangerManager(UserInterface);
            new Safety();
            new TutorialManager(UserInterface);
        }

        public static void InitMainMenu(Cult cult)
        {
            Menu = new World(cult);
            new MenuManager(Menu);
            new TextEntity("Inverseteroids", new Vector2f(220, 200), Menu, Fonts.ExoRegular, 60, Colors.White);
            new TextEntity("Press [Enter] to Play", new Vector2f(260, 260), Menu, Fonts.ExoRegular, 40, Colors.White);
            new TextEntity("Player 2", new Vector2f(564, 360), Menu, Fonts.ExoRegular, 18, Colors.White, Colors.Black);
            new TextEntity("Player 1", new Vector2f(200, 360), Menu, Fonts.ExoRegular, 18, Colors.White, Colors.Black);
            new Entity(Rsc.Tex("rsc/WADS.png"), new Vector2f(500, 400), Menu);
            new Entity(Rsc.Tex("rsc/Mouse.png"), new Vector2f(190, 400), Menu){Scale = new Vector2f(2.8f, 2.8f)};
            new Entity(Rsc.Tex("rsc/Goat.png"), new Vector2f(0, 0), Menu);
        }
        public static void InitGameOver(Cult cult)
        {
            GameOver = new World(cult);
            new GameOver(GameOver);
        }
    }
}
