using System;
using System.Globalization;
using CultLib;
using CultLib.Input;
using SFML.Graphics;
using SFML.Window;

namespace ld25
{
    class SpawnButton : Entity
    {
        public int Val = 5;
        private readonly string _tex;
        public static SpawnButton Selected;
        public TextEntity DPDisp;
        public SpawnButton(World parent, Vector2f pos, string tex, int val, bool add = true)
            : base(Rsc.Tex(tex), pos, parent, add)
        {
            _tex = tex;
            Val = val;
            new Entity(Rsc.Tex("rsc/Button.png"), pos, parent);
            {
                Layer = Layer - 1;
            }
            Bounds = new FloatRect(pos.X, pos.Y, 64, 64);
            DPDisp = new TextEntity(val.ToString(CultureInfo.InvariantCulture), pos + new Vector2f(4, 40), parent, Fonts.ExoRegular, 18, Colors.White, Colors.Black)
                         {Layer = Layer - 2};
            Scale /= 2;
        }
        public override void Update()
        {
            base.Update();
            if(MouseManager.LeftPressed && MouseOver())
            {
                Selected = this;
                Tex = Rsc.Tex(_tex.Substring(0, _tex.IndexOf(".", StringComparison.Ordinal)) + "Selected.png");
            }
            else if(Selected != this)
            {
                Tex = Rsc.Tex(_tex);
            }
        }
        public virtual void Throw(Thrower thrower)
        {
            new Asteroid("Large", thrower.Rotation, thrower.Base, Ld25.Game, 25, true);
        }
        public override bool MouseOver()
        {
            return MouseManager.Pos.X > Pos.X && MouseManager.Pos.X < Pos.X + 64 && MouseManager.Pos.Y > Pos.Y &&
                   MouseManager.Pos.Y < Pos.Y + 64;
        }
    }

}
