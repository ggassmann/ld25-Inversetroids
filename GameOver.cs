using CultLib;
using SFML.Window;

namespace ld25
{
    class GameOver : Entity
    {
        public GameOver(World parent, bool add = true) : base(null, new Vector2f(0, 0), parent, add)
        {
        }
        public override void Update()
        {
            base.Update();
            if(Keyboard.IsKeyPressed(Keyboard.Key.Escape))
            {
                Ld25.InitGame(Parent.Parent);
                Ld25.InitMainMenu(Parent.Parent);
                Ld25.InitUI(Parent.Parent);
                Parent.Parent.CurrentWorld = Ld25.Menu;
            }

        }
    }
}
