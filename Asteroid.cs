using System;
using CultLib;
using SFML.Graphics;
using SFML.Window;

namespace ld25
{
    class Asteroid : Danger
    {
        public float MoveRot;
        public string Size = "";
        public bool Active;

        public Asteroid(string size, float rotation, Vector2f pos, World parent, int value, bool spawned = false, bool add = true) : base(Rsc.Tex("rsc/Asteroid" + size + ".png"), pos, parent, value, add)
        {
            Rotation = Rand.RandInt(0, 360);
            MoveRot = rotation;
            Origin = Center;
            Size = size;
            Col = new Color(255, 200, 200, 100);
            var t = new Ticker(Parent, new TimeSpan(0, 0, 2), null);
            t.OnTick = delegate
                           {
                               t.Destroy();
                               Col = Color.White;
                               Active = true;
                           };
            Active = !spawned;
            if (Active)
            {
                Col = Color.White;
            }
            if(DangerManager.Instance.UsedDP > DangerManager.Instance.MaxDP)
            {
                Destroy();
            }
        }
        public override void Update()
        {
            base.Update();
            if (Active)
            {
                Pos += Angle.GetStep(MoveRot)*3;
                if (Angle.DistanceBetween(Ship.Instance.Pos, Pos) < Tex.Size.X*0.65f)
                {
                    Parent.Parent.CurrentWorld = Ld25.GameOver;
                    Parent.Parent.UserInterface = null;
                }
            }
            Wrap.WrapPos(ref Pos);
        }
        public override void Destroy()
        {
            if (Size.Length > 2)
            {
                base.Destroy();
            }
        }
    }
}
