using CultLib;
using SFML.Window;

namespace ld25
{
    class TutorialManager : Entity
    {
        private Entity _tut1;
        private Entity _tut2;
        public TutorialManager(World parent, bool add = true) : base(null, new Vector2f(0, 0), parent, add)
        {
            _tut1 = new Entity(Rsc.Tex("rsc/tutorial/ClickOneToUseIt.png"), new Vector2f(600, 200), Ld25.UserInterface)
                        {Scale = new Vector2f(0.5f, 0.5f), Rotation = 20, Layer = -5};
        }
        public override void Update()
        {
            base.Update();
            if (_tut2 != null)
            {
                for (var i = 0; i < Ld25.Game.Entities.Count; i++)
                {
                    var a = Ld25.Game.Entities[i];
                    
                    if (a.GetType() == typeof (Asteroid))
                    {
                        _tut2.Destroy();
                    }
                }
            }
            if (SpawnButton.Selected == null || _tut1 == null) return;
            _tut1.Destroy();
            _tut1 = null;
            _tut2 = new Entity(Rsc.Tex("rsc/tutorial/ClickAndDragToAimTheAsteroids.png"), new Vector2f(100, 20),
                               Ld25.UserInterface) {Scale = new Vector2f(0.5f, 0.5f), Rotation = 10, Layer = -5};
        }
    }
}
