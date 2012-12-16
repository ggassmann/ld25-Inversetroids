using System.Linq;
using CultLib;
using SFML.Window;

namespace ld25
{
    class DangerManager : Entity
    {
        public static DangerManager Instance
        {
            get;
            set;
        }

        private readonly TextEntity _dpDisp;
        public int MaxDP = 50;
        public int UsedDP
        {
            get
            {
                return Ld25.Game.Entities.OfType<Danger>().Sum(danger => danger.Value);
            }
        }
        public DangerManager(World parent, bool add = true) : base(null, new Vector2f(0, 0), parent, add)
        {
            Instance = this;
            new TextEntity("DP: ", new Vector2f(650, 577), parent, Fonts.ExoRegular, 16, Colors.White, Colors.Black);
            _dpDisp = new TextEntity("1/1", new Vector2f(680, 577), parent, Fonts.ExoRegular, 16, Colors.White, Colors.Black);
        }
        public override void Update()
        {
            base.Update();
            _dpDisp.Text = UsedDP + "/" + MaxDP;
        }
    }
}
