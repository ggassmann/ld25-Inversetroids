using System;
using CultLib;
using SFML.Graphics;
using SFML.Window;

namespace ld25
{
    class UFO : Danger
    {
        public bool Active;
        public float MoveRot;
        public UFO(Vector2f pos, float rotation, World parent, bool add = true) : base(Rsc.Tex("rsc/UFO.png"), pos, parent, 80, add)
        {
            MoveRot = rotation;
            Col = new Color(255, 200, 200, 100);
            var t = new Ticker(Parent, new TimeSpan(0, 0, 2), null);
            t.OnTick = delegate
            {
                t.Destroy();
                Col = Color.White;
                Active = true;
            };
            Active = false;
            Origin = Center;
        }
        public override void Update()
        {
            base.Update();
            if (Active)
            {
                Pos.X += (Angle.GetStep(MoveRot)*10).X;
                if (Angle.DistanceBetween(Ship.Instance.Pos, Pos) < Tex.Size.X * 0.65f)
                {
                    Parent.Parent.CurrentWorld = Ld25.GameOver;
                    Parent.Parent.UserInterface = null;
                }
            }

            Wrap.WrapPos(ref Pos);
        }
    }
}
