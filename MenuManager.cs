using CultLib;
using SFML.Window;

namespace ld25
{
    class MenuManager : Entity 
    {
        public MenuManager(World parent, bool add = true) : base(null, new Vector2f(0, 0), parent, add)
        {
        }
        public override void Update()
        {
            base.Update();
            if (!Keyboard.IsKeyPressed(Keyboard.Key.Return)) return;
            Parent.Parent.CurrentWorld = Ld25.Game;
            Parent.Parent.UserInterface = Ld25.UserInterface;
            new LevelManager(Ld25.Game);
        }
    }
}
